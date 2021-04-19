﻿using Microsoft.AspNetCore.Mvc;
using Project_Appbank.Models.DBModels;
using Project_Appbank.ViewModels;
using System.Collections.Generic;
using System.Linq;

namespace Project_Appbank.respositoris
{
    public class UserResponsitory : Controller
    {
        private readonly appbankContext context;

        public UserResponsitory(appbankContext context)
        {
            this.context = context;
        }

        public List<UserViewModel> Getuser()
        {
            IQueryable<UserViewModel> us_data = from a in context.User
                                                    //where a.isEnable == true
                                                select new UserViewModel
                                                {
                                                    UserId = a.UserId,
                                                    UserName = a.UserName,
                                                    UserEmail = a.UserEmail,
                                                    UserIsActive = a.UserIsActive
                                                };
            return us_data.ToList();
        }
    }
}
