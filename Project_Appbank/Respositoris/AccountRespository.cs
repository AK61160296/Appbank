using Microsoft.AspNetCore.Http;
using Project_Appbank.Models.DBModels;
using Project_Appbank.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Project_Appbank.Respositoris
{
    public class AccountRespository
    {
        private readonly appbankContext _context;

        public AccountRespository(appbankContext context)
        {
            this._context = context;
        }
        public decimal Getbalance(int account_id)
        {

            var queryResult = (from a in _context.Account
                                          where a.AcId == account_id
                                          select a.AcBalance).Single();
            return queryResult;
        }

        public int GetAccountId(string account_number)
        {
            var queryResult = (from a in _context.Account
                               where a.AcNumber == account_number
                               select a.AcId).FirstOrDefault();
            return queryResult;
        }

        public void UpdateBalance(int account_id,decimal monney)
        {
            var account_data = (from c in _context.Account
                                where c.AcId == account_id
                                select c).Single();
            account_data.AcBalance += monney;
            _context.SaveChanges();
        }

        public List<AccountViewModels> GetOptionAccount(int user_id)
        {

            IQueryable<AccountViewModels> queryResult = from a in _context.Account
                                                        where a.UserId == user_id
                                                        select new AccountViewModels
                                                        {
                                                            AcId = a.AcId,
                                                            AcNumber = a.AcNumber
                                                         };
            return queryResult.ToList();
        }
         
        public List<AccountViewModels> GetAccounts(int user_id,AccountParam model)
        {
            IQueryable<AccountViewModels> queryResult = from a in _context.Account
                                                     where a.UserId == user_id && (a.AcNumber.Contains(model.AcNumber) || a.AcName.Contains(model.AcName))
                                                     select new AccountViewModels
                                                     {
                                                         AcId = a.AcId,
                                                         AcNumber = a.AcNumber,
                                                         AcName = a.AcName,
                                                         AcBalance = a.AcBalance,
                                                         AcIsActive = a.AcIsActive
                                                     };
            return queryResult.ToList();
        }
        public AccountViewModels GetAccount(int account_id)
        {
            IQueryable<AccountViewModels> queryResult = from a in _context.Account
                                                    where a.AcId == account_id
                                                    select new AccountViewModels
                                                    {
                                                        AcId = a.AcId,
                                                        AcNumber = a.AcNumber,
                                                        AcName = a.AcName,
                                                        AcBalance = a.AcBalance,
                                                        AcIsActive = a.AcIsActive
                                                    };
            return queryResult.Single();
        }

        public void Add(int user_id,AccountParam model) 
        {
            var account = new Account()
            {
                AcNumber = model.AcNumber,
                AcBalance = 0,
                AcName = model.AcName,
                AcIsActive = model.AcIsActive,
                UserId = user_id,
            };
            _context.Account.Add(account);
            _context.SaveChanges();

        }
        public void Update(AccountParam model)
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
        }

    }

}
