using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.Entity;
using System.Data.Entity.Migrations;
using System.Data.OleDb;
using System.Data.SqlClient;
using System.Globalization;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;
using DebtorsControl.Filters;
using DebtorsControl.Models;
using DebtorsControl.ViewModel;
using System.IO;
using System.Web.UI;
using System.Web.UI.WebControls;
using DebtorsControl.Classes;
using LinqToExcel;
using OfficeOpenXml;
using OfficeOpenXml.FormulaParsing.Excel.Functions.DateTime;
using Rotativa.Options;
using Year = DebtorsControl.ViewModel.Year;

namespace DebtorsControl.Controllers
{
    [AdminAuthFilter]
    public class DebtorsController : Controller
    {
        // GET: Debtors

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
                    Session["PaidNaira"] = data;
                    Session["PaidDollar"] = datadollar;
                    Session["Unpaid"] = unpaid;
                    Session["UnpaidDollar"] = unpaiddollar;
                    ViewBag.PaidNaira = Session["PaidNaira"];
                    ViewBag.PaidDollar = Session["PaidDollar"];
                    ViewBag.Unpaid = Session["Unpaid"];
                    ViewBag.UnpaidDollar = Session["UnpaidDollar"];
                }
                var datavm = new InvoiceViewModel
                {
                    GetClients = GetClients(),
                    YearList = YearList()
                };
                return View(datavm);
            }
            return RedirectToAction("Login", "Account");


        }

        public ActionResult AddInvoice()
        {
            ViewBag.UserFullName = Session["AdminFullname"].ToString();
            ViewBag.Username = Session["AdminId"].ToString();
            ClientsViewModel clientlist = new ClientsViewModel { GetClients = GetClients() };
            ViewBag.ErrorMessage = TempData["ErrorMessage"];
            ViewBag.Empty = TempData["Empty"];
            ViewBag.Error = TempData["Error"];
            ViewBag.Success = TempData["Success"];
            ViewBag.PaidNaira = Session["PaidNaira"];
            ViewBag.PaidDollar = Session["PaidDollar"];
            ViewBag.Unpaid = Session["Unpaid"];
            ViewBag.UnpaidDollar = Session["UnpaidDollar"];
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
                        var invoicenum = form["invoicenumber"];
                        var clientname = form["GetClients"];
                        var invoice = new Invoice
                        {
                            InvoiceNumber = invoicenum,
                            ClientName = clientname
                        };
                        var dated = DateTime.UtcNow;
                        var span = dated.TimeOfDay;
                        var activitybyStaff = $"added or Updated the Invoice Number {invoice.InvoiceNumber}";
                        var activityLog = new ActivityLog
                        {
                            Staffinfo = this.Session["AdminFullname"].ToString(),
                            TypeofActivity = activitybyStaff,
                            TimeCreated = $"{dated} , updated : {span}"
                        };

                        try
                        {
                            db.Invoices.Add(invoice);
                            db.ActivityLogs.Add(activityLog);
                            await db.SaveChangesAsync().ConfigureAwait(true);
                            TempData["Success"] = $"Invoice Number created successfully for {clientname}";
                            return RedirectToAction("AddInvoice");
                        }
                        catch (Exception e)
                        {
                            TempData["ErrorMessage"] = $"Invoice can not be added or already exist duplicate entry not allowed  with Message {e.Message}";
                            return RedirectToAction("AddInvoice");
                        }

                    }

                    TempData["Empty"] = "Invoice field is empty";
                    return RedirectToAction("AddInvoice");
                }
            }

            TempData["Error"] = "Invalid submission";
            return RedirectToAction("AddInvoice");

        }

        /// <summary>
        /// The service entry.
        /// </summary>
        /// <returns>
        /// The <see cref="ActionResult"/>.
        /// </returns>
        public ActionResult ServiceEntry()
        {
            ViewBag.UserFullName = Session["AdminFullname"].ToString();
            ViewBag.Username = Session["AdminId"].ToString();
            ClientsViewModel clientlist = new ClientsViewModel { GetClients = GetClients() };
            ViewBag.Empty = TempData["Empty"];
            ViewBag.ServicesSucess = TempData["ServicesSucess"];
            ViewBag.ErrorMessage = TempData["ErrorMessage"];
            ViewBag.PaidNaira = Session["PaidNaira"];
            ViewBag.PaidDollar = Session["PaidDollar"];
            ViewBag.Unpaid = Session["Unpaid"];
            ViewBag.UnpaidDollar = Session["UnpaidDollar"];
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
        public ActionResult ServiceEntry(FormCollection form)
        {
            if (this.ModelState.IsValid)
            {
                using (pdInvoiceEntities db = new pdInvoiceEntities())
                {

                    if (string.Equals(form["invoicenumber"], null, StringComparison.Ordinal))
                    {
                        TempData["Empty"] = "Invoice field is empty";
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
                    var activitybyStaff = $"Uploaded the remittance advice {service.InvoiceNumber} with Services Entry Number {service.SENumber}";
                    var activityLog = new ActivityLog
                    {
                        Staffinfo = Session["AdminFullname"].ToString(),
                        TypeofActivity = activitybyStaff,
                        TimeCreated = $"{dated} , updated : {span}"
                    };

                    try
                    {
                        db.ServiceEnteries.AddOrUpdate(service);
                        db.ActivityLogs.Add(activityLog);
                        db.SaveChanges();
                        TempData["ServicesSucess"] = "Services Entry Sucessfully Created";
                        return RedirectToAction("ServiceEntry");
                    }
                    catch (Exception)
                    {
                        TempData["ErrorMessage"]
                           = "Invoice can not be added or already exist duplicate entry not allowed";
                        return RedirectToAction("ServiceEntry");
                    }
                }

            }
            return RedirectToAction("ServiceEntry");
        }



        //[HttpGet]
        public ActionResult PostNairaInvoice()
        {
            ViewBag.UserFullName = Session["AdminFullname"].ToString();
            ViewBag.Username = Session["AdminId"].ToString();
            //ClientsViewModel clientlist = new ClientsViewModel { GetServiceEntries = GetEntries() };
            ClientsViewModel clientlist = new ClientsViewModel { GetClients = GetClients() };
            ViewBag.Success = TempData["Success"];
            ViewBag.Error = TempData["Error"];
            ViewBag.PreventDuplicate = TempData["InvoiceExist"];
            ViewBag.PaidNaira = Session["PaidNaira"];
            ViewBag.PaidDollar = Session["PaidDollar"];
            ViewBag.Unpaid = Session["Unpaid"];
            ViewBag.UnpaidDollar = Session["UnpaidDollar"];
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
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> PostNaira(FormCollection form)
        {
            if (this.ModelState.IsValid)
            {
                try
                {
                    var seform = form["servicenumber"];
                    if (seform.Equals("Select Service Number"))
                    {
                        string servicesEntry = "";
                        return await SavePostNaira(form, servicesEntry);
                    }
                    else
                    {
                        return await SavePostNaira(form, seform);
                    }

                    //}
                }
                catch (Exception e)
                {
                    TempData["Error"] = $"An error occured, this message could help debug:  {e.Message}";
                    return RedirectToAction("PostNairaInvoice");
                }
            }
            TempData["Error"] = "Form state is invalid refesh the page and try again";
            return RedirectToAction("PostNairaInvoice");
        }

        private async Task<ActionResult> SavePostNaira(FormCollection form, string servicesEntry)
        {
            var date = form["year"].Split('-');
            var year = date[0];
            var month = date[1];
            var clientName = form["GetClients"];
            var invoicenum = form["invoicenumber"];
            // servicesEntry = form["servicenumber"];
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
            var nairaequival = form["NairaEquiv"];
            // var remittance = file;
            // var httpServerUtilityBase = this.Server;
            //if (httpServerUtilityBase != null)
            //   {
            ////  System.IO.File.Move(pathtoremittance,$"{}");

            using (pdInvoiceEntities db = new pdInvoiceEntities())
            {
                if (db.Nairas.Any(c => c.InvoiceNumber.Equals(invoicenum)))
                {
                    TempData["InvoiceExist"] = "Invoice number and services entry already exist in the database";
                    return RedirectToAction("PostNairaInvoice");
                }

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
                    NairaValue = decimal.Parse(nairaequival),
                    // RemittanceAdvise = pathtoremittance,
                    Comments = comments
                };
                var dated = DateTime.UtcNow;
                var span = dated.TimeOfDay;
                var activitybyStaff =
                    $"Posted or Updated the Naira Invoice Number {invoice.InvoiceNumber} with Services Entry Number {invoice.SENumber}";
                var activityLog = new ActivityLog
                {
                    Staffinfo = this.Session["AdminFullname"].ToString(),
                    TypeofActivity = activitybyStaff,
                    TimeCreated = $"{dated} , updated : {span}"
                };
                db.Nairas.AddOrUpdate(invoice);
                db.ActivityLogs.AddOrUpdate(activityLog);
                await db.SaveChangesAsync().ConfigureAwait(true);
                TempData["Success"] = "Naira part of the invoice was successfully created!!!";
                return RedirectToAction("PostNairaInvoice");
            }
        }

        /// <summary>
        /// The post dollar invoice.
        /// </summary>
        /// <returns>
        /// The <see cref="ActionResult"/>.
        /// </returns>
        public ActionResult PostDollarInvoice()
        {
            ViewBag.UserFullName = Session["AdminFullname"].ToString();
            ViewBag.Username = Session["AdminId"].ToString();
            ClientsViewModel clientlist = new ClientsViewModel { GetClients = GetClients() };
            ViewBag.Success = TempData["Success"];
            ViewBag.Error = TempData["Error"];
            ViewBag.PreventDuplicate = TempData["InvoiceExist"];
            ViewBag.PaidNaira = Session["PaidNaira"];
            ViewBag.PaidDollar = Session["PaidDollar"];
            ViewBag.Unpaid = Session["Unpaid"];
            ViewBag.UnpaidDollar = Session["UnpaidDollar"];
            return View(clientlist);
        }
        [HttpPost]
        [ActionName("PostDollar")]
        public async Task<ActionResult> PostDollar(FormCollection form)
        {
            if (this.ModelState.IsValid)
            {
                try
                {
                    var seform = form["servicenumber"];
                    if (seform.Equals("Select Service Number"))
                    {
                        string servicesEntry = "";
                        return await SavePostDollar(form, servicesEntry);
                    }
                    return await SavePostDollar(form, seform);
                }
                catch (Exception e)
                {
                    TempData["Error"] = $"An error occured, this message could help debug:  {e.Message}";
                    return RedirectToAction("PostDollarInvoice");
                }
            }

            TempData["Error"] = "Form state is invalid refesh the page and try again";
            return RedirectToAction("PostDollarInvoice");
        }

        private async Task<ActionResult> SavePostDollar(FormCollection form, string servicesEntry)
        {
            var date = form["year"].Split('-');
            var year = date[0];
            var month = date[1];
            var clientName = form["GetClients"];
            var invoicenum = form["invoicenumber"];
            //servicesEntry = form["servicenumber"];
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
            // var remittance = file;
            //  var httpServerUtilityBase = this.Server;
            // if (httpServerUtilityBase != null)
            // {
            ////  System.IO.File.Move(pathtoremittance,$"{}");
            using (pdInvoiceEntities db = new pdInvoiceEntities())
            {
                if (db.Dollars.Any(c => c.InvoiceNumber.Equals(invoicenum)))
                {
                    TempData["InvoiceExist"] = "Invoice number and services entry already exist in the database";
                    return RedirectToAction("PostDollarInvoice");
                }

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
                    //RemittanceAdvise = pathtoremittance,
                    Comments = comments
                };
                var dated = DateTime.UtcNow;
                var span = dated.TimeOfDay;
                var activitybyStaff =
                    $"Posted or Updated the Naira Invoice Number {invoice.InvoiceNumber} with Services Entry Number {invoice.SENumber}";
                var activityLog = new ActivityLog
                {
                    Staffinfo = this.Session["AdminFullname"].ToString(),
                    TypeofActivity = activitybyStaff,
                    TimeCreated = $"{dated} , updated : {span}"
                };
                db.Dollars.AddOrUpdate(invoice);
                db.ActivityLogs.AddOrUpdate(activityLog);
                await db.SaveChangesAsync().ConfigureAwait(true);
                TempData["Success"] = "Dollar part of the invoice was successfully created!!!";
                return RedirectToAction("PostDollarInvoice");
            }
        }

        /// <summary>
        /// The remittance advise.
        /// </summary>
        /// <returns>
        /// The <see cref="ActionResult"/>.
        /// </returns>
        public ActionResult RemittanceAdvise()
        {
            ViewBag.UserFullName = Session["AdminFullname"].ToString();
            ViewBag.Username = Session["AdminId"].ToString();
            ViewBag.PaidNaira = Session["PaidNaira"];
            ViewBag.PaidDollar = Session["PaidDollar"];
            ViewBag.Unpaid = Session["Unpaid"];
            ViewBag.UnpaidDollar = Session["UnpaidDollar"];
            ViewBag.Error = TempData["Error"];
            ViewBag.Success = TempData["Success"];
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
                var datepaid = form["DatePaid"];
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
                            naira.DatePaid = DateTime.Parse(datepaid);
                            naira.AmountPaid = actual;
                            naira.Outstanding = outstanding;
                            naira.Comments = comment;
                            naira.RemittanceAdvise = pathtoremittance;
                            await db.SaveChangesAsync().ConfigureAwait(true);
                            TempData["Success"] = "Naira table succesffuly updated";
                            return RedirectToAction("RemittanceAdvise");
                        }

                        if (invoicetype == "Dollar")
                        {
                            SaveRemittance(remittance, httpServerUtilityBase, folder, pathtoremittance);
                            var naira = db.Dollars.FirstOrDefault(c =>
                                c.InvoiceNumber.Equals(invoicenum) && c.SENumber.Equals(serventry) &&
                                c.ClientName.Equals(clientname));
                            naira.DatePaid = DateTime.Parse(datepaid);
                            naira.AmountPaid = actual;
                            naira.Outstanding = outstanding;
                            naira.Comments = comment;
                            naira.RemittanceAdvise = pathtoremittance;
                            var dated = DateTime.UtcNow;
                            var span = dated.TimeOfDay;
                            var activitybyStaff = $"Uploaded the remittance advice {naira.InvoiceNumber} with Services Entry Number {naira.SENumber}";
                            var activityLog = new ActivityLog
                            {
                                Staffinfo = this.Session["AdminFullname"].ToString(),
                                TypeofActivity = activitybyStaff,
                                TimeCreated = $"{dated} , updated : {span}"
                            };
                            db.ActivityLogs.AddOrUpdate(activityLog);
                            await db.SaveChangesAsync().ConfigureAwait(true);
                            TempData["Success"] = "Dollar table succesffuly updated";
                            return RedirectToAction("RemittanceAdvise");
                        }
                    }
                }
            }

            TempData["Error"] = "An error occured, check your inputs";
            return RedirectToAction("RemittanceAdvise");
        }

        private void SaveRemittance(HttpPostedFileBase remittance, HttpServerUtilityBase httpServerUtilityBase, string folder, string pathtoremittance)
        {

            if (remittance.ContentLength != 0)
            {
                if (remittance.FileName != null)
                {
                    if (!remittance.FileName.EndsWith(".zip"))
                    {
                        if (remittance.FileName.EndsWith(".png") || remittance.FileName.EndsWith(".jpg")
                                                                 || remittance.FileName.EndsWith(".jpeg") || remittance.FileName.EndsWith(".pdf"))
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
        /// <summary>
        /// The create client.
        /// </summary>
        /// <returns>
        /// The <see cref="ActionResult"/>.
        /// </returns>
        public ActionResult CreateClient()
        {
            ViewBag.Client = TempData["Client"];
            ViewBag.Success = TempData["Success"];
            ViewBag.Problem = TempData["Problem"];
            ViewBag.Matching = TempData["NotMatching"];
            ViewBag.PaidNaira = Session["PaidNaira"];
            ViewBag.PaidDollar = Session["PaidDollar"];
            ViewBag.Unpaid = Session["Unpaid"];
            ViewBag.UnpaidDollar = Session["UnpaidDollar"];
            return View();
        }

        [HttpPost]
        public async Task<ActionResult> CreateClient(FormCollection form, HttpPostedFileBase clientlogo)
        {
            if (ModelState.IsValid)
            {
                using (pdInvoiceEntities db = new pdInvoiceEntities())
                {
                    var extension = Path.GetExtension(clientlogo.FileName);
                    if (extension != null && !extension.Equals(".png"))
                    {
                        TempData["Client"] = "Logo format not supported!";
                        return RedirectToAction("CreateClient");
                    }
                    
                    var imgpath = "../img/clientlogo/" + clientlogo.FileName;
                    clientlogo.SaveAs(Server.MapPath(imgpath));
                    var clientname = form["ClientName"];
                    var clientcode = form["ClientName"];
                    //if (clientname != clientcode) { TempData["NotMatching"] = "Company name doesnt match each other"; return RedirectToAction("CreateClient");}
                    if (await db.Clients.AnyAsync(c => c.ClientName.Equals(clientname) && c.ClientCode.Equals(clientcode)))
                    {
                        TempData["Client"] = "Client name already exist in the database!";
                        return RedirectToAction("CreateClient");
                    }

                    var client = new Client
                    {
                        ClientName = clientname,
                        ClientCode = clientcode,
                        ClientLogo = "img/clientlogo/"+clientlogo.FileName

                    };
                    try
                    {
                        db.Clients.Add(client);
                        await db.SaveChangesAsync().ConfigureAwait(true);
                        TempData["Success"] = "Client created successfully";
                        return RedirectToAction("CreateClient");

                    }
                    catch (Exception e)
                    {
                        TempData["Problem"] = $"An Exception Occured Duplicate entry with message {e.Message}";
                        return RedirectToAction("CreateClient");
                    }

                }
            }

            TempData["Client"] = "An Error Occured!";
            return RedirectToAction("CreateClient");

        }
        /// <summary>
        /// The create staff.
        /// </summary>
        /// <returns>
        /// The <see cref="ActionResult"/>.
        /// </returns>
        public ActionResult CreateStaff()
        {
            var admincheck = Session["AdminId"].ToString();
            ViewBag.PaidNaira = Session["PaidNaira"];
            ViewBag.PaidDollar = Session["PaidDollar"];
            ViewBag.Unpaid = Session["Unpaid"];
            ViewBag.UnpaidDollar = Session["UnpaidDollar"];
            using (pdInvoiceEntities db = new pdInvoiceEntities())
            {
                var admin = db.Staffs.Where(c => c.Email.Equals(admincheck) && c.Role.Equals("MD") || c.Role.Equals("GM") ||
                    c.Role.Equals("Accountant") || c.Email.Equals("sola.saliu@petrodata.net"));
                if (admin.Any(c => c.Email.Equals(admincheck)))
                {
                    ViewBag.Staff = TempData["Staff"];
                    ViewBag.Success = TempData["Success"];
                    ViewBag.Problem = TempData["Problem"];
                    var staffviemmodel = new StaffViewModel
                    {
                        StaffRoles = GetRoles(db)
                    };
                    return View(staffviemmodel);
                }

                return RedirectToAction("Index");
            }

        }
        [HttpPost]
        public async Task<ActionResult> CreateStaff(FormCollection form)
        {
            if (ModelState.IsValid)
            {
                using (pdInvoiceEntities db = new pdInvoiceEntities())
                {
                    var staffname = form["FullName"];
                    var email = form["email"];
                    var password = form["Password"];
                    var role = form["StaffRoles"];
                    if (await db.Staffs.AnyAsync(c => c.FullName.Equals(staffname) && c.Email.Equals(email)))
                    {
                        TempData["Staff"] = "User already exist in the database!";
                        return RedirectToAction("CreateStaff");
                    }

                    var staff = new Staff
                    {
                        FullName = staffname,
                        Email = email,
                        Password = Encryption.Sha256(password),
                        Role = role

                    };
                    try
                    {
                        db.Staffs.Add(staff);
                        await db.SaveChangesAsync().ConfigureAwait(true);
                        TempData["Success"] = "Staff created successfully";
                        return RedirectToAction("CreateStaff");
                    }
                    catch (Exception e)
                    {
                        TempData["Problem"] = $"Cannot insert duplicate staff, Message: {e.Message}";
                        return RedirectToAction("CreateStaff");
                    }

                }
            }
            TempData["Problem"] = "Form state is not valid";
            return RedirectToAction("CreateStaff");
        }
        /// <summary>
        /// The suspend staff.
        /// </summary>
        /// <returns>
        /// The <see cref="ActionResult"/>.
        /// </returns>
        public ActionResult SuspendStaff()
        {
            return View();
        }


        public ViewResult ActivityLog()
        {
            ViewBag.UserFullName = Session["AdminFullname"].ToString();
            ViewBag.Username = Session["AdminId"].ToString();
            ViewBag.PaidNaira = Session["PaidNaira"];
            ViewBag.PaidDollar = Session["PaidDollar"];
            ViewBag.Unpaid = Session["Unpaid"];
            ViewBag.UnpaidDollar = Session["UnpaidDollar"];
            using (pdInvoiceEntities db = new pdInvoiceEntities())
            {

                var actvityViewModel = new ActivityViewModel
                {
                    Activities = GetActivities(db)
                };
                return View(actvityViewModel);
            }

        }

        [HttpGet]
        public ActionResult MonthlyAnalysis()
        {
            ViewBag.UserFullName = Session["AdminFullname"].ToString();
            ViewBag.Username = Session["AdminId"].ToString();
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
            ViewBag.PaidNaira = Session["PaidNaira"];
            ViewBag.PaidDollar = Session["PaidDollar"];
            ViewBag.Unpaid = Session["Unpaid"];
            ViewBag.UnpaidDollar = Session["UnpaidDollar"];
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
        /// <summary>
        /// The get clients.
        /// </summary>
        /// <returns>
        /// The <see cref="List"/>.
        /// </returns>
        public List<Clients> GetClients()
        {
            using (pdInvoiceEntities db = new pdInvoiceEntities())
            {
                var clientslist = db.Clients.Select(c => new Clients
                {
                    ClientCode = c.ClientCode,
                    ClientName = c.ClientName,
                    ClientLogo = c.ClientLogo
                }).Distinct().ToList();
                clientslist.Insert(0, new Clients { ClientName = "Select Client", ClientCode = "Select Client" });
                return clientslist;
            }
        }

        /// <summary>
        /// The get entries.
        /// </summary>
        /// <returns>
        /// The <see cref="List"/>.
        /// </returns>
        public List<ServiceEntry> GetEntries()
        {
            using (pdInvoiceEntities db = new pdInvoiceEntities())
            {
                var clientslist = db.ServiceEnteries.Select(c => new ServiceEntry
                {
                    InvoiceNo = c.InvoiceNumber,
                    SeNumber = c.SENumber,
                    ClientName = c.ClientName
                }).ToList();
                return clientslist;
            }
        }

        public List<Activity> GetActivities(pdInvoiceEntities db)
        {
            var activities = db.ActivityLogs.OrderByDescending(c => c.Id).Select(c => new Activity
            {
                Id = c.Id,
                StaffName = c.Staffinfo,
                ActivityType = c.TypeofActivity,
                TimeCreated = c.TimeCreated
            }).ToList();
            return activities;
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
                    DatePaid =n.DatePaid,
                    DueDate = n.DueDate,
                    Comments = n.Comments
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
        public ActionResult Datadownload(FormCollection form)
        {
            var gv = new GridView();
            var clientname = form["GetClients"]; //Session["ClientId"].ToString();
            var year = form["YearList"];
            // var _year = int.Parse(year);
            // var datatype = form["DataType"].ToString();
            //   var date = DateTime.UtcNow.ToLongDateString();
            gv.DataSource = GetAvailableDataByProspect(clientname, year);
            gv.DataBind();
            Response.ClearContent();
            Response.Buffer = true;
            Response.AddHeader("content-disposition",
                "attachment; filename=export.xls");
            Response.ContentType = "application/ms-excel";
            Response.Charset = "";
            StringWriter objStringWriter = new StringWriter();
            HtmlTextWriter objHtmlTextWriter = new HtmlTextWriter(objStringWriter);
            gv.RenderControl(objHtmlTextWriter);
            Response.Output.Write(objStringWriter.ToString());
            Response.Flush();
            Response.End();
            return RedirectToAction("MonthlyAnalysis");

        }

        public ActionResult ActivityDownload(FormCollection form)
        {

            var gv = new GridView();
            var clientname = form["GetClients"]; //Session["ClientId"].ToString();
            var year = form["YearList"];
            // var _year = int.Parse(year);
            // var datatype = form["DataType"].ToString();
            //   var date = DateTime.UtcNow.ToLongDateString();
            using (pdInvoiceEntities db = new pdInvoiceEntities())
            {
                gv.DataSource = db.ActivityLogs.ToList();
                gv.DataBind();
                Response.ClearContent();
                Response.Buffer = true;
                Response.AddHeader("content-disposition",
                    "attachment; filename=activities.xls");
                Response.ContentType = "application/ms-excel";
                Response.Charset = "";
                StringWriter objStringWriter = new StringWriter();
                HtmlTextWriter objHtmlTextWriter = new HtmlTextWriter(objStringWriter);
                gv.RenderControl(objHtmlTextWriter);
                Response.Output.Write(objStringWriter.ToString());
                Response.Flush();
                Response.End();
                return RedirectToAction("ActivityLog");
            }

        }
        public IList<InvoiceExport> GetAvailableDataByProspect(string clientname, string year)
        {
            using (pdInvoiceEntities db = new pdInvoiceEntities())
            {
                var _year = int.Parse(year);
                // long id = 0;
                //  var dataList = new List<InvoiceExport>();
                var naira = db.Nairas.Where(c => c.Year.Equals(_year) && c.ClientName.Equals(clientname))
                    .AsEnumerable();
                var dollar = db.Dollars.Where(c => c.Year.Equals(_year) && c.ClientName.Equals(clientname))
                    .AsEnumerable();
                // IEnumerable<dynamic> ds = dynamicDataType.AsEnumerable();
                //var clientdata = ds as dynamic[] ?? ds.ToArray();
                return IterateDataList(naira, dollar);
            }
        }

        private IList<InvoiceExport> IterateDataList(IEnumerable<Naira> naira, IEnumerable<Dollar> dollar)
        {
            var invoiceexport = new List<InvoiceExport>();

            foreach (var nair in naira)
            {
                var export = new InvoiceExport
                {
                    InvoiceType = "Naira",
                    ClientName = nair.ClientName,
                    Year = nair.Year,
                    Month = nair.Month,
                    Date = $"{nair.Month}/{nair.Year}",
                    InvoiceNumber = nair.InvoiceNumber,
                    SeNumber = nair.SENumber,
                    FxRate = nair.FxRate,
                    Amount = nair.Amount,
                    Vat = nair.VAT,
                    TotalInvoice = nair.TotalInvoice,
                    Payable = nair.Payable,
                    LcdCharge = nair.LcdCharge,
                    AmountPaid = nair.AmountPaid,
                    Outstanding = nair.Outstanding,
                    DateSubmitted = nair.DateSubmitted.ToString("dd-MMMM-yyyy"),
                    DueDate = nair.DueDate.ToString("dd-MMMM-yyyy"),
                    WithHoldingTax = nair.WithHoldingTax,
                    Comments = nair.Comments,
                    NairaEquiv = nair.NairaValue
                };
                invoiceexport.Add(export);
            }

            foreach (var nair in dollar)
            {
                var export = new InvoiceExport
                {
                    InvoiceType = "Dollar",
                    ClientName = nair.ClientName,
                    Year = nair.Year,
                    Month = nair.Month,
                    Date = $"{nair.Month}/{nair.Year}",
                    InvoiceNumber = nair.InvoiceNumber,
                    SeNumber = nair.SENumber,
                    FxRate = (decimal)0.00000,
                    Amount = nair.Amount,
                    Vat = nair.VAT,
                    TotalInvoice = nair.TotalInvoice,
                    Payable = nair.Payable,
                    LcdCharge = nair.LcdCharge,
                    AmountPaid = nair.AmountPaid,
                    Outstanding = nair.Outstanding,
                    DateSubmitted = nair.DateSubmitted.ToString("dd-MMMM-yyyy"),
                    DueDate = nair.DueDate.ToString("dd-MMMM-yyyy"),
                    WithHoldingTax = nair.WithHoldinTax,
                    Comments = nair.Comments
                };
                invoiceexport.Add(export);
            }

            return invoiceexport;
        }
        [HttpGet]
        public ActionResult EditInvoice()
        {
            ViewBag.UserFullName = Session["AdminFullname"].ToString();
            ViewBag.Username = Session["AdminId"].ToString();
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
            ViewBag.PaidNaira = Session["PaidNaira"];
            ViewBag.PaidDollar = Session["PaidDollar"];
            ViewBag.Unpaid = Session["Unpaid"];
            ViewBag.UnpaidDollar = Session["UnpaidDollar"];
            ViewBag.Success = TempData["SuccessEdit"];
            ViewBag.Error = TempData["Error"];
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
                            var dated = DateTime.UtcNow;
                            var span = dated.TimeOfDay;
                            var activitybyStaff = $"Updated the amountpaid for dollar part of Invoice Number {dataupdate.InvoiceNumber} with Services Entry Number {dataupdate.SENumber}, The amount paid is {decimal.Parse(form["AmountPaid"])}  @ {dated} and timespan {span}";
                            var activityLog = new ActivityLog
                            {
                                Staffinfo = this.Session["AdminFullname"].ToString(),
                                TypeofActivity = activitybyStaff
                            };
                            db.ActivityLogs.AddOrUpdate(activityLog);
                            dataupdate.AmountPaid = decimal.Parse(form["AmountPaid"]);
                            dataupdate.Outstanding = decimal.Parse(form["Outstanding"]);
                            await db.SaveChangesAsync();
                            TempData["SuccessEdit"] = "Paid amount successfully updated for ";
                            return RedirectToAction("EditInvoice", new { invoicenum, senum });
                        }
                        TempData["Error"] = "The Invoice doesn't exist";
                        return RedirectToAction("EditInvoice", new { invoicenum, senum });
                    }
                    TempData["Error"] = "The submitted entry is invalid";
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
                            //ViewBag.Success = "Paid amount successfully updated for ";
                            TempData["SuccessEdit"] = "Paid amount successfully updated for ";
                            return RedirectToAction("EditInvoice", new { invoicenum, senum });
                        }
                        TempData["Error"] = "The Invoice doesn't exist";
                        return RedirectToAction("EditInvoice", new { invoicenum, senum });
                    }
                    TempData["Error"] = "The submitted entry is invalid";
                    return RedirectToAction("EditInvoice", new { invoicenum, senum });
                }

            }
            return RedirectToAction("EditInvoice");
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
                }).Distinct().ToList();
                return remittancevm.Distinct().ToList();
            }
        }

        public List<StaffRole> GetRoles(pdInvoiceEntities db)
        {
            var role = db.Roles.Select(c => new StaffRole { Role = c.Role1 }).ToList();
            return role;
        }

        public ActionResult UploadClient()
        {

            return View();
        }

        [ActionName("UploadC")]
        [HttpPost]
        public ActionResult UploadC(HttpPostedFileBase file)
        {
            using (pdInvoiceEntities db = new pdInvoiceEntities())
            {
                var extension = Path.GetExtension(file.FileName);
                try
                {
                    if (extension == null || !extension.Equals(".xlsx")) return RedirectToAction("UploadClient");
                    using (var reader = new BinaryReader(file.InputStream))
                    {
                        var filecontent = reader.BaseStream;
                        var excel1 = new ExcelPackage(filecontent);
                        var t = excel1.ToDataTable();
                        var excel = new ExcelPackage(file.InputStream);
                        string path = Server.MapPath("~/DebtorSheet/" + file.FileName);
                        file.SaveAs(path);
                        var excelnew = new ExcelQueryFactory(path);
                        var clientnameuupload = (excelnew.Worksheet()
                            .Select(e => new ExcelClientUpload { ClientName = e["ClientName"], ClientCode = e["ClientCode"] }));

                        var client = clientnameuupload
                            .Select(c => new Client { ClientName = c.ClientName, ClientCode = c.ClientCode })
                            .AsEnumerable();
                        var dated = DateTime.UtcNow;
                        var span = dated.TimeOfDay;
                        var activitybyStaff = $"Uploaded clients @ {dated} and timespan {span}";
                        var activityLog = new ActivityLog
                        {
                            Staffinfo = Session["AdminFullname"].ToString(),
                            TypeofActivity = activitybyStaff
                        };
                        db.ActivityLogs.AddOrUpdate(activityLog);
                        db.Clients.AddRange(client);
                        db.SaveChanges();
                        TempData["ClientMessage"] = "Clients upload was successfully. Thank you";
                    }
                    return RedirectToAction("UploadInvoices");
                }
                catch (Exception exception)
                {
                    TempData["ClientError"] = $"an error occured, {exception.InnerException}";
                    return RedirectToAction("UploadInvoices");
                }
                //return View("UploadClient", ViewBag);
            }

        }
        [HttpGet]
        public async Task<ActionResult> DeleteNaira(string invoicenum, string servicenum)
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
                ViewBag.DeleteMessage = TempData["DeleteMessage"];
                return View(data);
            }
        }
        [HttpPost]
        [ActionName("RemoveNaira")]
        public async Task<ActionResult> RemoveNaira(FormCollection form)
        {
            var invoicenum = form["invoicenum"];
            var servicenum = form["servicenum"];
            using (pdInvoiceEntities db = new pdInvoiceEntities())
            {
                try
                {
                    var data = await db.Nairas.Where(c => c.InvoiceNumber.Equals(invoicenum) && c.SENumber.Equals(servicenum))
                      .FirstOrDefaultAsync();
                    if (data != null) db.Nairas.Remove(data);
                    var dated = DateTime.UtcNow;
                    var span = dated.TimeOfDay;
                    var activitybyStaff = $"Deleted information associated with naira invoice :{data?.InvoiceNumber} @ {dated} and timespan {span}";
                    var activityLog = new ActivityLog
                    {
                        Staffinfo = Session["AdminFullname"].ToString(),
                        TypeofActivity = activitybyStaff
                    };
                    db.ActivityLogs.AddOrUpdate(activityLog);
                    await db.SaveChangesAsync();
                    TempData["DeleteMessage"] = $"information matching {invoicenum} and {servicenum} was deleted from the database!!!. Thank you";
                    return RedirectToAction("DeleteNaira");
                }
                catch (Exception e)
                {
                    TempData["DeleteMessage"] = $"information matching {invoicenum} and {servicenum} could not be deleted from the database becuase an error {e.InnerException.Message} occurred!!!. Thank you";
                    return RedirectToAction("DeleteNaira");
                }

            }

        }
        public async Task<ActionResult> DeleteDollar(string invoicenum, string servicenum)
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
        [HttpPost]
        [ActionName("RemoveDollar")]
        public async Task<ActionResult> RemoveDollar(FormCollection form)
        {
            var invoicenum = form["invoicenum"];
            var servicenum = form["servicenum"];
            using (pdInvoiceEntities db = new pdInvoiceEntities())
            {
                try
                {
                    var data = await db.Dollars.Where(c => c.InvoiceNumber.Equals(invoicenum) && c.SENumber.Equals(servicenum))
                      .FirstOrDefaultAsync();
                    if (data != null)
                    {
                        db.Dollars.Remove(data);
                        var dated = DateTime.UtcNow;
                        var span = dated.TimeOfDay;
                        var activitybyStaff =
                            $"Deleted information associated with dollar invoice :{data?.InvoiceNumber} @ {dated} and timespan {span}";
                        var activityLog = new ActivityLog
                        {
                            Staffinfo = Session["AdminFullname"].ToString(),
                            TypeofActivity = activitybyStaff
                        };
                        db.ActivityLogs.AddOrUpdate(activityLog);
                    }

                    await db.SaveChangesAsync();
                    TempData["DeleteMessage"] = $"information matching {invoicenum} and {servicenum} was deleted from the database!!!. Thank you";
                    return RedirectToAction("DeleteNaira");
                }
                catch (Exception e)
                {
                    TempData["DeleteMessage"] = $"information matching {invoicenum} and {servicenum} could not be deleted from the database becuase an error {e.InnerException.Message} occurred!!!. Thank you";
                    return RedirectToAction("DeleteNaira");
                }

            }

        }
        public ActionResult UploadInvoices()
        {
            ViewBag.UserFullName = Session["AdminFullname"].ToString();
            ViewBag.Username = Session["AdminId"].ToString();
            ClientsViewModel clientlist = new ClientsViewModel { GetClients = GetClients() };
            ViewBag.Mes = TempData["Message"];
            ViewBag.ClientMessage = TempData["ClientMessage"];
            ViewBag.Error = TempData["Error"];
            ViewBag.ClientError = TempData["ClientError"];
            ViewBag.Exception = TempData["Exception"];
            ViewBag.PaidNaira = Session["PaidNaira"];
            ViewBag.PaidDollar = Session["PaidDollar"];
            ViewBag.Unpaid = Session["Unpaid"];
            ViewBag.UnpaidDollar = Session["UnpaidDollar"];
            ViewBag.SeInvoiceSaved = TempData["SeInvoiceSaved"];
            ViewBag.SeInvoiceError = TempData["SeInvoiceError"];
            return View(clientlist);
        }


        [HttpPost]
        public ActionResult UploadInvoices(FormCollection form, HttpPostedFileBase file)
        {
            var extension = Path.GetExtension(file.FileName);
            using (pdInvoiceEntities db = new pdInvoiceEntities())
            {
                try
                {
                    if (extension == null || !extension.Equals(".xlsx")) return RedirectToAction("UploadInvoices");
                    using (var reader = new BinaryReader(file.InputStream))
                    {
                        var filecontent = reader.BaseStream;
                        var excel1 = new ExcelPackage(filecontent);
                        var t = excel1.ToDataTable();
                        var excel = new ExcelPackage(file.InputStream);
                        string path = Server.MapPath("~/InvoiceUpload/" + file.FileName);
                        file.SaveAs(path);
                        var excelnew = new ExcelQueryFactory(path);
                        var invoicetype = form["invoicetype"].ToString();
                        if (invoicetype.Equals("Naira"))
                        {
                          //  decimal fxzero = 0;
                            var invoiceupload = excelnew.Worksheet("Naira")
                                .Select(e => new NairaInvoice
                                {
                                    ClientName = e["ClientName"],
                                    //Year = int.Parse(e["Year"]),
                                    // Month = int.Parse(e["Month"]),decimal.Parse(e["FxRate"])
                                    InvoiceNumber = e["InvoiceNumber"],
                                    SeNumber = e["SENumber"],
                                    FxRate = decimal.Parse(e["FxRate"] == null || e["FxRate"].ToString() == String.Empty ? "0" : e["FxRate"].ToString()),
                                    Amount = decimal.Parse(e["Amount"] == null || e["Amount"].ToString() == String.Empty ? "0" : e["Amount"].ToString()),
                                    Vat = decimal.Parse(e["VAT"] == null || e["VAT"].ToString() == String.Empty ? "0" : e["VAT"].ToString()),
                                    TotalInvoice = decimal.Parse(e["TotalInvoice"] == null || e["TotalInvoice"].ToString() == String.Empty ? "0" : e["TotalInvoice"].ToString()),
                                    Payable = decimal.Parse(e["Payable"] == null || e["Payable"].ToString() == String.Empty ? "0" : e["Payable"].ToString()),
                                    LcdCharge = decimal.Parse(e["LcdCharge"] == null || e["LcdCharge"].ToString() == String.Empty ? "0" : e["LcdCharge"].ToString()),
                                    AmountPaid = decimal.Parse(e["AmountPaid"]),
                                    Outstanding = decimal.Parse(e["Outstanding"]),
                                    DateSubmitted = DateTime.Parse(e["DateSubmitted"]),
                                    DueDate = DateTime.Parse(e["DueDate"]),
                                    DatePaid = DateTime.Parse(e["DatePaid"]),
                                    WithHoldingTax = decimal.Parse(e["WithHoldingTax"]),
                                    Comments = e["Comments"],
                                    NairaValue = decimal.Parse(e["NairaValue"])
                                });

                            var invoicen = invoiceupload
                                .Select(c => new Naira
                                {
                                    ClientName = c.ClientName,
                                    //Year = (int)c.Year,
                                   // Month = c.Month,
                                    InvoiceNumber = c.InvoiceNumber,
                                    SENumber = c.SeNumber,
                                 //   FxRate = c.FxRate,
                                    Amount = c.Amount,
                                    VAT = c.Vat,
                                    TotalInvoice = c.TotalInvoice,
                                    Payable = c.Payable,
                                    LcdCharge = c.LcdCharge,
                                    AmountPaid = c.AmountPaid,
                                    Outstanding = c.Outstanding,
                                    DateSubmitted = c.DateSubmitted,
                                    DueDate = c.DueDate,
                                    DatePaid = c.DatePaid,
                                    WithHoldingTax = c.WithHoldingTax,
                                    Comments = c.Comments,
                                    NairaValue = c.NairaValue
                                })
                                .AsEnumerable();
                            db.Nairas.AddRange(invoicen);
                            var dated = DateTime.UtcNow;
                            var span = dated.TimeOfDay;
                            var activitybyStaff = $"uploaded invoices information associated with naira invoice @ {dated} and timespan {span}";
                            var activityLog = new ActivityLog
                            {
                                Staffinfo = Session["AdminFullname"].ToString(),
                                TypeofActivity = activitybyStaff
                            };
                            db.ActivityLogs.AddOrUpdate(activityLog);
                            db.SaveChanges();
                            TempData["Message"] = "Invoice upload was successfully. Thank you";
                            return RedirectToAction("UploadInvoices");
                        }

                        if (invoicetype.Equals("Dollar"))
                        {
                            var invoiceupload = excelnew.Worksheet("Dollar")
                                .Select(e => new DollarInvoice
                                {
                                    ClientName = e["ClientName"],
                                 //   Year = int.Parse(e["Year"]),
                                  //  Month = int.Parse(e["Month"]),
                                    InvoiceNumber = e["InvoiceNumber"],
                                    SeNumber = e["SENumber"],
                                    Amount = decimal.Parse(e["Amount"]),
                                    Vat = decimal.Parse(e["VAT"]),
                                    TotalInvoice = decimal.Parse(e["TotalInvoice"]),
                                    Payable = decimal.Parse(e["Payable"]),
                                    LcdCharge = decimal.Parse(e["LcdCharge"]),
                                    AmountPaid = decimal.Parse(e["AmountPaid"]),
                                    Outstanding = decimal.Parse(e["Outstanding"]),
                                    DateSubmitted = DateTime.Parse(e["DateSubmitted"]),
                                    DueDate = DateTime.Parse(e["DueDate"]),
                                    DatePaid = DateTime.Parse(e["DatePaid"]),
                                    WithHoldinTax = decimal.Parse(e["WithHoldingTax"]),
                                    Comments = e["Comments"]
                                });

                            var invoicen = invoiceupload
                                .Select(c => new Dollar
                                {
                                    ClientName = c.ClientName,
                                  //  Year = (int)c.Year,
                                   // Month = c.Month,
                                    InvoiceNumber = c.InvoiceNumber,
                                    SENumber = c.SeNumber,
                                    Amount = c.Amount,
                                    VAT = c.Vat,
                                    TotalInvoice = c.TotalInvoice,
                                    Payable = c.Payable,
                                    LcdCharge = c.LcdCharge,
                                    AmountPaid = c.AmountPaid,
                                    Outstanding = c.Outstanding,
                                    DateSubmitted = c.DateSubmitted,
                                    DueDate = c.DueDate,
                                    DatePaid = c.DatePaid,
                                    WithHoldinTax = c.WithHoldinTax,
                                    Comments = c.Comments
                                })
                                .AsEnumerable();
                            db.Dollars.AddRange(invoicen);
                            var dated = DateTime.UtcNow;
                            var span = dated.TimeOfDay;
                            var activitybyStaff = $"uploaded invoices information associated with dollar invoice @ {dated} and timespan {span}";
                            var activityLog = new ActivityLog
                            {
                                Staffinfo = Session["AdminFullname"].ToString(),
                                TypeofActivity = activitybyStaff
                            };
                            db.ActivityLogs.AddOrUpdate(activityLog);
                            db.SaveChanges();
                            TempData["Message"] = "Invoice upload was successfully. Thank you";
                            return RedirectToAction("UploadInvoices");
                        }
                        TempData["Error"] = "Invalid Selection";
                        return RedirectToAction("UploadInvoices");
                    }

                }
                catch (Exception e)
                {
                    //  throw e;
                    TempData["Exception"] = "Fatal Error Occured " + e;
                    return RedirectToAction("UploadInvoices");
                }

            }

        }

        [HttpPost]
        public ActionResult UploadServices(HttpPostedFileBase file)
        {
            var extension = Path.GetExtension(file.FileName);
            using (pdInvoiceEntities db = new pdInvoiceEntities())
            {
                try
                {
                    if (extension == null || !extension.Equals(".xlsx")) return RedirectToAction("UploadInvoices");
                    using (var reader = new BinaryReader(file.InputStream))
                    {
                        var filecontent = reader.BaseStream;
                        var excel1 = new ExcelPackage(filecontent);
                        var t = excel1.ToDataTable();
                        var excel = new ExcelPackage(file.InputStream);
                        string path = Server.MapPath("~/InvoiceUpload/" + file.FileName);
                        file.SaveAs(path);
                        var excelnew = new ExcelQueryFactory(path);
                        db.SaveChanges();
                        var invoiceno = (excelnew.Worksheet("Invoices")
                            .Select(e =>
                                new InvoiceNumber { ClientName = e["ClientName"], InvoiceNo = e["InvoiceNumber"] }));
                        var inv = invoiceno
                            .Select(c => new Invoice { InvoiceNumber = c.InvoiceNo, ClientName = c.ClientName })
                            .AsEnumerable();
                        db.Invoices.AddRange(inv);
                        db.SaveChanges();
                        var seno = (excelnew.Worksheet("Invoices")
                            .Select(e => new ServiceEntry
                            {
                                ClientName = e["ClientName"],
                                InvoiceNo = e["InvoiceNumber"],
                                SeNumber = e["SENumber"]
                            }));
                        var se = seno.Select(c => new ServiceEntery
                        {
                            SENumber = c.SeNumber,
                            InvoiceNumber = c.InvoiceNo,
                            ClientName = c.ClientName
                        }).AsEnumerable();
                        db.ServiceEnteries.AddRange(se);
                        db.SaveChanges();
                        TempData["SeInvoiceSaved"] = "SE and Invoices were successfully uploaded";
                        return RedirectToAction("UploadInvoices");
                    }
                }
                catch (Exception e)
                {
                    TempData["SeInvoiceError"] = "An Error Occured " + e.Message;
                    return RedirectToAction("UploadInvoices");
                }
            }
        }

        public ActionResult Reconcilation()
        {
            using (pdInvoiceEntities db = new pdInvoiceEntities())
            {
                var monthlyAnalysisViewmModel = new InvoiceViewModel
                {
                    NairaInvoices = GetNairaInvoices(db),
                    DollarInvoices = GetDollarInvoices(db),
                    GetClients = GetClients(),
                    YearList = YearList(),
                    Reconcilations = GetReconcilation(db)
                };
                return View(monthlyAnalysisViewmModel);
            }
          
        }

        private List<DollarInvoice> GetDollarInvoices(pdInvoiceEntities db)
        {
            var dollarinvoice = db.Dollars.Select(c=> new DollarInvoice()).ToList();
            return dollarinvoice;
        }

        private List<Reconcilation> GetReconcilation(pdInvoiceEntities db)
        {

            //var reconcilen = db.Nairas.Select(c => new Reconcilation {ClientName = c.ClientName,NairaOutsnd = c.Outstanding}).ToList();
            ////var k = reconcilen.Where(c => c.ClientName.Equals("hh")).Sum();
            //var reconciled = db.Dollars.Select(c => new Reconcilation { ClientName = c.ClientName, DollarOutsnd = c.Outstanding }).ToList();
            //var rec = reconcilen.Concat(reconciled).ToList();
            var nairaval = db.Nairas.Select(c => new { c.ClientName, c.Outstanding }).ToList();
            var dollarval = db.Dollars.Select(c=> new {c.ClientName, c.Outstanding}).ToList();
       
            var reci = new List<Reconcilation>();
            foreach (var c in GetClients().Where(k => k.ClientName != "Select Client"))
            {
                var d = dollarval.Where(l => l.ClientName == c.ClientName).Sum(o => o.Outstanding);
                var n = nairaval.Where(l => l.ClientName == c.ClientName).Sum(o => o.Outstanding);

                var recc = new Reconcilation
                {
                    ClientName = c.ClientName,
                    NairaOutsnd = n,
                    DollarOutsnd = d
                };
                reci.Add(recc);

            }
            return reci;


        }

        private List<NairaInvoice> GetNairaInvoices(pdInvoiceEntities db)
        {
            var dollarinvoice = db.Nairas.Select(c => new NairaInvoice()).ToList();
            return dollarinvoice;
        }

        public ActionResult UploadHistory()
        {
            return View();
        }

        public ActionResult HistoryDownload()
        {
            return RedirectToAction("UploadHistory");
        }
        public ActionResult GeneratePDF()
        {
            return new Rotativa.ActionAsPdf("Reconcilation")
            {
                FileName = "Debtors Reconcilation.pdf",
                PageSize = Size.A3,
                PageOrientation = Rotativa.Options.Orientation.Landscape,
                PageMargins = { Left = 0, Right = 0 },
                //ContentDisposition = ContentDisposition.Inline
            }; ;
        }
    }

    internal class ExcelClientUpload
    {
        public string ClientCode { get; set; }
        public string ClientName { get; set; }
    }
}