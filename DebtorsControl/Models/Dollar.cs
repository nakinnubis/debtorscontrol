//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated from a template.
//
//     Manual changes to this file may cause unexpected behavior in your application.
//     Manual changes to this file will be overwritten if the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace DebtorsControl.Models
{
    using System;
    using System.Collections.Generic;
    
    public partial class Dollar
    {
        public long Id { get; set; }
        public string ClientName { get; set; }
        public Nullable<int> Year { get; set; }
        public Nullable<int> Month { get; set; }
        public string InvoicedMonth { get; set; }
        public string InvoiceNumber { get; set; }
        public string SENumber { get; set; }
        public Nullable<decimal> Amount { get; set; }
        public Nullable<decimal> VAT { get; set; }
        public Nullable<decimal> TotalInvoice { get; set; }
        public Nullable<decimal> Payable { get; set; }
        public Nullable<decimal> LcdCharge { get; set; }
        public Nullable<decimal> AmountPaid { get; set; }
        public Nullable<decimal> Outstanding { get; set; }
        public Nullable<System.DateTime> DateSubmitted { get; set; }
        public Nullable<System.DateTime> DueDate { get; set; }
        public Nullable<System.DateTime> DatePaid { get; set; }
        public Nullable<decimal> WithHoldinTax { get; set; }
        public string RemittanceAdvise { get; set; }
        public string Comments { get; set; }
    }
}
