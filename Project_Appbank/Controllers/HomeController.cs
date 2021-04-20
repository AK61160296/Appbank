﻿using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using Project_Appbank.Models;
using Project_Appbank.Models.DBModels;
using Project_Appbank.ViewModels;
using System;
using System.Diagnostics;
using System.Linq;
using Project_Appbank.Constant;

namespace Project_Appbank.Controllers
{
    public class HomeController : Controller
    {
        private appbankContext _context;
        private string check_status;
        public HomeController(appbankContext context)
        {
            this._context = context;
        }


        public IActionResult Userlist()
        {

            IQueryable<UserViewModel> user = from a in _context.User
                                                 //where a.isEnable == true
                                             select new UserViewModel
                                             {
                                                 UserId = a.UserId,
                                                 UserName = a.UserName,
                                                 UserEmail = a.UserEmail,
                                                 UserIsActive = a.UserIsActive
                                             };

            return View(user.ToList());
        }
        [HttpPost]
        public IActionResult login([FromBody] User model)
        {
            HttpContext.Session.SetString("usersession", model.UserId.ToString());

            return Json(Checkstatus.success);
        }

        public IActionResult Index()
        {

            return View();
        }
        [HttpPost]
        public IActionResult Search([FromBody] AccountParam model)
        {
            var user_session = HttpContext.Session.GetString("usersession");
            IQueryable<AccountViewModels> accounts = from a in _context.Account
                                                     where a.UserId == Int32.Parse(user_session) && (a.AcNumber.Contains(model.AcNumber) || a.AcName.Contains(model.AcName))
                                                     select new AccountViewModels
                                                     {
                                                         AcId = a.AcId,
                                                         AcNumber = a.AcNumber,
                                                         AcName = a.AcName,
                                                         AcBalance = a.AcBalance,
                                                         AcIsActive = a.AcIsActive
                                                     };
            return Json(accounts.ToList());
        }

        [HttpPost]
        public IActionResult Edit([FromBody] AccountParam model)
        {

            IQueryable<AccountViewModels> account = from a in _context.Account
                                                    where a.AcId == model.AcId
                                                    select new AccountViewModels
                                                    {
                                                        AcId = a.AcId,
                                                        AcNumber = a.AcNumber,
                                                        AcName = a.AcName,
                                                        AcBalance = a.AcBalance,
                                                        AcIsActive = a.AcIsActive
                                                    };
            return Json(account.Single());
        }

        [HttpPost]
        public IActionResult Add([FromBody] AccountParam model)
        {
            var user_session = HttpContext.Session.GetString("usersession");
            IQueryable<Account> json_data = from a in _context.Account
                                            where a.AcNumber == model.AcNumber
                                            select new Account
                                            {
                                                AcId = a.AcId,
                                            };

            if (json_data.Count() == 0)
            {
                var account = new Account()
                {
                    AcNumber = model.AcNumber,
                    AcBalance = 0,
                    AcName = model.AcName,
                    AcIsActive = model.AcIsActive,
                    UserId = Int32.Parse(user_session),
                };
                _context.Account.Add(account);
                _context.SaveChanges();
                check_status = Checkstatus.success;
            }


            return Json(check_status);
        }

        public IActionResult Update([FromBody] Account model)
        {
            //_context.Account.Attach(model);
            //EntityEntry<Account> entry = _context.Entry(model);
            //entry.State = EntityState.Modified;
            //_context.SaveChanges();

            var account = (from c in _context.Account
                           where c.AcId == model.AcId
                           select c).Single();

            account.AcName = model.AcName;
            account.AcIsActive = model.AcIsActive;
            account.AcNumber = model.AcNumber;
            _context.SaveChanges();

            return Json(Checkstatus.success);
        }

        public IActionResult Privacy()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }

    internal class FrombodyAttribute : Attribute
    {
    }
}
