﻿//------------------------------------------------------------------------------
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
    using System.Data.Entity;
    using System.Data.Entity.Infrastructure;
    
    public partial class pdInvoiceEntities : DbContext
    {
        public pdInvoiceEntities()
            : base("name=pdInvoiceEntities")
        {
        }
    
        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            throw new UnintentionalCodeFirstException();
        }
    
        public virtual DbSet<ActivityLog> ActivityLogs { get; set; }
        public virtual DbSet<Client> Clients { get; set; }
        public virtual DbSet<Dollar> Dollars { get; set; }
        public virtual DbSet<Invoice> Invoices { get; set; }
        public virtual DbSet<Naira> Nairas { get; set; }
        public virtual DbSet<Report> Reports { get; set; }
        public virtual DbSet<Role> Roles { get; set; }
        public virtual DbSet<ServiceEntery> ServiceEnteries { get; set; }
        public virtual DbSet<Staff> Staffs { get; set; }
    }
}
