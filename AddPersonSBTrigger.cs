using System;
using System.Text;
using Azure.Messaging.ServiceBus;
using Microsoft.Azure.Documents.SystemFunctions;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Host;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;

namespace Person.Function
{
    public class AddPersonSBTrigger
    {
        [FunctionName("AddPersonSBTrigger")]
        public void Run(
            [ServiceBusTrigger("add-person-queue", Connection = "SBConnection")]
            ServiceBusReceivedMessage myQueueItem,
            [CosmosDB(
                databaseName: "MainDatabase",
                collectionName: "Persons",
                ConnectionStringSetting = "CosmosDbConnectionString"
            )]
            out dynamic document,
            ILogger log)
        {
            Person person = JsonConvert.DeserializeObject<Person>(Encoding.UTF8.GetString(myQueueItem.Body));
            document = new { personId = Guid.NewGuid(), firstName = person.FirstName, lastName = person.LastName };
            log.LogInformation($"C# ServiceBus queue trigger function processed message: {myQueueItem.Body}");
        }
    }

    public class Person 
    {
        //public Guid Id { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
    }
}
