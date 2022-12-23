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
    }
}
