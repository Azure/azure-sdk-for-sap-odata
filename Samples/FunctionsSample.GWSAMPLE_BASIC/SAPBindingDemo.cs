using System.Threading.Tasks;
using DataOperations;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.Extensions.Logging;
using DataOperations.Bindings.Generated;
using GWSAMPLE_BASIC;

namespace FunctionsDemo
{
    //provided REST calls to test the bindings in odata-product-sample.http. Use with VS Code REST Client extension: https://marketplace.visualstudio.com/items?itemName=humao.rest-client
    public class SAPBindingDemo
    {
        [FunctionName("UpdateSalesOrderPrice")]
        public async Task<IActionResult> Run(
            [HttpTrigger(AuthorizationLevel.Anonymous, "get", "post", Route = "SalesOrderPriceUpdate/{ID}/{Price}")] HttpRequest req, ILogger log, int Price,
            [Input_GWSAMPLE_BASIC_SalesOrderAttribute("{ID}")] SalesOrder salesOrderInput,
            [Output_GWSAMPLE_BASIC_SalesOrderAttribute()] IAsyncCollector<SalesOrder> salesOrderCollector
        )
        {
            salesOrderInput.GrossAmount = Price;
            var AddressCity = (await salesOrderInput.ToBusinessPartner.GetAsync()).Address.City;
            await salesOrderCollector.AddAsync(salesOrderInput);
            return new OkObjectResult($"Updated Sales order price to {Price}, oh and the order will go to {AddressCity}");
        }
       
        [FunctionName("Products")]
        public async Task<IActionResult> RunProduct([HttpTrigger(AuthorizationLevel.Anonymous, "get", Route = "Products/{top}")] HttpRequest req, ILogger log, int top,
        [Input_GWSAMPLE_BASIC_ProductSetAttribute()] ProductSet productListInput)
        {   
            return new OkObjectResult(
                await productListInput.GetListAsync(QueryTop.TopFactory(top), null, 
                    QueryOrderBy.OrderByFactory("ProductID"), 
                    QueryFilter.FilterFactory(new QueryFilterExpression("ProductID", FilterOperator.startswith,"HT")),null)
                );


        }

        [FunctionName("GetDeliveryAddressLabel")]
        public async Task<IActionResult> RunAddress(
            [HttpTrigger(AuthorizationLevel.Anonymous, "get", "post", Route = "GetDeliveryAddressLabel/{ID}")] HttpRequest req, ILogger log, string ID,
            [Input_GWSAMPLE_BASIC_SalesOrderAttribute("{ID}")] SalesOrder salesOrderInput
        )
        {
            CT_Address Address = (await salesOrderInput.ToBusinessPartner.GetAsync()).Address;

            var stringResult = 
                $"<!doctype html><html lang=\"en\">" +
                $" <head>" + 
                $"  <meta charset=\"utf-8\"><meta name=\"viewport\" content=\"width=device-width, initial-scale=1\"><title>Address Label for Order {salesOrderInput.SalesOrderID}</title>" + 
                $"  <link rel=\"stylesheet\" href=\"https://cdn.jsdelivr.net/npm/bootstrap@5.2.3/dist/css/bootstrap.min.css\" integrity=\"sha384-rbsA2VBKQhggwzxH7pPCaAqO46MgnOM80zW1RWuH61DGLwZJEdK2Kadq2F9CUG65\" crossorigin=\"anonymous\">" + 
                $"  <script src=\"https://cdn.jsdelivr.net/npm/bootstrap@5.2.3/dist/js/bootstrap.min.js\" integrity=\"sha384-cuYeSxntonz0PPNlHhBs68uyIAVpIIOZZ5JqeqvYYIcEL727kskC66kF92t6Xl2V\" crossorigin=\"anonymous\"></script>" + 
                $" </head>" + 
                $" <body>" + 
                $"  <div class=\"card w-50 border-primary text-white bg-primary mb-3\">" + 
                $"   <div class=\"card-header text-white\">Order: {salesOrderInput.SalesOrderID}</div>" + 
                $"   <div class=\"card-body bg-white text-black\">" + 
                $"    <h5 class=\"card-title text-primary\">{salesOrderInput.CustomerName}</h5>" + 
                $"    <p class=\"card-text\">{Address.Building} {Address.Street}</p></p>" + 
                $"    <p class=\"card-text\">{Address.City}</p>" + 
                $"    <p class=\"card-text\">{Address.Country}</p>" + 
                $"    <p class=\"card-text\">{Address.PostalCode}</p>" + 
                $"   </div>" + 
                $"  </div>" + 
                $" </body>" + 
                $"</html>";
            
            var objectresult = new ContentResult();
            objectresult.Content = stringResult;
            objectresult.ContentType = "text/html";
            objectresult.StatusCode = 200;

            return objectresult;
        }
    }
}
