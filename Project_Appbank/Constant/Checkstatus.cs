using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Project_Appbank.Constant
{
    public static class Checkstatus
    {
        public const string success = "success";
        public const string duplicate_data = "duplicate";
        public const string error = "error";
        public const string error_blance = "error";
    }
    public static class Checktype
    {
        public const int tranfered = 1;
        public const int withdraw = 2;
        public const int depositor = 1;
    }
}
