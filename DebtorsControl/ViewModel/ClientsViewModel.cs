using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace DebtorsControl.ViewModel
{
    public class ClientsViewModel
    {
        public List<Clients> GetClients { get; set; }
        public List<InvoiceNumber> GetInvoiceNumbers { get; set; }
        public List<ServiceEntry> GetServiceEntries { get; set; }
    }

    public class Clients
    {
        public string ClientName { get; set; }
        public string ClientCode { get; set; }
        public string ClientLogo { get; set; }
    }

    public class InvoiceNumber
    {
        public string InvoiceNo { get; set; }
        public string ClientName { get; set; }
    }

    public class ServiceEntry
    {
        public string InvoiceNo { get; set; }
        public string SeNumber { get; set; }
        public string ClientName { get; set; }
    }
}