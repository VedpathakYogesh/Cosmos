using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Net;
using Microsoft.Azure.Documents;
using Microsoft.Azure.Documents.Client;
using MySql.Data.MySqlClient.Memcached;
using System.Threading.Tasks;
using CosmosDbDemo.Models;

namespace CosmosDbDemo.Controllers
{
    public class HomeController : Controller
    {
        string EndpointUrl;  
        string  PrimaryKey;
        
        private DocumentClient client;
       

        //private  CollectionLink collectionLink;
        public HomeController()
        {
            EndpointUrl = "https://yogeshazurecosmosdb.documents.azure.com:443/";
            PrimaryKey = "5Kgy5AARYrnjTjMIzUjOm2nQlYblrHwfuHDo5zzbjK2NpwoF4G6FXt13ElcYCIZsOm0OeUxL6rh8LMN9Uu7Jtw==";

            client = new DocumentClient(new Uri(EndpointUrl), PrimaryKey);
        }
   
        [HttpGet]
        public ActionResult AddEmployee()
        {
            return View();
        }

        public async Task<ActionResult> Employees()
        {
            await client.CreateDatabaseIfNotExistsAsync(new Database { Id = "Employee" });

            await client.CreateDocumentCollectionIfNotExistsAsync(UriFactory.CreateDatabaseUri("Employee"),
                new DocumentCollection { Id = "Items" });


          //  FeedOptions queryOptions = new FeedOptions { MaxItemCount = -1 };


            IQueryable<Employee> employeeQuery = this.client.CreateDocumentQuery<Employee>(
                    UriFactory.CreateDocumentCollectionUri("Employee", "Items"),
                    new FeedOptions { MaxItemCount = 1, EnableCrossPartitionQuery = true })
                    
                    .Where(f => f.Salary >= 100);

            //// Enable cross partition query.
            // IQueryable < Employee > employeeQuery = client.CreateDocumentQuery<Employee>(
            //    collectionLink, new FeedOptions { EnableCrossPartitionQuery = true }).Where(b => b.Salary >= 1000);

            return View(employeeQuery);
        }



        [HttpPost]
        public async Task<ActionResult> AddEmployee(Employee employee)
        {
            await this.client.CreateDocumentAsync(UriFactory.CreateDocumentCollectionUri("Employee", "Items"), employee);
            return RedirectToAction("Employees");
        }

       
        public async Task<ActionResult> DeleteEmployee(string documentId )
        {
            await this.client.DeleteDocumentAsync(UriFactory.CreateDocumentUri("Employee", "Items", documentId),
                new RequestOptions() { PartitionKey = new PartitionKey(Undefined.Value) }
               );

           // client.DeleteDocumentAsync(
           //UriFactory.CreateDocumentUri(Employee, Items, documentId),
           //new RequestOptions() { PartitionKey = new PartitionKey(Undefined.Value) });

            return RedirectToAction("Employees");
        }

        public ActionResult About()
        {
            ViewBag.Message = "Your application description page.";

            return View();
        }

        public ActionResult Contact()
        {
            ViewBag.Message = "Your contact page.";

            return View();
        }
    }
}