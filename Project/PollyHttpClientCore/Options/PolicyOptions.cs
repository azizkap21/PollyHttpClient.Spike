using Polly;
using System;
using System.Collections.Generic;
using System.Text;

namespace PollyHttpClientCore.Options
{
    public class PolicyOptions
    {
        public CircuitBreakerPolicyOptions HttpCircuitBreaker { get; set; }

        public RetryPolicyOptions HttpRetry { get; set; }

        public TimeOutOptions HttpTimeout { get; set; }
    }
}
