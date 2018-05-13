using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace InvoiceDueDateNotification
{
    interface ISendMailOnDueDate
    {
        void SendMail(string clientname, string invoiceno, string senum, DateTime datesubmit, DateTime duedate);
    }
}
