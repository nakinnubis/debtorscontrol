using DebtorsControl.Models;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Data.Entity.Migrations;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;
using DebtorsControl.ViewModel;

namespace DebtorsControl.Controllers
{
    public class StaffsController : Controller
    {
        // GET: Staffs
        public ActionResult Index()
        {

            if (Session["AdminId"] != null)
            {
                ViewBag.UserFullName = Session["AdminFullname"].ToString();
                ViewBag.Username = Session["AdminId"].ToString();
                using (pdInvoiceEntities db = new pdInvoiceEntities())
                {
                    var year = DateTime.Now.Year;
                    var data = db.Nairas.Count(c => c.Year == year && c.Outstanding.Equals(0));
                    var datadollar = db.Dollars.Count(c => c.Year == year && c.Outstanding.Equals(0));
                    var unpaid = db.Nairas.Count(c => c.Year == year && !c.Outstanding.Equals(0));
                    var unpaiddollar = db.Dollars.Count(c => c.Year == year && !c.Outstanding.Equals(0));
                    ViewBag.PaidNaira = data;
                    ViewBag.PaidDollar = datadollar;
                    ViewBag.Unpaid = unpaid;
                    ViewBag.UnpaidDollar = unpaiddollar;
                }

                return View();
            }
            return RedirectToAction("Login", "Account");


        }

        /// <summary>
        /// The create client.
        /// </summary>
        /// <returns>
        /// The <see cref="ActionResult"/>.
        /// </returns>
        public ActionResult CreateClient()
        {
            return View();
        }

        public async Task<ActionResult> CreateClient(FormCollection form)
        {
            if (ModelState.IsValid)
            {
                using (pdInvoiceEntities db = new pdInvoiceEntities())
                {
                    var clientname = form["ClientName"];
                    var clientcode = form["ClientCode"];
                    if (await db.Clients.AnyAsync(c => c.ClientName.Equals(clientname)))
                    {
                        ViewBag.Client = "Client name already exist in the database!";
                        return View();
                    }

                    var client = new Client
                    {
                        ClientName = clientname,
                        ClientCode = clientcode

                    };
                    db.Clients.Add(client);
                    await db.SaveChangesAsync().ConfigureAwait(true);
                    ViewBag.Success = "Staff created successfully";
                    return View();
                }
            }
            return View();

        }


        public ActionResult AddInvoice()
        {
            ClientsViewModel clientlist = new ClientsViewModel { GetClients = GetClients() };
            return View(clientlist);
        }

        [HttpPost]
        public async Task<ActionResult> AddInvoice(FormCollection form)
        {
            if (ModelState.IsValid)
            {
                using (pdInvoiceEntities db = new pdInvoiceEntities())
                {
                    if (form["invoicenumber"] != null)
                    {
                        var invoice = new Invoice
                        {
                            InvoiceNumber = form["invoicenumber"],
                            ClientName = form["GetClients"]
                        };

                        var dated = DateTime.UtcNow;
                        var span = dated.TimeOfDay;
                        var activitybyStaff = $"added or Updated the Invoice Number {invoice.InvoiceNumber}  @ {dated} and timespan {span}";
                        var activityLog = new ActivityLog
                        {
                            Staffinfo = this.Session["AdminFullname"].ToString(),
                            TypeofActivity = activitybyStaff
                        };

                        try
                        {
                            db.Invoices.AddOrUpdate(invoice);
                            db.ActivityLogs.Add(activityLog);
                            await db.SaveChangesAsync().ConfigureAwait(true);
                            return RedirectToAction("AddInvoice");
                        }
                        catch (Exception)
                        {
                            ViewBag.ErrorMessage = "Invoice can not be added or already exist duplicate entry not allowed";
                            return View();
                        }

                    }

                    ViewBag.Empty = "Invoice field is empty";
                    return View();
                }
            }

            ViewBag.Error = "Invalid submission";
            return View();


        }
        //[HttpGet]
        [HttpGet]
        public ActionResult EditInvoice()
        {
            var editinvoiceViewmModel = new InvoiceViewModel
            {
                GetClients = GetClients(),
                YearList = YearList()
            };
            return View(editinvoiceViewmModel);
        }

        [HttpPost]
        public ViewResult EditInvoice(FormCollection form)
        {
            var clientname = form["GetClients"];
            var year = form["YearList"];
            Session["YearList"] = year;
            Session["GetClients"] = clientname;
            using (pdInvoiceEntities db = new pdInvoiceEntities())
            {
                var _year = int.Parse(year);
                var editinvoiceViewmModel = new InvoiceViewModel
                {
                    NairaInvoices = GetNairaInvoices(db, _year, clientname),
                    DollarInvoices = GetDollarInvoices(db, _year, clientname),
                    GetClients = GetClients(),
                    YearList = YearList()
                };
                return View(editinvoiceViewmModel);
            }
        }

        public async Task<ActionResult> Edit(string invoicenum, string servicenum)
        {
            using (pdInvoiceEntities db = new pdInvoiceEntities())
            {
                var data = await db.Dollars.Where(c => c.InvoiceNumber.Equals(invoicenum) && c.SENumber.Equals(servicenum))
                    .Select(c =>
                        new DollarInvoice
                        {
                            ClientName = c.ClientName,
                            Year = c.Year,
                            InvoiceNumber = c.InvoiceNumber,
                            SeNumber = c.SENumber,
                            Amount = c.Amount,
                            AmountPaid = c.AmountPaid,
                            Vat = c.VAT,
                            TotalInvoice = c.TotalInvoice,
                            Payable = c.Payable,
                            LcdCharge = c.LcdCharge,
                            Outstanding = c.Outstanding,
                            DateSubmitted = c.DateSubmitted,
                            DueDate = c.DueDate,
                            WithHoldinTax = c.WithHoldinTax,
                            Comments = c.Comments
                        }).FirstOrDefaultAsync();
                return View(data);
            }
        }

        public async Task<ActionResult> EditNaira(string invoicenum, string servicenum)
        {
            using (pdInvoiceEntities db = new pdInvoiceEntities())
            {
                var data = await db.Nairas.Where(c => c.InvoiceNumber.Equals(invoicenum) && c.SENumber.Equals(servicenum))
                    .Select(c =>
                        new NairaInvoice
                        {
                            ClientName = c.ClientName,
                            Year = c.Year,
                            InvoiceNumber = c.InvoiceNumber,
                            SeNumber = c.SENumber,
                            Amount = c.Amount,
                            AmountPaid = c.AmountPaid,
                            Vat = c.VAT,
                            FxRate = c.FxRate,
                            TotalInvoice = c.TotalInvoice,
                            Payable = c.Payable,
                            LcdCharge = c.LcdCharge,
                            Outstanding = c.Outstanding,
                            DateSubmitted = c.DateSubmitted,
                            DueDate = c.DueDate,
                            WithHoldingTax = c.WithHoldingTax,
                            Comments = c.Comments
                        }).FirstOrDefaultAsync();
                return View(data);
            }
        }
        [HttpPost]
        public async Task<ActionResult> Edit(FormCollection form)
        {
            if (ModelState.IsValid)
            {
                using (pdInvoiceEntities db = new pdInvoiceEntities())
                {
                    var clientname = form["ClientName"];
                    var invoicenum = form["InvoiceNumber"];
                    var senum = form["SeNumber"];
                    if (await db.Dollars.AnyAsync(c =>
                        c.ClientName == clientname && c.InvoiceNumber == invoicenum && c.SENumber == senum))
                    {
                        var dataupdate = db.Dollars
                            .FirstOrDefault(c => c.InvoiceNumber.Equals(invoicenum) && c.SENumber.Equals(senum));
                        if (dataupdate != null)
                        {
                            var activitybyStaff = $"Updated the amountpaid for dollar part of Invoice Number {dataupdate.InvoiceNumber} with Services Entry Number {dataupdate.SENumber}, The amount paid is {decimal.Parse(form["AmountPaid"])}";
                            var activityLog = new ActivityLog
                            {
                                Staffinfo = this.Session["AdminFullname"].ToString(),
                                TypeofActivity = activitybyStaff
                            };
                            db.ActivityLogs.AddOrUpdate(activityLog);
                            dataupdate.AmountPaid = decimal.Parse(form["AmountPaid"]);
                            dataupdate.Outstanding = decimal.Parse(form["Outstanding"]);
                            await db.SaveChangesAsync();
                            ViewBag.Success = "Paid amount successfully updated for ";
                            return RedirectToAction("EditInvoice", new { invoicenum, senum });
                        }
                        ViewBag.Error = "The Invoice doesn't exist";
                        return RedirectToAction("EditInvoice", new { invoicenum, senum });
                    }
                    ViewBag.Error = "The submitted entry is invalid";
                    return RedirectToAction("EditInvoice", new { invoicenum, senum });
                }

            }
            return RedirectToAction("EditInvoice");
        }

        [HttpPost]
        public async Task<ActionResult> EditNaira(FormCollection form)
        {
            if (ModelState.IsValid)
            {
                using (pdInvoiceEntities db = new pdInvoiceEntities())
                {
                    var clientname = form["ClientName"];
                    var invoicenum = form["InvoiceNumber"];
                    var senum = form["SeNumber"];
                    if (await db.Nairas.AnyAsync(c =>
                        c.ClientName == clientname && c.InvoiceNumber == invoicenum && c.SENumber == senum))
                    {
                        var dataupdate = db.Nairas
                            .FirstOrDefault(c => c.InvoiceNumber.Equals(invoicenum) && c.SENumber.Equals(senum));
                        if (dataupdate != null)
                        {
                            var dated = DateTime.UtcNow;
                            var span = dated.TimeOfDay;
                            var activitybyStaff = $"Updated the amount paid for Naira part of Invoice Number {dataupdate.InvoiceNumber} with Services Entry Number {dataupdate.SENumber}, The amount paid is {decimal.Parse(form["AmountPaid"])}  @ {dated} and timespan {span}";
                            var activityLog = new ActivityLog
                            {
                                Staffinfo = Session["AdminFullname"].ToString(),
                                TypeofActivity = activitybyStaff
                            };
                            db.ActivityLogs.AddOrUpdate(activityLog);
                            dataupdate.AmountPaid = decimal.Parse(form["AmountPaid"]);
                            dataupdate.Outstanding = decimal.Parse(form["Outstanding"]);
                            await db.SaveChangesAsync();
                            ViewBag.Success = "Paid amount successfully updated for ";
                            return RedirectToAction("EditInvoice", new { invoicenum, senum });
                        }
                        ViewBag.Error = "The Invoice doesn't exist";
                        return RedirectToAction("EditInvoice", new { invoicenum, senum });
                    }
                    ViewBag.Error = "The submitted entry is invalid";
                    return RedirectToAction("EditInvoice", new { invoicenum, senum });
                }

            }
            return RedirectToAction("EditInvoice");
        }


        [HttpGet]
        public ActionResult MonthlyAnalysis()
        {
            var monthlyAnalysisViewmModel = new InvoiceViewModel
            {
                GetClients = GetClients(),
                YearList = YearList()
            };
            return View(monthlyAnalysisViewmModel);
        }
        [HttpPost]
        public ViewResult MonthlyAnalysis(FormCollection form)
        {
            var clientname = form["GetClients"];
            var year = form["YearList"];
            Session["YearList"] = year;
            Session["GetClients"] = clientname;
            using (pdInvoiceEntities db = new pdInvoiceEntities())
            {
                var _year = int.Parse(year);
                var monthlyAnalysisViewmModel = new InvoiceViewModel
                {
                    NairaInvoices = GetNairaInvoices(db, _year, clientname),
                    DollarInvoices = GetDollarInvoices(db, _year, clientname),
                    GetClients = GetClients(),
                    YearList = YearList()
                };
                return View(monthlyAnalysisViewmModel);
            }

        }

        public ActionResult PostDollarInvoice()
        {
            ClientsViewModel clientlist = new ClientsViewModel { GetClients = GetClients() };
            return View(clientlist);
        }
        [HttpPost]
        [ActionName("PostDollar")]
        public async Task<ActionResult> PostDollar(FormCollection form, HttpPostedFileBase file)
        {
            if (this.ModelState.IsValid)
            {
                try
                {
                    var date = form["year"].Split('-');
                    var year = date[0];
                    var month = date[1];
                    var clientName = form["GetClients"];
                    var invoicenum = form["invoicenumber"];
                    var servicesEntry = form["servicenumber"];
                    // var fxRate = form["FxRate"];
                    var amount = form["Amount"];
                    var vat = form["Vat"];
                    var totalinvoice = form["TotalInvoice"];
                    var payable = form["Payable"];
                    var lcdcharge = form["LcdCharge"];
                    var amountpaid = form["AmountPaid"];
                    var outstanding = form["Outstanding"];
                    var datesubmitted = form["DateSubmitted"];
                    var duedate = form["DueDate"];
                    var withold = form["WithHoldingTax"];
                    var comments = form["comments"];
                    var remittance = file;
                    var httpServerUtilityBase = this.Server;
                    if (httpServerUtilityBase != null)
                    {
                        var baspath = AppDomain.CurrentDomain.BaseDirectory;
                        var folder = Server.MapPath("../RemittanceUpload/");
                        var pathtoremittance = Path.Combine(folder, remittance.FileName);
                        if (remittance.ContentLength != 0)
                        {
                            if (remittance.FileName != null)
                            {
                                if (!remittance.FileName.EndsWith(".pdf"))
                                {
                                    if (remittance.FileName.EndsWith(".png") || remittance.FileName.EndsWith(".jpg")
                                                                             || remittance.FileName.EndsWith(".jpeg"))
                                    {
                                        remittance.SaveAs(pathtoremittance);
                                    }
                                }
                                else
                                {
                                    folder = httpServerUtilityBase.MapPath("~/RemittanceUpload/");
                                    remittance.SaveAs(Path.Combine(folder, remittance.FileName));
                                }
                            }
                        }
                        ////  System.IO.File.Move(pathtoremittance,$"{}");

                        using (pdInvoiceEntities db = new pdInvoiceEntities())
                        {
                            var invoice = new Dollar
                            {
                                ClientName = clientName,
                                Amount = decimal.Parse(amount),
                                AmountPaid = decimal.Parse(amountpaid),
                                Year = int.Parse(year),
                                Month = int.Parse(month),
                                InvoiceNumber = invoicenum,
                                SENumber = servicesEntry,
                                VAT = decimal.Parse(vat),
                                TotalInvoice = decimal.Parse(totalinvoice),
                                Payable = decimal.Parse(payable),
                                LcdCharge = decimal.Parse(lcdcharge),
                                Outstanding = decimal.Parse(outstanding),
                                WithHoldinTax = decimal.Parse(withold),
                                DateSubmitted = DateTime.Parse(datesubmitted),
                                DueDate = DateTime.Parse(duedate),
                                RemittanceAdvise = pathtoremittance,
                                Comments = comments
                            };
                            var activitybyStaff = $"Posted or Updated the Naira Invoice Number {invoice.InvoiceNumber} with Services Entry Number {invoice.SENumber}";
                            var activityLog = new ActivityLog
                            {
                                Staffinfo = this.Session["AdminFullname"].ToString(),
                                TypeofActivity = activitybyStaff
                            };
                            db.Dollars.AddOrUpdate(invoice);
                            db.ActivityLogs.AddOrUpdate(activityLog);
                            await db.SaveChangesAsync().ConfigureAwait(true);
                            return RedirectToAction("PostDollarInvoice");
                        }
                    }
                }
                catch (Exception e)
                {
                    this.ViewBag.Error = $"An error occured, this message could help debug:  {e.Message}";
                    return RedirectToAction("PostDollarInvoice");
                }
            }
            return RedirectToAction("PostDollarInvoice");
        }


        public ActionResult PostNairaInvoice()
        {
            //ClientsViewModel clientlist = new ClientsViewModel { GetServiceEntries = GetEntries() };
            ClientsViewModel clientlist = new ClientsViewModel { GetClients = GetClients() };
            return View(clientlist);
        }

        /// <summary>
        /// The post naira invoice.
        /// </summary>
        /// <param name="form">
        /// The form.
        /// </param>
        /// <param name="file">
        /// The file.
        /// </param>
        /// <returns>
        /// The <see cref="Task"/>.
        /// </returns>r
        [HttpPost]
        [ActionName("PostNaira")]
        public async Task<ActionResult> PostNaira(FormCollection form, HttpPostedFileBase file)
        {
            if (this.ModelState.IsValid)
            {
                try
                {
                    var date = form["year"].Split('-');
                    var year = date[0];
                    var month = date[2];
                    var clientName = form["GetClients"];
                    var invoicenum = form["invoicenumber"];
                    var servicesEntry = form["servicenumber"];
                    var fxRate = form["FxRate"];
                    var amount = form["Amount"];
                    var vat = form["Vat"];
                    var totalinvoice = form["TotalInvoice"];
                    var payable = form["Payable"];
                    var lcdcharge = form["LcdCharge"];
                    var amountpaid = form["AmountPaid"];
                    var outstanding = form["Outstanding"];
                    var datesubmitted = form["DateSubmitted"];
                    var duedate = form["DueDate"];
                    var comments = form["comments"];
                    var withold = form["WithHoldingTax"];
                    var remittance = file;
                    var httpServerUtilityBase = this.Server;
                    if (httpServerUtilityBase != null)
                    {
                        var folder = httpServerUtilityBase.MapPath("~/RemittanceUpload/");
                        var pathtoremittance = Path.Combine(folder, remittance.FileName);
                        if (remittance.ContentLength != 0)
                        {
                            if (remittance.FileName != null)
                            {
                                if (!remittance.FileName.EndsWith(".pdf"))
                                {
                                    if (remittance.FileName.EndsWith(".png") || remittance.FileName.EndsWith(".jpg")
                                                                             || remittance.FileName.EndsWith(".jpeg"))
                                    {
                                        remittance.SaveAs(pathtoremittance);
                                    }
                                }
                                else
                                {
                                    folder = httpServerUtilityBase.MapPath("~/RemittanceUpload/");
                                    remittance.SaveAs(Path.Combine(folder, remittance.FileName));
                                }
                            }
                        }
                        ////  System.IO.File.Move(pathtoremittance,$"{}");

                        using (pdInvoiceEntities db = new pdInvoiceEntities())
                        {
                            var invoice = new Naira
                            {
                                ClientName = clientName,
                                Amount = decimal.Parse(amount),
                                AmountPaid = decimal.Parse(amountpaid),
                                Year = int.Parse(year),
                                Month = int.Parse(month),
                                InvoiceNumber = invoicenum,
                                SENumber = servicesEntry,
                                FxRate = decimal.Parse(fxRate),
                                VAT = decimal.Parse(vat),
                                TotalInvoice = decimal.Parse(totalinvoice),
                                Payable = decimal.Parse(payable),
                                LcdCharge = decimal.Parse(lcdcharge),
                                Outstanding = decimal.Parse(outstanding),
                                WithHoldingTax = decimal.Parse(withold),
                                DateSubmitted = DateTime.Parse(datesubmitted),
                                DueDate = DateTime.Parse(duedate),
                                RemittanceAdvise = pathtoremittance,
                                Comments = comments
                            }; var dated = DateTime.UtcNow;
                            var span = dated.TimeOfDay;
                            var activitybyStaff = $"Posted or Updated the Naira Invoice Number {invoice.InvoiceNumber} with Services Entry Number {invoice.SENumber}  @ {dated} and timespan {span}";
                            var activityLog = new ActivityLog
                            {
                                Staffinfo = this.Session["AdminFullname"].ToString(),
                                TypeofActivity = activitybyStaff
                            };
                            db.Nairas.AddOrUpdate(invoice);
                            db.ActivityLogs.AddOrUpdate(activityLog);
                            await db.SaveChangesAsync().ConfigureAwait(true);
                            return RedirectToAction("PostNairaInvoice");
                        }
                    }
                }
                catch (Exception e)
                {
                    this.ViewBag.Error = $"An error occured, this message could help debug:  {e.Message}";
                    return RedirectToAction("PostNairaInvoice");
                }
            }

            return RedirectToAction("PostNairaInvoice");
        }

        /// <summary>
        /// The remittance advise.
        /// </summary>
        /// <returns>
        /// The <see cref="ActionResult"/>.
        /// </returns>
        public ActionResult RemittanceAdvise()
        {
            var listinvoicetype = new List<string> { "Naira", "Dollar" };
            var remittancevm = new RemittanceViewModel { DoRemittance = GetClients(), InvoiceTypes = listinvoicetype.Select(c => new InvoiceType { InvoicesType = c }) };
            return View(remittancevm);
        }
        [HttpPost]
        public async Task<ActionResult> RemittanceAdvise(FormCollection form, HttpPostedFileBase file)
        {
            if (ModelState.IsValid)
            {
                var clientname = form["DoRemittance"];
                var invoicenum = form["InvoiceNumber"];
                var serventry = form["ServiceNumber"];
                var amountpaid = form["AmountPaid"];
                var invoicetype = form["InvoiceTypes"];
                var comment = form["Comments"];
                var amountlcd = Getlcd(clientname, invoicenum, serventry, invoicetype);
                var actual = decimal.Parse(amountpaid);
                var lcd = amountlcd[0];
                var payable = amountlcd[2];
                var outstanding = payable - actual - lcd;
                var remittance = file;
                var httpServerUtilityBase = this.Server;
                if (httpServerUtilityBase != null)
                {
                    var baspath = AppDomain.CurrentDomain.BaseDirectory;
                    var folder = Server.MapPath("../RemittanceUpload/");
                    var pathtoremittance = Path.Combine(folder, remittance.FileName);
                    using (pdInvoiceEntities db = new pdInvoiceEntities())
                    {
                        if (invoicetype == "Naira")
                        {
                            SaveRemittance(remittance, httpServerUtilityBase, folder, pathtoremittance);
                            var naira = db.Nairas.FirstOrDefault(c =>
                                c.InvoiceNumber.Equals(invoicenum) && c.SENumber.Equals(serventry) &&
                                c.ClientName.Equals(clientname));
                            naira.Outstanding = outstanding;
                            naira.Comments = comment;
                            naira.RemittanceAdvise = pathtoremittance;
                            await db.SaveChangesAsync().ConfigureAwait(true);
                            ViewBag.Success = "Naira table succesffuly updated";
                            return RedirectToAction("RemittanceAdvise");
                        }

                        if (invoicetype == "Dollar")
                        {
                            SaveRemittance(remittance, httpServerUtilityBase, folder, pathtoremittance);
                            var naira = db.Dollars.FirstOrDefault(c =>
                                c.InvoiceNumber.Equals(invoicenum) && c.SENumber.Equals(serventry) &&
                                c.ClientName.Equals(clientname));
                            naira.Outstanding = outstanding;
                            naira.Comments = comment;
                            naira.RemittanceAdvise = pathtoremittance;
                            await db.SaveChangesAsync().ConfigureAwait(true);
                            ViewBag.Success = "Dollar table succesffuly updated";
                            return RedirectToAction("RemittanceAdvise");
                        }
                    }
                }
            }

            ViewBag.Error = "An error occured, check your inputs";
            return RedirectToAction("RemittanceAdvise");
        }

        private void SaveRemittance(HttpPostedFileBase remittance, HttpServerUtilityBase httpServerUtilityBase, string folder, string pathtoremittance)
        {

            if (remittance.ContentLength != 0)
            {
                if (remittance.FileName != null)
                {
                    if (!remittance.FileName.EndsWith(".pdf"))
                    {
                        if (remittance.FileName.EndsWith(".png") || remittance.FileName.EndsWith(".jpg")
                                                                 || remittance.FileName.EndsWith(".jpeg"))
                        {
                            remittance.SaveAs(pathtoremittance);
                        }
                    }
                    else
                    {
                        folder = httpServerUtilityBase.MapPath("~/RemittanceUpload/");
                        remittance.SaveAs(Path.Combine(folder, remittance.FileName));
                    }
                }
            }
        }

        public decimal[] Getlcd(string clientname, string invoicenum, string servicenum, string invoicetype)
        {
            using (pdInvoiceEntities db = new pdInvoiceEntities())
            {
                if (invoicetype == "Naira")
                {
                    var naira = db.Nairas.FirstOrDefault(c => c.ClientName.Equals(clientname) && c.InvoiceNumber.Equals(invoicenum) &&
                        c.SENumber.Equals(servicenum));
                    decimal[] result = new decimal[3];
                    if (naira != null)
                        result[0] = naira.LcdCharge;
                    result[1] = naira.Amount;
                    result[2] = naira.Payable;
                    return result;
                }
                else
                {
                    var dollar = db.Dollars.FirstOrDefault(c => c.ClientName.Equals(clientname) && c.InvoiceNumber.Equals(invoicenum) &&
                                                              c.SENumber.Equals(servicenum));
                    decimal[] result = new decimal[3];
                    if (dollar != null)
                        result[0] = dollar.LcdCharge;
                    result[1] = dollar.Amount;
                    result[2] = dollar.Payable;
                    return result;
                }
            }
        }


        public List<Remittance> DoRemittance()
        {
            using (pdInvoiceEntities db = new pdInvoiceEntities())
            {
                var remittancevm = db.ServiceEnteries.GroupBy(c => new { c.ClientName, c.InvoiceNumber, c.SENumber }).Select(c => new Remittance
                {
                    ClientName = c.Key.ClientName,
                    InvoiceNumber = c.Key.InvoiceNumber,
                    ServiceEntry = c.Key.SENumber
                }).ToList();
                return remittancevm;
            }
        }

        /// <summary>
        /// The service entry.
        /// </summary>
        /// <returns>
        /// The <see cref="ActionResult"/>.
        /// </returns>
        public ActionResult ServiceEntry()
        {
            ClientsViewModel clientlist = new ClientsViewModel { GetClients = GetClients() };
            return View(clientlist);
        }

        /// <summary>
        /// The service entry.
        /// </summary>
        /// <param name="form">
        /// The form.
        /// </param>
        /// <returns>
        /// The <see cref="Task"/>.
        /// </returns>
        [HttpPost]
        public async Task<ActionResult> ServiceEntry(FormCollection form)
        {
            if (this.ModelState.IsValid)
            {
                using (pdInvoiceEntities db = new pdInvoiceEntities())
                {
                    if (string.Equals(form["invoicenumber"], null, StringComparison.Ordinal) || form["servicenumber"] == null)
                    {
                        ViewBag.Empty = "Invoice field is empty";
                        return RedirectToAction("ServiceEntry");
                    }

                    var service = new ServiceEntery
                    {
                        InvoiceNumber = form["invoicenumber"],
                        SENumber = form["servicenumber"],
                        ClientName = form["GetClients"]
                    };
                    var dated = DateTime.UtcNow;
                    var span = dated.TimeOfDay;
                    var activitybyStaff = $"added or Updated the Invoice {service.InvoiceNumber} Service Entry Number {service.SENumber}  @ {dated} and timespan {span}";
                    var activityLog = new ActivityLog
                    {
                        Staffinfo = Session["AdminFullname"].ToString(),
                        TypeofActivity = activitybyStaff
                    };

                    try
                    {
                        db.ServiceEnteries.AddOrUpdate(service);
                        db.ActivityLogs.Add(activityLog);
                        await db.SaveChangesAsync().ConfigureAwait(true);
                        return RedirectToAction("ServiceEntry");
                    }
                    catch (Exception)
                    {
                        ViewBag.ErrorMessage = "Invoice can not be added or already exist duplicate entry not allowed";
                        return RedirectToAction("ServiceEntry");
                    }
                }

            }
            return RedirectToAction("ServiceEntry");
        }
        public List<Clients> GetClients()
        {
            using (pdInvoiceEntities db = new pdInvoiceEntities())
            {
                var clientslist = db.Clients.ToList();
                var result = new List<Clients>();
                foreach (var client in clientslist)
                {
                    var clients = new Clients
                    {
                        ClientCode = client.ClientCode,
                        ClientName = client.ClientName
                    };
                    result.Add(clients);
                }
                return result;
            }
        }

        public List<NairaInvoice> GetNairaInvoices(pdInvoiceEntities db, int year, string client)
        {
            var naira = db.Nairas.Where(c => c.Year.Equals(year) && c.ClientName.Equals(client)).OrderBy(c => c.Month).ToList();
            var nairainvoice = new List<NairaInvoice>();
            foreach (var n in naira)
            {
                var nairapart = new NairaInvoice
                {
                    Date = $"{n.Month}/{n.Year}",
                    InvoiceNumber = n.InvoiceNumber,
                    SeNumber = n.SENumber,
                    FxRate = n.FxRate,
                    Amount = n.Amount,
                    Vat = n.VAT,
                    TotalInvoice = n.TotalInvoice,
                    Payable = n.Payable,
                    AmountPaid = n.AmountPaid,
                    LcdCharge = n.LcdCharge,
                    Outstanding = n.Outstanding,
                    DateSubmitted = n.DateSubmitted,
                    DueDate = n.DueDate
                };
                nairainvoice.Add(nairapart);
            }

            return nairainvoice;
        }
        public List<DollarInvoice> GetDollarInvoices(pdInvoiceEntities db, int year, string client)
        {
            var dollar = db.Dollars.Where(c => c.Year.Equals(year) && c.ClientName.Equals(client)).OrderBy(c => c.Month).ToList();
            var dollarinvoice = new List<DollarInvoice>();
            foreach (var n in dollar)
            {
                var nairapart = new DollarInvoice
                {
                    Date = $"{n.Month}/{n.Year}",
                    InvoiceNumber = n.InvoiceNumber,
                    SeNumber = n.SENumber,
                    Amount = n.Amount,
                    Vat = n.VAT,
                    TotalInvoice = n.TotalInvoice,
                    Payable = n.Payable,
                    AmountPaid = n.AmountPaid,
                    LcdCharge = n.LcdCharge,
                    Outstanding = n.Outstanding,
                    DateSubmitted = n.DateSubmitted,
                    DueDate = n.DueDate
                };
                dollarinvoice.Add(nairapart);
            }

            return dollarinvoice;
        }

        public List<Year> YearList()
        {
            var years = new List<Year>();
            var startyr = DateTime.Now.Year - 10;
            var endyr = DateTime.Now.Year + 1;
            var year = Enumerable.Range(startyr, endyr - startyr).Select(c => new Year { Years = c }).ToList();
            return year;
        }
    }
}