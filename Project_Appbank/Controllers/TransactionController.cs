using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Project_Appbank.Models.DBModels;
using Project_Appbank.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Project_Appbank.Controllers
{
    public class TransactionController : Controller
    {
        private appbankContext _context;
        private DateTime begin;
        private DateTime end;
        public TransactionController(appbankContext context)
        {
            this._context = context;
        }

        public IActionResult Tran()
        {
            return View();
        }

        [HttpPost]
        public IActionResult Search([FromBody] TransactionParam model)
        {
            var user_session = HttpContext.Session.GetString("usersession");
            var user_id = Int16.Parse(user_session);
            if (model.date_begin != "")
            {
                begin = DateTime.Parse(model.date_begin.ToString());

            }
            if (model.date_end != "")
            {
                end = DateTime.Parse(model.date_end.ToString());
            }

            IQueryable<TransactionViewModels> json_data = from a in _context.Transaction
                                                          join b in _context.Account on a.TsAcId equals b.AcId
                                                          where

                                                          b.UserId == user_id && ((model.keyword == "" && model.date_begin == "" && model.date_end == "") ||
                                                          (model.keyword != "" && (b.AcNumber.Contains(model.keyword) || a.TsDetail.Contains(model.keyword))) ||
                                                          (model.keyword != "" && model.date_begin != "" && model.date_end == "" && (b.AcNumber.Contains(model.keyword) || a.TsDetail.Contains(model.keyword) && a.TsDate >= begin)) ||
                                                          (model.keyword != "" && model.date_begin == "" && model.date_end != "" && (b.AcNumber.Contains(model.keyword) && a.TsDate >= begin || a.TsDetail.Contains(model.keyword) && a.TsDate <= end)) ||
                                                          (model.keyword != "" && model.date_begin != "" && model.date_end != "" && (b.AcNumber.Contains(model.keyword) || a.TsDetail.Contains(model.keyword) && a.TsDate >= begin && a.TsDate <= end)) ||
                                                          (model.keyword == "" && model.date_begin != "" && model.date_end == "" && (a.TsDate >= begin)) ||
                                                          (model.keyword == "" && model.date_begin == "" && model.date_end != "" && (a.TsDate <= end)) ||
                                                          (model.keyword == "" && model.date_begin != "" && model.date_end != "" && model.date_end != "" && (a.TsDate >= begin && a.TsDate <= end))
                                                          )

                                                         //b.AcNumber.Contains(model.keyword) && b.UserId == user_id && model.keyword == "" && model.date_begin == "" && model.date_end == "" ||
                                                          //model.keyword != "" && model.date_begin == "" && model.date_end == "" && b.AcNumber.Contains(model.keyword) && b.UserId == user_id ||
                                                          //model.keyword != "" && model.date_begin == "" && model.date_end == "" && a.TsDetail.Contains(model.keyword) && b.UserId == user_id ||
                                                          //model.keyword != "" && model.date_begin != "" && model.date_end == "" && b.AcNumber.Contains(model.keyword) && b.UserId == user_id && a.TsDate >= begin ||
                                                          //model.keyword != "" && model.date_begin != "" && model.date_end == "" && a.TsDetail.Contains(model.keyword) && b.UserId == user_id && a.TsDate >= begin ||
                                                          //model.keyword != "" && model.date_begin == "" && model.date_end != "" && b.AcNumber.Contains(model.keyword) && b.UserId == user_id && a.TsDate <= end ||
                                                          //model.keyword != "" && model.date_begin == "" && model.date_end != "" && a.TsDetail.Contains(model.keyword) && b.UserId == user_id && a.TsDate <= end ||
                                                          //model.keyword != "" && model.date_begin != "" && model.date_end != "" && b.AcNumber.Contains(model.keyword) && b.UserId == user_id && a.TsDate >= begin && a.TsDate <= end ||
                                                          //model.keyword != "" && model.date_begin != "" && model.date_end != "" && a.TsDetail.Contains(model.keyword) && b.UserId == user_id && a.TsDate >= begin && a.TsDate <= end ||
                                                          //model.date_begin != "" && model.date_end == "" && model.keyword == "" && a.TsDate >= begin && b.UserId == user_id ||
                                                          //model.date_begin == "" && model.date_end != "" && model.keyword == "" && a.TsDate <= end && b.UserId == user_id ||
                                                          //model.date_begin != "" && model.date_end != "" && model.keyword == "" && a.TsDate >= begin && a.TsDate <= end && b.UserId == user_id

                                                          orderby a.TsId
                                                          select new TransactionViewModels
                                                          {
                                                              TsId = a.TsId,
                                                              name = b.AcNumber,
                                                              TsAcId = a.TsAcId,
                                                              Date1 = a.Date,
                                                              TsAcDestinationId = a.TsAcDestinationId,
                                                              TsBalance = a.TsBalance,
                                                              TsMoney = a.TsMoney,
                                                              TsDetail = a.TsDetail,
                                                              TsType = a.TsType,
                                                          };
            return Json(json_data.ToList());
        }

        public IActionResult Balance([FromBody] TransactionParamId model)
        {
             
            var balance = from a in _context.Account
                          where a.AcId == model.TsAcId
                          select a.AcBalance;
            return Json(balance.FirstOrDefault());
        }

        public IActionResult Option_account()
        {
            var user_session = HttpContext.Session.GetString("usersession");
            IQueryable<AccountViewModels> json_data = from a in _context.Account
                                                      where a.UserId == Int16.Parse(user_session)
                                                      select new AccountViewModels
                                                      {
                                                        AcId = a.AcId,
                                                        AcNumber = a.AcNumber,
                                                      };
            return Json(json_data.ToList());
        }

        public IActionResult Transfer([FromBody] TransactionParam model)
        {
            int check_status = 0;

            using (var t = _context.Database.BeginTransaction())
            {
                try
                {
                        var check_balace = (from a in _context.Account
                                             where a.AcId == model.TsAcId
                                             select a.AcBalance).FirstOrDefault();

                    if (check_balace >= model.TsMoney)
                    {

                        var account_id = (from a in _context.Account
                                             where a.AcNumber == model.TsAD
                                             select a.AcId).FirstOrDefault();

                        //get account data by account_id
                        var account_data = (from c in _context.Account
                                             where c.AcId == account_id
                                             select c).Single();
                        account_data.AcBalance += model.TsMoney;
                        _context.SaveChanges();// update balance

                        //get account data by ts_acid
                        var account_data1 = (from c in _context.Account
                                             where c.AcId == model.TsAcId
                                             select c).Single();
                        account_data1.AcBalance -= model.TsMoney;
                        _context.SaveChanges(); // update balance

                        var transfer_data = new Transaction()
                        {
                            TsAcId = model.TsAcId,
                            TsBalance = check_balace,
                            TsDate = DateTime.Now,
                            TsAcDestinationId = account_id,
                            TsMoney = model.TsMoney,
                            TsDetail = model.TsDetail,
                            TsType = model.TsType,
                            TsNote = model.TsNote,
                        };
                            _context.Transaction.Add(transfer_data);
                            _context.SaveChanges();

                        #region

                        var now_balance = (from a in _context.Account
                                           where a.AcId == account_id
                                           select a.AcBalance).FirstOrDefault();

                        var transfer_data_target = new Transaction()
                        {
                            TsAcId = account_id,
                            TsBalance = now_balance,
                            TsDate = DateTime.Now,
                            TsAcDestinationId = model.TsAcId,
                            TsMoney = model.TsMoney,
                            TsDetail = "เงินเข้าบัญชี",
                            TsType = 2,
                            TsNote = model.TsNote,
                        };
                            _context.Transaction.Add(transfer_data_target);
                            _context.SaveChanges();
                        #endregion
                        check_status = 1;
                    }
                    t.Commit();
                }
                catch (Exception ex)
                {
                    t.Rollback();
                    check_status = 2;
                }
            }

            return Json(check_status);
        }
        public IActionResult Deposit_Withdraw([FromBody] TransactionParam model)
        {

            int check_status = 0;

            using (var tran = _context.Database.BeginTransaction())
            {
                try
                {
                        var check_balance = (from a in _context.Account
                                         where a.AcId == model.TsAcId
                                         select a.AcBalance).FirstOrDefault();

                    if (model.TsType == 2 && check_balance >= model.TsMoney)
                    {
                        var balance_withdraw = (from c in _context.Account
                                              where c.AcId == model.TsAcId
                                              select c).Single();

                            balance_withdraw.AcBalance -= model.TsMoney;
                            _context.SaveChanges();

                        var now_balance = (from a in _context.Account
                                           where a.AcId == model.TsAcId
                                           select a.AcBalance).FirstOrDefault();

                        var transaction_withdraw = new Transaction()
                        {
                            TsAcId = model.TsAcId,
                            TsBalance = now_balance,
                            TsDate = DateTime.Now,
                            TsMoney = model.TsMoney,
                            TsDetail = model.TsDetail,
                            TsType = model.TsType,
                        };
                            _context.Transaction.Add(transaction_withdraw);
                            _context.SaveChanges();
                            check_status = 1;
                    }
                    else
                    {
                        var balance_depositor = (from c in _context.Account
                                                 where c.AcId == model.TsAcId
                                                 select c).Single();
                            balance_depositor.AcBalance += model.TsMoney;
                            _context.SaveChanges();

                        var now_balance = (from a in _context.Account
                                            where a.AcId == model.TsAcId
                                            select a.AcBalance).FirstOrDefault();

                        var transaction_depositor = new Transaction()
                        {
                            TsAcId = model.TsAcId,
                            TsBalance = now_balance,
                            TsDate = DateTime.Now,
                            TsMoney = model.TsMoney,
                            TsDetail = model.TsDetail,
                            TsType = model.TsType,
                        };

                            _context.Transaction.Add(transaction_depositor);
                            _context.SaveChanges();
                            check_status = 1;
                    }
                    tran.Commit();
                }
                catch(Exception ex)
                {
                    tran.Rollback();
                    check_status = 2;
                }
            }

            return Json(check_status);
        }
    }

}
