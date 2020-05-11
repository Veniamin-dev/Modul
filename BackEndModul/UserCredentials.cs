using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace BackEndModul
{
    public class UserCredentials
    {
        public string Email { get; set; }

        public string Password { get; set; }

        public string UserName { get; set; }

        public string PasswordConfirm { get; set; }

        public double HowMuch { get; set; }

        public long FromAccount { get; set; }

        public long ToAccount { get; set; }
    }
}
