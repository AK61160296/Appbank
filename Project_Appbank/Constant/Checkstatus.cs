using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Project_Appbank.Constant
{
    public static class Checkstatus
    {
        public const string success = "success";
        public const string successDeposit = "successdeposit";
        public const string successWithdraw = "successwithdraw";
        public const string duplicate_data = "duplicate";
        public const string error = "error";
        public const string error_balance = "error_balance";
    }
    public static class Checktype
    {
        public const int payee = 4;
        public const int transfer = 3;
        public const int withdraw = 2;
        public const int depositor = 1;
    }
}
