using System;
using System.Text;
using Microsoft.Azure.ServiceBus;
using Microsoft.Azure.WebJobs;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;

namespace PersonAdd.Function
{
    public class AddPersonSBTrigger
    {
        [FunctionName("AddPersonSBTrigger")]
        public void Run(
            [ServiceBusTrigger("add-person-queue", Connection = "SBConnection")]
            Message myQueueItem,
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
        public Guid? Id { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
    }
}
