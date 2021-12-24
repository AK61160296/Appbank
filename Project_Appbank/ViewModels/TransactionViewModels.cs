using Project_Appbank.Models.DBModels;
using System;
using System.Collections.Generic;

namespace Project_Appbank.ViewModels
{
    public class ShowTransacViewModel
    {
        public List<Account> Transaction { get; set; }

    }

    public class TransactionViewModels
    {
        public int TsId { get; set; }
        public int TsAcId { get; set; }
        public int TsAcDestinationId { get; set; }
        public decimal TsBalance { get; set; }
        public decimal TsMoney { get; set; }
        public string TsDetail { get; set; }
        public string name { get; set; }
        public string TsNote { get; set; }
        public int TsType { get; set; }
        public DateTime TsDate { get; set; }
        public string Date1 { get; set; }
    }

    public class TransactionParam
    {
       
        public int TsAcId { get; set; }
        //public int TsAcDestinationId { get; set; }
        //public decimal TsBalance { get; set; }
        public int UserId { get; set; }
        public decimal TsMoney { get; set; }
        public string TsDetail { get; set; }
        public string TsAD { get; set; }
        public int TsType { get; set; }
        public string TsNote { get; set; }
        public string date_begin { get; set; }
        public string date_end { get; set; }
        public string keyword { get; set; }

    }

    public class TransactionParamId
    {
        public int TsAcId { get; set; }
        public int UserId { get; set; }
    }
}
