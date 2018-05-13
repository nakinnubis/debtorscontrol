//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated from a template.
//
//     Manual changes to this file may cause unexpected behavior in your application.
//     Manual changes to this file will be overwritten if the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace InvoiceDueDateNotification
{
    using System;
    using System.Collections.Generic;
    
    public partial class Report
    {
        public long Id { get; set; }
        public string ClientName { get; set; }
        public int Year { get; set; }
        public int Month { get; set; }
        public string InvoiceNumber { get; set; }
        public string SENumber { get; set; }
        public decimal Amount { get; set; }
        public decimal AmountPaid { get; set; }
        public decimal Outstanding { get; set; }
        public System.DateTime DatePaid { get; set; }
        public bool WithHoldingTaxStatus { get; set; }
        public bool RemittanceAdviseStatus { get; set; }
    
        public virtual Client Client { get; set; }
        public virtual Invoice Invoice { get; set; }
        public virtual ServiceEntery ServiceEntery { get; set; }
    }
}
