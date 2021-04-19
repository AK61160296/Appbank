using Microsoft.AspNetCore.Mvc;
using Project_Appbank.Models.DBModels;

namespace Project_Appbank.Controllers
{
    public class UserController : Controller
    {
        private readonly appbankContext _context;

        public UserController(appbankContext context)
        {
            this._context = context;
        }

    }
}
