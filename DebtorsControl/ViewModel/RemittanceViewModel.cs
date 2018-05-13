using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace DebtorsControl.ViewModel
{
    public class RemittanceViewModel
    {
        public List<Clients> DoRemittance { get; set; }
        public IEnumerable<InvoiceType> InvoiceTypes { get; set; }
    }

    public class Remittance
    {
        public string ClientName { get; set; }
        public string InvoiceNumber { get; set; }
        public string ServiceEntry { get; set; }
       // public List<string> InvoiceType { get; set; }
    }

    public class InvoiceType
    {
        public string InvoicesType { get; set; }
    }
}