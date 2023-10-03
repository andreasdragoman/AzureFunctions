using System;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Azure.Documents;
using Microsoft.Azure.Documents.Client;
using Microsoft.Azure.ServiceBus;
using Microsoft.Azure.WebJobs;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;

namespace PersonDelete.Function
{
    public class DeletePersonSBTrigger
    {
        [FunctionName("DeletePersonSBTrigger")]
        public async Task Run([ServiceBusTrigger("delete-person-queue", Connection = "SBConnection")]Message myQueueItem,
            [CosmosDB(ConnectionStringSetting = "CosmosDbConnectionString")] DocumentClient client,
            ILogger log)
        {
            string personId = JsonConvert.DeserializeObject<string>(Encoding.UTF8.GetString(myQueueItem.Body));
            log.LogInformation($"C# ServiceBus queue trigger function processed message person id: {personId}");

            // method 1
            // var option = new FeedOptions { EnableCrossPartitionQuery = true };  
            // var collectionUri = UriFactory.CreateDocumentCollectionUri("MainDatabase", "Persons");
            // var document = client.CreateDocumentQuery(collectionUri, option).Where(t => t.Id == personId).AsEnumerable().FirstOrDefault();

            // await client.DeleteDocumentAsync(document.SelfLink, new RequestOptions
            // {
            //     PartitionKey = new Microsoft.Azure.Documents.PartitionKey(Undefined.Value)
            // });

            //method 2
            await client.DeleteDocumentAsync(
                UriFactory.CreateDocumentUri("MainDatabase", "Persons", personId), 
                new RequestOptions() { PartitionKey = new PartitionKey(Undefined.Value) }
            );

        }
    }

    public class PersonModel
    {
        public Guid? Id { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
    }
}
