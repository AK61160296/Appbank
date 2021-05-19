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
        private DateTime begin;
        private DateTime end;
        public TransactionRespository(appbankContext context)
        {
            this._context = context;
        }
        public List<TransactionViewModels> GetTransaction(TransactionParam model)
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

                                                            //b.UserId == user_id && 
                                                            //((model.keyword != "" && (b.AcNumber.Contains(model.keyword) || a.TsDetail.Contains(model.keyword))) ||
                                                            //model.date_begin != "" && model.date_end != "" && a.TsDate >= begin && a.TsDate <= end )

                                                            b.UserId == model.UserId &&
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
                string check_status;
                try
                {
                    var check_balance = Getbalance(model.TsAcId);

                    if (model.TsType == Checktype.withdraw && check_balance >= model.TsMoney)
                    {
                        UpdateBalance(model.TsAcId, -model.TsMoney);
                        addDepositAndWithdraw(model);
                        check_status = Checkstatus.success;
                    }
                    else if (model.TsType == Checktype.depositor)
                    {
                        UpdateBalance(model.TsAcId, model.TsMoney);
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
                return check_status;
            }

        }

        void addDepositAndWithdraw(TransactionParam model)
        {
            var transaction_deposit = new Transaction()
            {
                TsAcId = model.TsAcId,
                TsBalance = Getbalance(model.TsAcId),
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
                string check_status;
                try
                {
                    if (Getbalance(model.TsAcId) >= model.TsMoney)
                    {
                        var account_payee = GetAccountId(model.TsAD);

                        UpdateBalance(model.TsAcId, -model.TsMoney);
                        var balance_transfer = Getbalance(model.TsAcId);
                        addTransferor(model, account_payee, balance_transfer);

                        UpdateBalance(account_payee, model.TsMoney);
                        var balance_payee = Getbalance(account_payee);
                        addPayee(model, account_payee, balance_transfer);
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
                return check_status;
            }

        }

        void addTransferor(TransactionParam model, int account_payee, decimal balance_transfer)
        {
            var transfer = new Transaction()
            {
                TsAcId = model.TsAcId,
                TsBalance = balance_transfer,
                TsDate = DateTime.Now,
                TsAcDestinationId = account_payee,
                TsMoney = model.TsMoney,
                TsDetail = model.TsDetail,
                TsType = Checktype.transfer,
                TsNote = model.TsNote,
            };
            _context.Transaction.Add(transfer);
            _context.SaveChanges();
        }

        void addPayee(TransactionParam model, int account_payee, decimal balance_payee)
        {

            var payee = new Transaction()
            {
                TsAcId = account_payee,
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

        decimal Getbalance(int account_id)
        {

            var queryResult = (from a in _context.Account
                               where a.AcId == account_id
                               select a.AcBalance).Single();
            return queryResult;
        }

        int GetAccountId(string account_number)
        {
            var queryResult = (from a in _context.Account
                               where a.AcNumber == account_number
                               select a.AcId).SingleOrDefault();
            return queryResult;
        }

        void UpdateBalance(int account_id, decimal monney)
        {
            var account_data = (from c in _context.Account
                                where c.AcId == account_id
                                select c).Single();
            account_data.AcBalance += monney;
            _context.SaveChanges();
        }

    }
}
