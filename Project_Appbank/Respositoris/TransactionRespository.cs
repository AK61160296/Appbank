using Project_Appbank.Constant;
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
        private readonly AccountRespository accountRespository;
        private DateTime begin;
        private DateTime end;
        private string check_status;
        public TransactionRespository(appbankContext context, AccountRespository accountRespository)
        {
            this.accountRespository = accountRespository;
            this._context = context;
        }
        public List<TransactionViewModels> GetTransaction(int user_id, TransactionParam model)
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

                                                             b.UserId == user_id && 
                                                             model.keyword == "" && model.date_begin == "" && model.date_end == "" ||
                                                             model.keyword != "" && model.date_begin == "" && model.date_end == "" && (b.AcNumber.Contains(model.keyword) || a.TsDetail.Contains(model.keyword)) ||
                                                             model.date_begin != "" && a.TsDate >= begin && (b.AcNumber.Contains(model.keyword) || a.TsDetail.Contains(model.keyword)) ||
                                                             model.date_end != "" && a.TsDate <= end && (b.AcNumber.Contains(model.keyword) || a.TsDetail.Contains(model.keyword)) ||
                                                             a.TsDate >= begin && a.TsDate <= end && (b.AcNumber.Contains(model.keyword) || a.TsDetail.Contains(model.keyword))

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

        public string Transaction_DepositWithdraw(TransactionParam model)
        {
            using (var t = _context.Database.BeginTransaction())
            {
                try
                {
                    var check_balance = accountRespository.Getbalance(model.TsAcId);

                    if (model.TsType == Checktype.withdraw && check_balance >= model.TsMoney)
                    {
                        accountRespository.UpdateBalance(model.TsAcId, -model.TsMoney);
                        addDepositAndWithdraw(model);
                        check_status = Checkstatus.success;
                    }
                    else if (model.TsType == Checktype.depositor)
                    {
                        accountRespository.UpdateBalance(model.TsAcId, model.TsMoney);
                        addDepositAndWithdraw(model);
                        check_status = Checkstatus.success;
                    }
                    else
                    {
                        check_status = Checkstatus.error_balance;
                    }      
                    t.Commit();
                }
                catch (Exception ex)
                {
                    t.Rollback();
                    check_status = Checkstatus.error;
                }
            }
            return check_status;
        }

         void addDepositAndWithdraw(TransactionParam model)
        {
            var transaction_deposit = new Transaction()
            {
                TsAcId = model.TsAcId,
                TsBalance = accountRespository.Getbalance(model.TsAcId),
                TsDate = DateTime.Now,
                TsMoney = model.TsMoney,
                TsDetail = model.TsDetail,
                TsType = model.TsType,
            };
            _context.Transaction.Add(transaction_deposit);
            _context.SaveChanges();
        }

        public string Transaction_Transfer(TransactionParam model)
        {
            using (var t = _context.Database.BeginTransaction())
            {
                try
                {
                    if (accountRespository.Getbalance(model.TsAcId) >= model.TsMoney)
                    {
                        addTransferor(model);
                        addPayee(model);
                    }
                    t.Commit();
                    check_status = Checkstatus.success;
                }
                catch (Exception ex)
                {
                    t.Rollback();
                    check_status = Checkstatus.error;
                }
            }
            return check_status;
        }

         void addTransferor(TransactionParam model)
        {
            accountRespository.UpdateBalance(model.TsAcId, -model.TsMoney);
            var account_id = accountRespository.GetAccountId(model.TsAD);
            var balance_transfer = accountRespository.Getbalance(model.TsAcId);

            var transfer = new Transaction()
            {
                TsAcId = model.TsAcId,
                TsBalance = balance_transfer,
                TsDate = DateTime.Now,
                TsAcDestinationId = account_id,
                TsMoney = model.TsMoney,
                TsDetail = model.TsDetail,
                TsType = Checktype.transfer,
                TsNote = model.TsNote,
            };
            _context.Transaction.Add(transfer);
            _context.SaveChanges();
        }

         void addPayee(TransactionParam model)
        {
            var account_id = accountRespository.GetAccountId(model.TsAD);
            accountRespository.UpdateBalance(account_id, model.TsMoney);
            var balance_payee = accountRespository.Getbalance(account_id);

            var payee = new Transaction()
            {
                TsAcId = account_id,
                TsBalance = balance_payee,
                TsDate = DateTime.Now,
                TsAcDestinationId = model.TsAcId,
                TsMoney = model.TsMoney,
                TsDetail = "เงินเข้าบัญชี",
                TsType = Checktype.payee,
                TsNote = model.TsNote,
            };
            _context.Transaction.Add(payee);
            _context.SaveChanges();
        }

    }
}
