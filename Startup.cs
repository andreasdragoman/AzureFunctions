using Microsoft.Azure.Functions.Extensions.DependencyInjection;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Configuration.AzureAppConfiguration;
using Microsoft.FeatureManagement;
using System;

[assembly: FunctionsStartup(typeof(AzureFunctions.Startup))]
namespace AzureFunctions
{
    public class Startup : FunctionsStartup
    {
        

        public override void ConfigureAppConfiguration(IFunctionsConfigurationBuilder builder)
        {
            base.ConfigureAppConfiguration(builder);

            var settings = builder.ConfigurationBuilder.Build();
            builder.ConfigurationBuilder.AddAzureAppConfiguration(options =>
                options
                    .Connect(settings["AppConfigConnectionString"])
                    .Select(KeyFilter.Any, "dev")
                    .UseFeatureFlags()
                    .ConfigureRefresh(x =>
                        {
                            x.Register("refreshAll", refreshAll: true);
                            x.SetCacheExpiration(TimeSpan.FromSeconds(1));
                        }
                    )
            );
        }

        public override void Configure(IFunctionsHostBuilder builder)
        {
            builder.Services.AddFeatureManagement();
            builder.Services.AddAzureAppConfiguration();
        }
    }
}
