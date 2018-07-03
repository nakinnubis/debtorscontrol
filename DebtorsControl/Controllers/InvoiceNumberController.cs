using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using DebtorsControl.Models;

namespace DebtorsControl.Controllers
{
    public class InvoiceNumberController : ApiController
    {
        // GET api/<controller>
       // [HttpGet]
        public IEnumerable<InvoiceNum> Get(string clientName)
        {
            using (pdInvoiceEntities db = new pdInvoiceEntities())
            {
                var invoice = db.Invoices.Where(c => c.ClientName == clientName).Select(c=> new InvoiceNum { InvoiceNumber = c.InvoiceNumber }).Where(c=>c.InvoiceNumber !=null && !c.InvoiceNumber.Equals("NULL")).ToList();
                invoice.Insert(0, new InvoiceNum { InvoiceNumber = "Select Invoice Number" });
                return invoice;
            }
            
        }

        // GET api/<controller>/5
        public string Get(int id)
        {
            return "value";
        }

        // POST api/<controller>
        public void Post([FromBody]string value)
        {
        }

        // PUT api/<controller>/5
        public void Put(int id, [FromBody]string value)
        {
        }

        // DELETE api/<controller>/5
        public void Delete(int id)
        {
        }
    }

    public class InvoiceNum
    {
        public string InvoiceNumber { get; set; }
    }

    public class ServiceEnNum
    {
        public string SeNumber { get; set; }
    }
    public class ServiceEntriesController : ApiController
    {
        public IEnumerable<ServiceEnNum> Get(string clientName,string invoicenum)
        {
            using (pdInvoiceEntities db = new pdInvoiceEntities())
            {
                var invoice = db.ServiceEnteries.Where(c => c.ClientName == clientName &&c.InvoiceNumber== invoicenum).Select(c=> new ServiceEnNum
                {
                    SeNumber = c.SENumber
                }).ToList();
                return invoice;
            }

        }
    }
}