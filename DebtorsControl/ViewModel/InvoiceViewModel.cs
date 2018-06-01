using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace DebtorsControl.ViewModel
{
    public class InvoiceViewModel
    {
        public List<NairaInvoice> NairaInvoices { get; set; }
        public List<DollarInvoice> DollarInvoices { get; set; }
        public Summation Summations { get; set; }
        public List<Clients> GetClients { get; set; }
        public List<Year> YearList { get; set; }
        public List<Reconcilation> Reconcilations { get; set; }
    }

    public class NairaInvoice
    {
        public long Id { get; set; }
        public string ClientName { get; set; }
        public int? Year { get; set; }     
        public string Date { get; set; }
        public string InvoicedMonth { get; set; }
        public string InvoiceNumber { get; set; }
        public string SeNumber { get; set; }
        public decimal? FxRate { get; set; }
        public decimal? Amount { get; set; }
        public decimal? Vat { get; set; }
        public decimal? TotalInvoice { get; set; }
        public decimal? Payable { get; set; }
        public decimal? NairaValue { get; set; }
        public decimal? LcdCharge { get; set; }
        public decimal? AmountPaid { get; set; }
        public decimal? Outstanding { get; set; }
        public DateTime? DateSubmitted { get; set; }
        public DateTime? DatePaid { get; set; }
        public DateTime? DueDate { get; set; }
        public decimal? WithHoldingTax { get; set; }
        public string RemittanceAdvise { get; set; }
        public string Comments { get; set; }
       
    }
   

    public class DollarInvoice
    {
        public long Id { get; set; }
        public string ClientName { get; set; }
        public int? Year { get; set; }
        public int Month { get; set; }
        public string Date { get; set; }
        public string InvoiceNumber { get; set; }
        public string SeNumber { get; set; }
        public decimal? Amount { get; set; }
        public decimal? Vat { get; set; }
        public decimal? TotalInvoice { get; set; }
        public decimal? Payable { get; set; }
        public decimal? LcdCharge { get; set; }
        public decimal? AmountPaid { get; set; }
        public decimal? Outstanding { get; set; }
        public DateTime? DateSubmitted { get; set; }
        public DateTime? DueDate { get; set; }
        public DateTime DatePaid { get; set; }
        public decimal? WithHoldinTax { get; set; }
        public string RemittanceAdvise { get; set; }
        public string Comments { get; set; }
    }

    public class Summation
    {
        public decimal Amount { get; set; }
        public decimal Vat { get; set; }
        public decimal Invoiced { get; set; }
        public decimal Payable { get; set; }
        public decimal Paid { get; set; }
        public decimal Lcd { get; set; }
        public decimal Oustanding { get; set; }
    }

    public class Reconcilation
    {
        public string ClientName { get; set; }
        public decimal? NairaOutsnd { get; set; }
        public decimal? DollarOutsnd { get; set; }
    }

    public class Year
    {
        public int Years { get; set; }
    }
}