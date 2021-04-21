using Project_Appbank.Models.DBModels;
using Project_Appbank.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Project_Appbank.Respositoris
{
    public class TransactionRespository
    {
        private readonly appbankContext _context;
        private DateTime begin;
        private DateTime end;
        public TransactionRespository(appbankContext context)
        {
            this._context = context;
        }
        public List<TransactionViewModels> GetTransaction(int user_id,TransactionParam model)
        {
            if (model.date_begin != "")
            {
                begin = DateTime.Parse(model.date_begin.ToString());

            }
            if (model.date_end != "")
            {
                end = DateTime.Parse(model.date_end.ToString());
            }

            IQueryable<TransactionViewModels> queryResult = from a in _context.Transaction
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

            return queryResult.ToList();
        }

        public void Add_Deposit_Withdraw(decimal balance,TransactionParam model)
        {

            var transaction_withdraw = new Transaction()
            {
                TsAcId = model.TsAcId,
                TsBalance = balance,
                TsDate = DateTime.Now,
                TsMoney = model.TsMoney,
                TsDetail = model.TsDetail,
                TsType = model.TsType,
            };
            _context.Transaction.Add(transaction_withdraw);
            _context.SaveChanges();
          
        }

    }
}
