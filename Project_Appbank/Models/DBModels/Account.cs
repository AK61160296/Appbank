using System.Collections.Generic;

namespace Project_Appbank.Models.DBModels
{
    public partial class Account
    {
        public Account()
        {
            Transaction = new HashSet<Transaction>();
        }

        public int AcId { get; set; }
        public string AcNumber { get; set; }
        public decimal AcBalance { get; set; }
        public string AcName { get; set; }
        public sbyte AcIsActive { get; set; }
        public int UserId { get; set; }

        public virtual ICollection<Transaction> Transaction { get; set; }
    }
}
