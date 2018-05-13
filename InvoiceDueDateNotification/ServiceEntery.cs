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
    
    public partial class ServiceEntery
    {
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors")]
        public ServiceEntery()
        {
            this.Dollars = new HashSet<Dollar>();
            this.Nairas = new HashSet<Naira>();
            this.Reports = new HashSet<Report>();
        }
    
        public long Id { get; set; }
        public string SENumber { get; set; }
        public string InvoiceNumber { get; set; }
        public string ClientName { get; set; }
    
        public virtual Client Client { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<Dollar> Dollars { get; set; }
        public virtual Invoice Invoice { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<Naira> Nairas { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<Report> Reports { get; set; }
    }
}