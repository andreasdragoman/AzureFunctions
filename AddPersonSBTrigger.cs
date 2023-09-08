using System;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Host;
using Microsoft.Extensions.Logging;

namespace Person.Function
{
    public class AddPersonSBTrigger
    {
        [FunctionName("AddPersonSBTrigger")]
        public void Run(
            [ServiceBusTrigger("personqueue", Connection = "SBConnection")]
            string myQueueItem,
            ILogger log)
        {
            log.LogInformation($"C# ServiceBus queue trigger function processed message: {myQueueItem}");
        }
    }
}
