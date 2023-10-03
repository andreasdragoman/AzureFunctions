using System;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Azure.Documents.Client;
using Microsoft.Azure.ServiceBus;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Host;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;

namespace PersonUpdate.Function
{
    public class UpdatePersonSBTrigger
    {
        [FunctionName("UpdatePersonSBTrigger")]
        public async Task Run([ServiceBusTrigger("update-person-queue", Connection = "SBConnection")]Message myQueueItem,
            [CosmosDB(ConnectionStringSetting = "CosmosDbConnectionString")] DocumentClient client,
            ILogger log)
        {
            log.LogInformation($"C# ServiceBus queue trigger function processed message: {myQueueItem}");
            Person person = JsonConvert.DeserializeObject<Person>(Encoding.UTF8.GetString(myQueueItem.Body));
            var option = new FeedOptions { EnableCrossPartitionQuery = true };  
            var collectionUri = UriFactory.CreateDocumentCollectionUri("MainDatabase", "Persons");
            var document = client.CreateDocumentQuery(collectionUri, option).Where(t => t.Id == person.Id.ToString()).AsEnumerable().FirstOrDefault();

            document.SetPropertyValue("FirstName", person.FirstName);
            document.SetPropertyValue("LastName", person.LastName);

            await client.ReplaceDocumentAsync(document);
        }
    }

    public class Person 
    {
        public Guid? Id { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
    }
}
