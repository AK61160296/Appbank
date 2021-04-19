namespace Project_Appbank.Models.DBModels
{
    public partial class User
    {
        public int UserId { get; set; }
        public string UserName { get; set; }
        public string UserEmail { get; set; }
        public int UserIsActive { get; set; }
    }
}
