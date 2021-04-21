using System.ComponentModel.DataAnnotations;

namespace Project_Appbank.ViewModels
{
    public class AccountParamId
    {
        public int AcId { get; set; }
        public string AcNumber { get; set; }
    }

    public class AccountParam
    {
        public int AcId { get; set; }
        [Required]
        public string AcNumber { get; set; }
        public decimal AcBalance { get; set; }
        public string AcName { get; set; }
        public sbyte AcIsActive { get; set; }

    }
    public class AccountViewModels
    {
        public int AcId { get; set; }
        public string AcNumber { get; set; }
        public decimal AcBalance { get; set; }
        public string AcName { get; set; }
        public sbyte AcIsActive { get; set; }
        public int UserId { get; set; }
    }
}
