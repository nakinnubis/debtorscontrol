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
    
    public partial class Naira
    {
        public long Id { get; set; }
        public string ClientName { get; set; }
        public Nullable<int> Year { get; set; }
        public Nullable<int> Month { get; set; }
        public string InvoiceNumber { get; set; }
        public string SENumber { get; set; }
        public Nullable<decimal> FxRate { get; set; }
        public decimal Amount { get; set; }
        public decimal VAT { get; set; }
        public decimal TotalInvoice { get; set; }
        public decimal Payable { get; set; }
        public decimal LcdCharge { get; set; }
        public decimal AmountPaid { get; set; }
        public decimal Outstanding { get; set; }
        public System.DateTime DateSubmitted { get; set; }
        public System.DateTime DueDate { get; set; }
        public Nullable<System.DateTime> DatePaid { get; set; }
        public decimal WithHoldingTax { get; set; }
        public string RemittanceAdvise { get; set; }
        public string Comments { get; set; }
        public Nullable<decimal> NairaValue { get; set; }
    
        public virtual Client Client { get; set; }
        public virtual Invoice Invoice { get; set; }
    }
}