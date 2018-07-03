using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using DebtorsControl.Models;

namespace DebtorsControl.Controllers
{
    public class ServiceEntryController : ApiController
    {
        // GET api/<controller>
        public IEnumerable<ServiceNum> Get(string clientName, string invoicenumber)
        {
            using (pdInvoiceEntities db = new pdInvoiceEntities())
            {
                var invoice = db.ServiceEnteries.Where(c => c.ClientName == clientName && c.InvoiceNumber==invoicenumber).Select(c=> new ServiceNum { ServiceNumber = c.SENumber }).Where(c=>c.ServiceNumber !=null && !c.ServiceNumber.Equals("NULL")).ToList();
                invoice.Insert(0, new ServiceNum {ServiceNumber = "Select Service Number"});
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
    public class ServiceNum
    {
        public string ServiceNumber { get; set; }
    }
}