using System;

namespace Project_Appbank.Models.DBModels
{
    public partial class Transaction
    {
        public int TsId { get; set; }
        public int TsAcId { get; set; }
        public int TsAcDestinationId { get; set; }
        public decimal TsBalance { get; set; }
        public decimal TsMoney { get; set; }
        public string TsDetail { get; set; }
        public string TsNote { get; set; }
        public int TsType { get; set; }

        public string Date => $"{TsDate.ToString("MM/dd/yyyy")}";
        public string date_search => $"{TsDate.ToString("yyyy/MM/dd")}";
        public DateTime TsDate { get; set; }

        public virtual Account TsAc { get; set; }
    }
}
