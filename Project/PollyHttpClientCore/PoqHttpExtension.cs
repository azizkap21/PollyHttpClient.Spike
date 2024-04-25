using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using Polly;
using Polly.Extensions.Http;
using PollyHttpClientCore.Options;
using System;


namespace PollyHttpClientCore
{
    public static class PoqHttpExtension
    {
        public static IServiceCollection AddCustomHttpClient<TClient, TClientImplementation>(this IServiceCollection services, string clientName, ApplicationOptions options)
                where TClient : class
                where TClientImplementation : class, TClient => services.AddHttpClient<TClient, TClientImplementation>(clientName)
                                                                .ConfigureHttpClient((client)=>client.BaseAddress = options.HttpClientOptions.BaseAddress)
                                                                .AddPolicyHandlerFromRegistry(PolicyName.PolicyNameWithPrefix(clientName, PolicyName.HttpTimeout))
                                                                .AddPolicyHandlerFromRegistry(PolicyName.PolicyNameWithPrefix(clientName, PolicyName.HttpRetry))
                                                                .AddPolicyHandlerFromRegistry(PolicyName.PolicyNameWithPrefix(clientName, PolicyName.HttpCircuitBreaker))
                                                                .Services;

        public static IServiceCollection AddPolicies(this IServiceCollection services, string clientName, PolicyOptions policyOptions)
        {
            var noOpPolicy = Policy.NoOpAsync();

            var policyRegistry = services.AddPolicyRegistry();

            policyRegistry.Add(PolicyName.HttpTimeout, Policy.Timeout(policyOptions.HttpTimeout.Timeout));

            if (policyOptions.HttpRetry != null)
            {
                policyRegistry.Add(
                    PolicyName.PolicyNameWithPrefix(clientName, PolicyName.HttpRetry),
                    HttpPolicyExtensions
                        .HandleTransientHttpError()
                        .WaitAndRetryAsync(
                            policyOptions.HttpRetry.Count,
                            retryAttempt => TimeSpan.FromSeconds(Math.Pow(policyOptions.HttpRetry.BackoffPower, retryAttempt))));
            }
            else
            {
                policyRegistry.Add(PolicyName.PolicyNameWithPrefix(clientName, PolicyName.HttpRetry), noOpPolicy);
            }

            if (policyOptions.HttpCircuitBreaker != null)
            {
                policyRegistry.Add(
                    PolicyName.PolicyNameWithPrefix(clientName, PolicyName.HttpCircuitBreaker),
                    HttpPolicyExtensions
                        .HandleTransientHttpError()
                        .CircuitBreakerAsync(
                            handledEventsAllowedBeforeBreaking: policyOptions.HttpCircuitBreaker.ExceptionsAllowedBeforeBreaking,
                            durationOfBreak: policyOptions.HttpCircuitBreaker.DurationOfBreak));
            }
            else
            {
                policyRegistry.Add(PolicyName.PolicyNameWithPrefix(clientName, PolicyName.HttpCircuitBreaker), noOpPolicy);
            }

            policyRegistry.Add(PolicyName.PolicyNameWithPrefix(clientName, PolicyName.HttpTimeout), Policy.Timeout(policyOptions.HttpTimeout.Timeout));

            return services;
        }
    }

    public static class PolicyName
    {
        public static string PolicyNameWithPrefix(string prefix, string policyName)
        {
            return $"{prefix}-{policyName}";
        }

        public const string HttpCircuitBreaker = nameof(HttpCircuitBreaker);
        public const string HttpRetry = nameof(HttpRetry);
        public const string HttpTimeout = nameof(HttpTimeout);
    }

}
