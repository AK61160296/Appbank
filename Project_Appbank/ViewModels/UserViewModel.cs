using Project_Appbank.Models.DBModels;
using System.Collections.Generic;

namespace Project_Appbank.ViewModels
{
    public class UserViewModel
    {
        public UserViewModel()
        {
            Account = new HashSet<Account>();
        }

        public int UserId { get; set; }
        public string UserName { get; set; }
        public string UserEmail { get; set; }
        public int UserIsActive { get; set; }

        public virtual ICollection<Account> Account { get; set; }
    }
}
