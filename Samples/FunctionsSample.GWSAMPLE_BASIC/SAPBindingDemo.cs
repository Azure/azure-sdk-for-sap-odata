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

        [FunctionName("UpdateBusinessPartnerAddress")]
        public async Task<IActionResult> RunUpdate(
            [HttpTrigger(AuthorizationLevel.Anonymous, "get", "post", Route = "UpdateBusinessPartnerAddress/{ID}/{Building}")] HttpRequest req, ILogger log, string Building,
            [Input_GWSAMPLE_BASIC_BusinessPartnerAttribute("{ID}")] BusinessPartner BusinessPartnerInput,
            [Output_GWSAMPLE_BASIC_BusinessPartnerAttribute()] IAsyncCollector<BusinessPartner> BusinessPartnerCollector
        )
        {
            
            try
            {
                BusinessPartnerInput.Address.Building = Building;
                await BusinessPartnerCollector.AddAsync(BusinessPartnerInput);
                
                return new HtmlPageResult(
                    $"New Address updated for BP: {BusinessPartnerInput.BusinessPartnerID}",
                    $"  <div class=\"card w-50 border-primary text-white bg-primary mb-3\">" + 
                    $"   <div class=\"card-header text-white\">Business Partner: {BusinessPartnerInput.BusinessPartnerID}</div>" + 
                    $"   <div class=\"card-body bg-white text-black\">" + 
                    $"    <h5 class=\"card-title text-primary\">{BusinessPartnerInput.CompanyName}</h5>" + 
                    $"    <p class=\"card-text\">{BusinessPartnerInput.Address.Building} {BusinessPartnerInput.Address.Street}</p></p>" + 
                    $"    <p class=\"card-text\">{BusinessPartnerInput.Address.City}</p>" + 
                    $"    <p class=\"card-text\">{BusinessPartnerInput.Address.Country}</p>" + 
                    $"    <p class=\"card-text\">{BusinessPartnerInput.Address.PostalCode}</p>" + 
                    $"   </div>" + 
                    $"  </div>"
                );
            }
            catch (System.Exception ex)
            {
                return new BadRequestObjectResult(ex.Message + ", " +  ex.InnerException?.Message);
            }
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

            return new HtmlPageResult(
                $"Delivery Address Label for Order: {salesOrderInput.SalesOrderID}",
                $"  <div class=\"card w-50 border-primary text-white bg-primary mb-3\">" + 
                $"   <div class=\"card-header text-white\">Order: {salesOrderInput.SalesOrderID}</div>" + 
                $"   <div class=\"card-body bg-white text-black\">" + 
                $"    <h5 class=\"card-title text-primary\">{salesOrderInput.CustomerName}</h5>" + 
                $"    <p class=\"card-text\">{Address.Building} {Address.Street}</p></p>" + 
                $"    <p class=\"card-text\">{Address.City}</p>" + 
                $"    <p class=\"card-text\">{Address.Country}</p>" + 
                $"    <p class=\"card-text\">{Address.PostalCode}</p>" + 
                $"   </div>" + 
                $"  </div>"
            );
            
        }
    }
    public class HtmlPageResult : ContentResult
    {
        public string BaseTemplate = $"<!doctype html><html lang=\"en\">" +
                $" <head>" + 
                $"  <meta charset=\"utf-8\"><meta name=\"viewport\" content=\"width=device-width, initial-scale=1\"><title>{{title}}</title>" + 
                $"  <link rel=\"stylesheet\" href=\"https://cdn.jsdelivr.net/npm/bootstrap@5.2.3/dist/css/bootstrap.min.css\" integrity=\"sha384-rbsA2VBKQhggwzxH7pPCaAqO46MgnOM80zW1RWuH61DGLwZJEdK2Kadq2F9CUG65\" crossorigin=\"anonymous\">" + 
                $"  <script src=\"https://cdn.jsdelivr.net/npm/bootstrap@5.2.3/dist/js/bootstrap.min.js\" integrity=\"sha384-cuYeSxntonz0PPNlHhBs68uyIAVpIIOZZ5JqeqvYYIcEL727kskC66kF92t6Xl2V\" crossorigin=\"anonymous\"></script>" + 
                $" </head>" + 
                $" <body>" + 
                $"  {{bodyhtml}}" +
                $" </body>" + 
                $"</html>";

        public HtmlPageResult(string title, string html)
        {
            Content = BaseTemplate.Replace("{{title}}", title).Replace("{{bodyhtml}}", html);
            ContentType = "text/html";
            StatusCode = 200;
        }
    }
}
