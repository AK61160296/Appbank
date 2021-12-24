using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Project_Appbank.Models.DBModels;
using Project_Appbank.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using Project_Appbank.Constant;
using Project_Appbank.Respositoris;

namespace Project_Appbank.Controllers
{
    public class TransactionController : Controller
    {
        private appbankContext _context;
        private readonly AccountRespository accountRespository;
        private readonly TransactionRespository transactionRespository;
        private string check_status;
        public TransactionController(appbankContext context, AccountRespository accountRespository, TransactionRespository transactionRespository)
        {
            this._context = context;
            this.accountRespository = accountRespository;
            this.transactionRespository = transactionRespository;
        }

        public IActionResult Tran()
        {
            return View();
        }

        [HttpPost]
        public IActionResult Search([FromBody] TransactionParam model)
        {
            //var user_session = HttpContext.Session.GetString("usersession");
            var transaction_data = transactionRespository.GetTransaction(model);
            return Json(transaction_data);
        }

        public IActionResult Balance([FromBody] TransactionParamId model)
        {
            var balance = accountRespository.Getbalance(model.TsAcId);
            return Json(balance);
        }

        public IActionResult Option_account([FromBody] TransactionParamId model)
        {
            var user_session = HttpContext.Session.GetString("usersession");
            var account = accountRespository.GetOptionAccount(model.UserId);
            return Json(account.ToList());
        }

        public IActionResult Transfer([FromBody] TransactionParam model)
        {
            check_status = transactionRespository.Transaction_Transfer(model);
            return Json(check_status);
        }

        public IActionResult Deposit_Withdraw([FromBody] TransactionParam model)
        {

            check_status = transactionRespository.Transaction_DepositWithdraw(model);

            return Json(check_status);
        }
    }

}
