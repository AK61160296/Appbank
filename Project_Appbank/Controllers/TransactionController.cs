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

        public IActionResult Balance([FromBody] TransactionParam model)
        {

            var balance = from a in _context.Account
                          where a.AcId == model.TsAcId
                          select a.AcBalance;

            return Json(balance.First());
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
            int check_swal = 0;


            using (var t = _context.Database.BeginTransaction())
            {
                try
                {
                    var check_balace = (from a in _context.Account
                                        where a.AcId == model.TsAcId
                                        select a.AcBalance).First();

                    if (check_balace >= model.TsMoney)
                    {

                        var account_transfer_id = (from a in _context.Account
                                                   where a.AcNumber == model.TsAD
                                                   select a.AcId).First();

                        var account_data = (from c in _context.Account
                                            where c.AcId == account_transfer_id
                                            select c).Single();
                        account_data.AcBalance += model.TsMoney;
                        _context.SaveChanges();

                        int zero = 0;
                        var aa = 1 / zero;
                        
                        var account_data1 = (from c in _context.Account
                                             where c.AcId == model.TsAcId
                                             select c).Single();
                        account_data1.AcBalance -= model.TsMoney;
                        _context.SaveChanges();

                        var transfer_data = new Transaction()
                        {
                            TsAcId = model.TsAcId,
                            TsBalance = check_balace,
                            TsDate = DateTime.Now,
                            TsAcDestinationId = account_transfer_id,
                            TsMoney = model.TsMoney,
                            TsDetail = model.TsDetail,
                            TsType = model.TsType,
                            TsNote = model.TsNote,
                        };
                        _context.Transaction.Add(transfer_data);
                        _context.SaveChanges();

                        #region

                        var now_balance = (from a in _context.Account
                                           where a.AcId == account_transfer_id
                                           select a.AcBalance).First();

                        var transfer_data_target = new Transaction()
                        {
                            TsAcId = account_transfer_id,
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
                        check_swal = 1;
                    }
                    t.Commit();
                }
                catch (Exception ex)
                {
                    t.Rollback();
                    check_swal = 0;
                }
            }

            return Json(check_swal);
        }
        public IActionResult Deposit_Withdraw([FromBody] TransactionParam model)
        {

            int check_swal = 0;

            var check_balance = (from a in _context.Account
                                 where a.AcId == model.TsAcId
                                 select a.AcBalance).First();


            if (model.TsType == 2 && check_balance >= model.TsMoney)
            {
                var update_balance = (from c in _context.Account
                                      where c.AcId == model.TsAcId
                                      select c).Single();

                update_balance.AcBalance -= model.TsMoney;
                _context.SaveChanges();

                var now_balance = (from a in _context.Account
                                   where a.AcId == model.TsAcId
                                   select a.AcBalance).First();

                var transaction_Deposit = new Transaction()
                {
                    TsAcId = model.TsAcId,
                    TsBalance = now_balance,
                    TsDate = DateTime.Now,
                    TsMoney = model.TsMoney,
                    TsDetail = model.TsDetail,
                    TsType = model.TsType,
                };
                _context.Transaction.Add(transaction_Deposit);
                _context.SaveChanges();
                check_swal = 1;
            }
            else
            {
                var update_balance = (from c in _context.Account
                                      where c.AcId == model.TsAcId
                                      select c).Single();
                update_balance.AcBalance += model.TsMoney;
                _context.SaveChanges();

                var now_balance = (from a in _context.Account
                                   where a.AcId == model.TsAcId
                                   select a.AcBalance).First();

                var transaction_Withdraw = new Transaction()
                {
                    TsAcId = model.TsAcId,
                    TsBalance = now_balance,
                    TsDate = DateTime.Now,
                    TsMoney = model.TsMoney,
                    TsDetail = model.TsDetail,
                    TsType = model.TsType,
                };

                _context.Transaction.Add(transaction_Withdraw);
                _context.SaveChanges();
                check_swal = 1;
            }

            return Json(check_swal);
        }
    }

}
