using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Project_Appbank.Models;
using Project_Appbank.Models.DBModels;
using Project_Appbank.ViewModels;
using System;
using System.Diagnostics;
using System.Linq;

namespace Project_Appbank.Controllers
{
    public class HomeController : Controller
    {
        private appbankContext _context;

        public HomeController(appbankContext context)
        {
            this._context = context;
        }


        public IActionResult Userlist()
        {

            IQueryable<UserViewModel> us_data = from a in _context.User
                                                    //where a.isEnable == true
                                                select new UserViewModel
                                                {
                                                    UserId = a.UserId,
                                                    UserName = a.UserName,
                                                    UserEmail = a.UserEmail,
                                                    UserIsActive = a.UserIsActive
                                                };

            return View(us_data.ToList());
        }
        [HttpPost]
        public IActionResult login([FromBody] User model)
        {
            HttpContext.Session.SetString("usersession", model.UserId.ToString());
            int check = 1;
            return Json(check);
        }

        public IActionResult Index()
        {
            //IQueryable<AccountViewModels> ac_data = from a in _context.Account
            //                                        where a.AcNumber == "1111111111"
            //                                        select new AccountViewModels
            //                                        {
            //                                            AcId = a.AcId,
            //                                            AcNumber = a.AcNumber,
            //                                            AcName = a.AcName,
            //                                            AcBalance = a.AcBalance,
            //                                            AcIsActive = a.AcIsActive
            //                                        };
            return View();
        }
        [HttpPost]
        public IActionResult Search([FromBody] AccountParam model)
        {
            var user_session = HttpContext.Session.GetString("usersession");
            IQueryable<AccountViewModels> json_data = from a in _context.Account
                                                     // where a.AcName.Contains(model.AcName) && a.UserId == Int32.Parse(user_session) || a.AcNumber.Contains(model.AcNumber) && a.UserId == Int32.Parse(user_session)
                                                     
                                                      where  a.UserId == Int32.Parse(user_session) && ( a.AcNumber.Contains(model.AcNumber) || a.AcName.Contains(model.AcName) )
                                                      select new AccountViewModels
                                                      {
                                                          AcId = a.AcId,
                                                          AcNumber = a.AcNumber,
                                                          AcName = a.AcName,
                                                          AcBalance = a.AcBalance,
                                                          AcIsActive = a.AcIsActive
                                                      };

            return Json(json_data.ToList());
        }

        [HttpPost]
        public IActionResult Edit([FromBody] AccountParam model)
        {

            IQueryable<AccountViewModels> json_data = from a in _context.Account
                                                      where a.AcId == model.AcId
                                                      select new AccountViewModels
                                                      {
                                                          AcId = a.AcId,
                                                          AcNumber = a.AcNumber,
                                                          AcName = a.AcName,
                                                          AcBalance = a.AcBalance,
                                                          AcIsActive = a.AcIsActive
                                                      };
            return Json(json_data.Single());
        }

        [HttpPost]
        public IActionResult Add([FromBody] AccountParam model)
        {
            var user_session = HttpContext.Session.GetString("usersession");
            int check = 1;
            IQueryable<Account> json_data = from a in _context.Account
                                            where a.AcNumber == model.AcNumber
                                            select new Account
                                            {
                                                AcId = a.AcId,
                                            };

            if (json_data.Count() == 0)
            {
                var ac = new Account()
                {
                    AcNumber = model.AcNumber,
                    AcBalance = 0,
                    AcName = model.AcName,
                    AcIsActive = model.AcIsActive,
                    UserId = Int32.Parse(user_session),
                };
                _context.Account.Add(ac);
                _context.SaveChanges();
            }
            else
            {
                check = 0;

            }

            return Json(check);
        }

        public IActionResult Update([FromBody] Account model)
        {
            //_context.Account.Attach(model);
            //EntityEntry<Account> entry = _context.Entry(model);
            //entry.State = EntityState.Modified;
            //_context.SaveChanges();



            // Update Statement
            var update = (from c in _context.Account
                          where c.AcId == model.AcId
                          select c).Single();

            update.AcName = model.AcName;
            update.AcIsActive = model.AcIsActive;
            update.AcNumber = model.AcNumber;
            _context.SaveChanges();


            //IQueryable<AccountViewModels> ac_data = from a in _context.Account
            //                                            //where a.isEnable == true
            //                                        select new AccountViewModels
            //                                        {
            //                                            AcId = a.AcId,
            //                                            AcNumber = a.AcNumber,
            //                                            AcName = a.AcName,
            //                                            AcBalance = a.AcBalance,
            //                                            AcIsActive = a.AcIsActive
            //                                        };
            int check = 1;
            return Json(check);
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
