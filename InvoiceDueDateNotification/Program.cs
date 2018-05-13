using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Mail;
using System.Text;
using System.Threading.Tasks;

namespace InvoiceDueDateNotification
{
    class Program
    {
        static void Main(string[] args)
        {
            using (pdInvoiceEntities db = new pdInvoiceEntities())
            {
                var invoicesdate = db.Nairas.Select(c =>
                    new InvoiceDate {ClientName = c.ClientName,InvoiceNum = c.InvoiceNumber,SeNumber = c.SENumber,DateSubmitted = c.DateSubmitted,DueDate = c.DueDate});
                foreach (var invoices in invoicesdate)
                {
                    SendMail(invoices.ClientName,invoices.InvoiceNum,invoices.SeNumber,invoices.DateSubmitted,invoices.DueDate);
                }
            }
        }

        public static void SendMail(string clientname, string invoiceno, string senum, DateTime datesubmit, DateTime duedate)
        {
            var title = $"{clientname} Invoice Due Notification";
           var start = datesubmit;
            var end = duedate;
            var servertime = DateTime.Today;
            var countdowndate = end.AddDays(-2);
          //  if()
            if (servertime == countdowndate)
            {
                var message = $"Your Invoice Due Notification {invoiceno}  with Service Entry no: {senum} expires in 3 days";
                using (pdInvoiceEntities db = new pdInvoiceEntities())
                {
                    List<string> staffsemail = db.Staffs.Select(c => c.Email).ToList();
                    SendEmailNotif(title, message, staffsemail);
                }
            }
            else if (servertime == end)
            {
               var message = $"Your Invoice Due Notification {invoiceno}  with Service Entry no: {senum} expires Today";
                using (pdInvoiceEntities db = new pdInvoiceEntities())
                {
                    List<string> staffsemail = db.Staffs.Select(c=>c.Email).ToList();
                    SendEmailNotif(title, message, staffsemail);
                }
            }
          
        }

        private static void SendEmailNotif( string title, string message,
             List<string> staffsemail)
        {
           
            MailMessage mail = new MailMessage();
            SmtpClient smtpServer = new SmtpClient("smtp.gmail.com");
            mail.From = new MailAddress("debtorscontrol@petrodata.net");
            foreach (var staff in staffsemail)
            {
                mail.CC.Add($"{staff}");
            }
            mail.Subject = $"{title}";
            mail.Body = $"{message}";
            smtpServer.Port = 587;
            smtpServer.Credentials =
                new System.Net.NetworkCredential("abiola.akinnubi@gmail.com", "yemisi1992");
            smtpServer.EnableSsl = true;
            smtpServer.Send(mail);
        }
    }

    class InvoiceDate
    {
        public string ClientName { get; set; }
        public string InvoiceNum { get; set; }
        public string SeNumber { get; set; }
        public DateTime DateSubmitted { get; set; }
        public DateTime DueDate { get; set; }
    }

}
