using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;
using DebtorsControl.Classes;
using DebtorsControl.Models;

namespace DebtorsControl.Controllers
{
    public class AccountController : Controller
    {
        // GET: Account
        public ActionResult Login()
        {
            ViewBag.ErrorMessage = TempData["ErrorMessage"];
            ViewBag.PasswordError = TempData["PasswordError"];
            return View();
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Login(FormCollection loginform)
        {
            if (ModelState.IsValid)
            {
                var adminname = loginform["email"];
                var password = loginform["pass"];
                if (!String.IsNullOrEmpty(adminname))
                {
                    var db = new pdInvoiceEntities();
                    var usernamee = db.Staffs.Any(a => a.Email == adminname);
                    if (usernamee)
                    {
                        var username = db.Staffs.Single(a => a.Email == adminname);
                        if (username == null)
                        {
                            TempData["ErrorMessage"] = "User not found, verify you have entered a valid information";
                            return View();
                        }
                        var pass = Encryption.Sha256(password);
                        if (username.Role.Equals("MD") || username.Role.Equals("GM") ||
                            username.Role.Equals("Accountant"))
                        {
                            if (pass.Equals(username.Password))
                            {
                                Session["AdminId"] = username.Email;
                                Session["AdminFullname"] = username.FullName;
                                return RedirectToAction("Index", "Debtors");
                            }

                            if (!pass.Equals(username.Password))
                            {
                                TempData["PasswordError"] = "Invalid password";
                                return RedirectToAction("Login");
                            }

                        }
                        else
                        {
                            if (pass.Equals(username.Password))
                            {
                                Session["AdminId"] = username.Email;
                                Session["AdminFullname"] = username.FullName;
                                return RedirectToAction("Index", "Debtors");
                            }

                            if (!pass.Equals(username.Password))
                            {
                                TempData["PasswordError"] = "Invalid password";
                                return RedirectToAction("Login");
                            }
                        }
                    }

                    TempData["ErrorMessage"] = "Invalid user or user not found!!!";
                    return RedirectToAction("Login");

                }
            }

            TempData["ErrorMessage"] = "User details cannot be empty";
            return View();
        }

        public ActionResult ChangePassword()
        {
            return View();
        }
        [HttpPost]
        public async Task<ActionResult> ChangePassword(FormCollection loginform)
        {
            if (ModelState.IsValid)
            {
                var adminname = loginform["email"];
                var password = loginform["pass"];
                var password2 = loginform["repeatpass"];
                if (!string.IsNullOrEmpty(adminname))
                {
                    if (password != password2)
                    {
                        ViewBag.PassMismatch = "Your password do not match";
                        return View();
                    }
                    var db = new pdInvoiceEntities();
                    var username = db.Staffs.Single(a => a.Email == adminname);
                    if (username == null)
                    {
                        ViewBag.ErrorMessage = "User not found, verify you have entered a valid information";
                        return View();
                    }
                    var pass = Encryption.Sha256(password);
                    username.Password = pass;
                    await db.SaveChangesAsync();
                    if (username.Role.Equals("MD") || username.Role.Equals("GM") ||
                        username.Role.Equals("Accountant"))
                    {
                        if (pass.Equals(username.Password))
                        {
                            Session["AdminId"] = username.Email;
                            Session["AdminFullname"] = username.FullName;
                            return RedirectToAction("Index", "Debtors");
                        }
                    }
                    else
                    {
                        if (pass.Equals(username.Password))
                        {
                            Session["AdminId"] = username.Email;
                            Session["AdminFullname"] = username.FullName;
                            return RedirectToAction("Index", "Debtors");
                        }
                    }

                }
            }
            return View();
        }
        public ActionResult Logout()
        {

            Session.Abandon();
           Session.Clear();
            return RedirectToAction("Login");
        }
        //public async Task<ActionResult> Login(FormCollection form)
        //{
        //    return View();
        //}
    }
}