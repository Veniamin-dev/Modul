using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace BackEndModul
{
    public class User
    {
        public User()
        {

        }
        public User(string usernaname, string password)
        {
            UserName = usernaname;
            _password = new Password(password);
        }

        public User(UserCredentials user)
        {
            UserName = user.UserName;
            _password = new Password(user.Password);
            Email = user.Email;
        }

        private Password _password;
        public string Email { get; set; }
        public string UserName { get; set; }
        public string Password => _password.PasswordHash;
        public string Salt => _password.Salt;
        public long AccountNumber { get; set; }
        public double Balance { get; set; }
    }
}
