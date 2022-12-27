using System;
using System.Collections.Generic;
using System.Linq;
using DataOperations;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
namespace GWSAMPLE_BASIC
{
    public class TestService : BackgroundService, IHostedService
    {
        private readonly ILogger _logger;
        private readonly IOperationsDispatcher _dispatcher;
        public BusinessPartnerSet bps;
        public ProductSet ps;
        public SalesOrderLineItemSet sos;

        public TestService(ILogger<TestService> logger, IOperationsDispatcher dispatcher, SalesOrderSet _sos, ProductSet _ps, BusinessPartnerSet _bps
        )
        {
            _logger = logger;
            _dispatcher = dispatcher;
            this.sos = _sos;
            this.ps = _ps;
            this.bps = _bps;
        }
        protected async override Task ExecuteAsync(CancellationToken stoppingToken)
        {

            _logger.LogInformation("Test is starting...");

            // get product 'ht-1023'
            var product = await ps.GetAsync("HT-1023");

            // get business partner '0100000001', update the address building to "16 Brook Meadow"
            var businessPartner = await bps.GetAsync("0100000001");
            businessPartner.Address.Building = "16 Brook";
            await bps.UpdateAsync(businessPartner);

            // Dump all salesOrderLineItem numbers to the console one by one (!)
            (await sos.GetListAsync()).ToList().ForEach(salesOrderLineItem => {
                Console.WriteLine($"Sales Order: {salesOrderLineItem.SalesOrderID}");
            });

            // get me a sales order line item with id 0500000000 at position 10 and update the note to "Test Note"
            // be aware of required states of the sales orders in SAP for this service: https://blogs.sap.com/2019/06/06/es5-error-message-create-is-not-allowed-because-of-property-value/
            var selectors = new Dictionary<string, object>();
            selectors.Add("SalesOrderID", "0500000000");
            selectors.Add("ItemPosition", "0000000010");
            var salesOrderLineItem = await sos.GetAsync(selectors);
            salesOrderLineItem.Note = "Test Note";
            await sos.UpdateAsync(salesOrderLineItem);

            Product pro = await ps.GetAsync("HT-1023");
            if(pro.ProductID != "HT-1023") throw new Exception("Failed GET");
            _logger.LogInformation(pro.Description);
            _logger.LogInformation("Test is complete.");
        }
    }
}
