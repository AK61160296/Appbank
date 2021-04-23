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
            var user_session = HttpContext.Session.GetString("usersession");
            var transaction_data = transactionRespository.GetTransaction(Int32.Parse(user_session), model);
            return Json(transaction_data);
        }

        public IActionResult Balance([FromBody] TransactionParamId model)
        {
            var balance = accountRespository.Getbalance(model.TsAcId);
            return Json(balance);
        }

        public IActionResult Option_account()
        {
            var user_session = HttpContext.Session.GetString("usersession");
            var account = accountRespository.GetOptionAccount(Int32.Parse(user_session));
            return Json(account.ToList());
        }

        public IActionResult Transfer([FromBody] TransactionParam model)
        {
            using (var t = _context.Database.BeginTransaction())
            {
                try
                {
                    var check_balace = accountRespository.Getbalance(model.TsAcId);
                    if (check_balace >= model.TsMoney)
                    {
                        //Update balance account transfer
                        accountRespository.UpdateBalance(model.TsAcId, -model.TsMoney);

                        var account_id = accountRespository.GetAccountId(model.TsAD);
                        accountRespository.UpdateBalance(account_id, model.TsMoney);
                        
                        var balance_transfer = accountRespository.Getbalance(model.TsAcId);
                        transactionRespository.Add_Transfer(account_id, model, balance_transfer);

                        //#region
                        var balance_payee = accountRespository.Getbalance(account_id);
                        transactionRespository.Add_Payee(account_id, model, balance_transfer);
                        
                        //#endregion
                        check_status = Checkstatus.success;
                    }
                    t.Commit();
                }
                catch (Exception ex)
                {
                    t.Rollback();
                    check_status = Checkstatus.error;
                }
            }

            return Json(check_status);
        }
        public IActionResult Deposit_Withdraw([FromBody] TransactionParam model)
        {

            using (var tran = _context.Database.BeginTransaction())
            {
                try
                {
                    var check_balance = accountRespository.Getbalance(model.TsAcId);
                    if (model.TsType == Checktype.withdraw && check_balance >= model.TsMoney)
                    {
                        //Update Balance withdraw
                        accountRespository.UpdateBalance(model.TsAcId, -model.TsMoney);           
                        //Get Now balance withdraw
                        var now_balance = accountRespository.Getbalance(model.TsAcId);
                        transactionRespository.Add_Deposit_Withdraw(now_balance, model);
                      
                        check_status = Checkstatus.success;
                    }
                    else
                    {
                        //Update Balance depositor
                        accountRespository.UpdateBalance(model.TsAcId, model.TsMoney);
                        //Get Now balance depositor
                        var now_balance = accountRespository.Getbalance(model.TsAcId);
                        transactionRespository.Add_Deposit_Withdraw(now_balance, model);
                      
                        check_status = Checkstatus.success;
                    }
                    tran.Commit();
                }
                catch (Exception ex)
                {
                    tran.Rollback();
                    check_status = Checkstatus.error;
                }
            }

            return Json(check_status);
        }
    }

}
