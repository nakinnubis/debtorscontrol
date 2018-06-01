using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using DebtorsControl.Models;

namespace DebtorsControl.Controllers
{
    public class PaidvsUnpaidController : ApiController
    {
        // GET api/<controller>
        public IEnumerable<Activiti> Get(string month)
        {
         //   return new string[] { "value1", "value2" };
            var m = int.Parse(month);
            using (pdInvoiceEntities db = new pdInvoiceEntities())
            {
                var paid = db.Nairas.Count(c => c.Outstanding == 0 && c.DateSubmitted.Value.Month.Equals(m));
                var unpaid = db.Nairas.Count(c => c.Outstanding != 0 && c.DateSubmitted.Value.Month.Equals(m));
                var data = new Activiti
                {
                    Paid = paid,
                    Unpaid = unpaid
                };
                var finaldata = new List<Activiti> {data};
                return finaldata;
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

    //public class CreditorController : ApiController
    //{
    //    public IEnumerable<Creditors> Get(string year)
    //    {
    //        var m = int.Parse(year);
    //        using (pdInvoiceEntities db = new pdInvoiceEntities())
    //        {
    //            var data= db.Nairas.GroupBy(c=>c.Month).Select(c=> new)
    //        }
    //    }
    //}
    public class TopCreditorsController : ApiController
    {
        // GET api/<controller>
        public IEnumerable<string> Get()
        {
            return new string[] { "value1", "value2" };
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
    public class TopDebtorsController : ApiController
    {
        // GET api/<controller>
        public IEnumerable<string> Get()
        {
            return new string[] { "value1", "value2" };
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

    public class NairaDebtController : ApiController
    {
        public IEnumerable<Debts> Get()
        {
            var paid = new List<Debts>();
            using (pdInvoiceEntities db = new pdInvoiceEntities())
            {
                var m = Enumerable.Range(1, 12);
                var months = db.Nairas.Select(c => c.DateSubmitted.Value.Month).Distinct().AsEnumerable();
                var year = DateTime.Now.Year;
                var data = db.Nairas.Where(c => c.DateSubmitted.Value.Year == year).ToList();
                foreach (var b in m)
                {
                    var ninflow =data.Where(c => c.DateSubmitted.Value.Month == b).Sum(c => c.AmountPaid);
                    var outflow = data.Where(c => c.DateSubmitted.Value.Month == b).Sum(c => c.Outstanding);
                    var p = new Debts
                    {
                      //  Dtype = "Paid",
                        //Flow = Convert.ToDecimal(ninflow),
                        Month = CultureInfo.CurrentCulture.DateTimeFormat.GetMonthName(b),
                        Paid = Convert.ToDecimal(ninflow),
                        Unpaid = outflow
                    };
                    paid.Add(p);
                   
                }

                return paid;
            }

        }
    }
    public class DollarDebtController : ApiController
    {
        public IEnumerable<Debts> Get()
        {
            var paid = new List<Debts>();
            using (pdInvoiceEntities db = new pdInvoiceEntities())
            {
                var m = Enumerable.Range(1, 12);
              //  var months = db.Nairas.Select(c => c.Month).Distinct().AsEnumerable();
                var year = DateTime.Now.Year;
                var data = db.Dollars.Where(c => c.DateSubmitted.Value.Year == year).ToList();
                foreach (var b in m)
                {
                    var ninflow = data.Where(c => c.DateSubmitted.Value.Month == b).Sum(c => c.AmountPaid);
                    var outflow = data.Where(c => c.DateSubmitted.Value.Month == b).Sum(c => c.Outstanding);
                    var p = new Debts
                    {
                        Month = CultureInfo.CurrentCulture.DateTimeFormat.GetMonthName(b),
                        Paid = Convert.ToDecimal(ninflow),
                        Unpaid = outflow
                    };
                    paid.Add(p);
                  
                }

                return paid;
            }

        }
    }

    public class DebtDollarClientController : ApiController
    {
        public IEnumerable<Debts> Get(string yr, string client)
        {
            var paid = new List<Debts>();
            using (pdInvoiceEntities db = new pdInvoiceEntities())
            {
                var m = Enumerable.Range(1, 12);
                //  var months = db.Nairas.Select(c => c.Month).Distinct().AsEnumerable();
                var year = DateTime.Now.Year;
                var queryYear = int.Parse(yr);
                var data = db.Dollars.Where(c => c.DateSubmitted.Value.Year == queryYear && c.ClientName.Equals(client)).ToList();
                foreach (var b in m)
                {
                    var ninflow = data.Where(c => c.DateSubmitted.Value.Month == b).Sum(c => c.AmountPaid);
                    var outflow = data.Where(c => c.DateSubmitted.Value.Month == b).Sum(c => c.Outstanding);
                    var p = new Debts
                    {
                        Month = CultureInfo.CurrentCulture.DateTimeFormat.GetMonthName(b),
                        Paid = Convert.ToDecimal(ninflow),
                        Unpaid = outflow
                    };
                    paid.Add(p);

                }

                return paid;
            }

        }

    }
    public class DebtNairaClientController : ApiController
    {
        public IEnumerable<Debts> Get(string yr, string client)
        {
            var paid = new List<Debts>();
            using (pdInvoiceEntities db = new pdInvoiceEntities())
            {
                var m = Enumerable.Range(1, 12);
                var months = db.Nairas.Select(c => c.DateSubmitted.Value.Month).Distinct().AsEnumerable();
                var year = DateTime.Now.Year;
                var queryYear = int.Parse(yr);
                var data = db.Nairas.Where(c => c.DateSubmitted.Value.Year == queryYear && c.ClientName.Equals(client)).ToList();
                foreach (var b in m)
                {
                    var ninflow = data.Where(c => c.DateSubmitted.Value.Month == b).Sum(c => c.AmountPaid);
                    var outflow = data.Where(c => c.DateSubmitted.Value.Month == b).Sum(c => c.Outstanding);
                    var p = new Debts
                    {
                        //  Dtype = "Paid",
                        //Flow = Convert.ToDecimal(ninflow),
                        Month = CultureInfo.CurrentCulture.DateTimeFormat.GetMonthName(b),
                        Paid = Convert.ToDecimal(ninflow),
                        Unpaid = outflow
                    };
                    paid.Add(p);

                }

                return paid;
            }

        }
    }
    public class Debts
    {
        //public string Dtype { get; set; }
        public decimal Paid { get; set; }
        public string Month { get; set; }
        public decimal? Unpaid { get; set; }
    }

    public class Activiti
    {
        public int Paid { get; set; }
        public int Unpaid { get; set; }
    }
    public class Creditors
    {
        public string ClientName { get; set; }
        public decimal TotalPaid { get; set; }
    }

    public class Debtors
    {
        public string ClientName { get; set; }
        public decimal TotalPaid { get; set; }
    }
}