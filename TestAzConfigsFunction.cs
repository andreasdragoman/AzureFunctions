using System;
using System.IO;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using Microsoft.Extensions.Configuration;
using Microsoft.FeatureManagement;
using Microsoft.Extensions.Configuration.AzureAppConfiguration;
using System.Linq;

namespace AzureFunctions
{
    public class TestAzConfigsFunction
    {
        private readonly IConfiguration _configuration;
        private readonly IFeatureManager _featureManager;
        private readonly IConfigurationRefresher _configurationRefresher;

        public TestAzConfigsFunction(IConfiguration configuration, IFeatureManager featureManager, IConfigurationRefresherProvider configurationRefresherProvider)
        {
            _configuration = configuration;
            _featureManager = featureManager;
            _configurationRefresher = configurationRefresherProvider.Refreshers.First();

        }

        [FunctionName("TestAzConfigsFunction")]
        public async Task<IActionResult> Run(
            [HttpTrigger(AuthorizationLevel.Function, "get", "post", Route = null)] HttpRequest req,
            ILogger log)
        {
            await _configurationRefresher.TryRefreshAsync();
            string responseMessage = $"[database1:host config value]: [{_configuration["database1:host"]}]"
                + $" ~ [database1:password config value]: [{_configuration["database1:password"]}]"
                + $" ~ [Feature flag value]: [{await _featureManager.IsEnabledAsync("sbdemo1508")}]";

            return new OkObjectResult(responseMessage);
        }
    }
}
