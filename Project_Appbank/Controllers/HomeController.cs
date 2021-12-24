using Microsoft.AspNetCore.Http;
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
using Project_Appbank.Respositoris;

namespace Project_Appbank.Controllers
{
    public class HomeController : Controller
    {
        private appbankContext _context;
        private readonly AccountRespository accountRespository;
        private readonly UserResponsitory UserResponsitory;
       
        public HomeController(appbankContext context,AccountRespository accountRespository,UserResponsitory userResponsitory)
        {
            this._context = context;
            this.accountRespository = accountRespository;
            this.UserResponsitory = userResponsitory;
        }
        public IActionResult Userlist()
        {
         
            var user_data =  UserResponsitory.Getusers();   
            return View(user_data);
        }
        [HttpPost]
        public IActionResult login([FromBody] User model)
        {
            HttpContext.Session.SetString("usersession", model.UserId.ToString());
            var user_session = HttpContext.Session.GetString("usersession");
            return Json(user_session);
        }

        public IActionResult Index()
        {
            return View();
        }

        [HttpPost]
        public IActionResult Search([FromBody] AccountParam model)
        {
            //if (!ModelState.IsValid)
            //{

            //    return BadRequest(ModelState);
            //}
            //var user_session = HttpContext.Session.GetString("usersession");
            //var account_data = accountRespository.GetAccounts(1, model);
            var account_data = accountRespository.GetAccounts(model);
            return Json(account_data);
        }

        [HttpPost]
        public IActionResult Edit([FromBody] AccountParamId model)
        {
            var account = accountRespository.GetAccount(model.AcId);
            return Json(account);
        }

        [HttpPost]
        public IActionResult Add([FromBody] AccountParam model)
        {
            string check_status;
            var user_session = HttpContext.Session.GetString("usersession");
            check_status = accountRespository.Add(model);
            return Json(check_status);
        }

        public IActionResult Update([FromBody] AccountParam model)
        {
            accountRespository.Update(model);
            return Json(Checkstatus.success);
        }

        public IActionResult updateStatus([FromBody] AccountParam model)
        {
            accountRespository.updateStatus(model);
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
