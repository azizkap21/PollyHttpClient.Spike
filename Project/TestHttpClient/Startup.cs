using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.HttpsPolicy;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using PollyHttpClientCore;
using PollyHttpClientCore.Options;

namespace TestHttpClient
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            //var section = Configuration.GetSection("Clients");

            var clientConfigurations = Configuration.GetSection("Clients").Get<Dictionary<string, ApplicationOptions>>();

            //Configuration.Bind ("Clients", clientConfigurations);

            services.AddMvc().SetCompatibilityVersion(CompatibilityVersion.Version_2_1);

            foreach (var applicationOption in clientConfigurations)
            {
                services.AddPolicies(applicationOption.Key, applicationOption.Value.Policies)
                .AddCustomHttpClient<ICustomHttpClient, CustomHttpClient>(applicationOption.Key, applicationOption.Value);
            }    
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IHostingEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }
            else
            {
                app.UseHsts();
            }

            app.UseHttpsRedirection();
            app.UseMvc();
        }
    }
}
