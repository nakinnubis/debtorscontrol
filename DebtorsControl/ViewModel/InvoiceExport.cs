using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace DebtorsControl.ViewModel
{
    public class InvoiceExport
    {  public string Date { get; set; }
        public string InvoiceType { get; set; }
        public string ClientName { get; set; }
        //public int? Year { get; set; }
        //public int? Month { get; set; }      
        public string InvoiceNumber { get; set; }
        public string SeNumber { get; set; }
        public decimal? FxRate { get; set; }
        public decimal? Amount { get; set; }
        public decimal? Vat { get; set; }
        public decimal? TotalInvoice { get; set; }
        public decimal? Payable { get; set; }
        public decimal? NairaEquiv { get; set; }
        public decimal? LcdCharge { get; set; }
        public decimal? AmountPaid { get; set; }
        public decimal? Outstanding { get; set; }
        public string DateSubmitted { get; set; }
        public string DueDate { get; set; }
        public decimal? WithHoldingTax { get; set; }
        public string RemittanceAdvise { get; set; }
        public string Comments { get; set; }
       
    }

    public class DebtSummary
    {
        public string InvoiceType { get; set; }
        public string ClientName { get; set; }
        public  int? Year { get; set; }
        public decimal? TotalDue { get; set; }
    }
}