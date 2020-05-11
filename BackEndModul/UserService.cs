using Dapper;
using Npgsql;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace BackEndModul
{
    public interface IUserService
    {
        bool IsValidUser(string username, string password);
        void CreateUser(UserCredentials user);
        bool CheckUniqueNameAndEmail(String username, String email);
        bool isValidEmail(string email);
        User GetUser(double accountnumber);
        void TopUpAccount(User user, double howmuch);
        bool Transfer(User userFrom, User userTo, double howmuch);
    }
    public class UserService : IUserService
    {
        public bool IsValidUser(string email, string password)
        {
            var testSalt = GetSalt(email);
            var testPasswordHash = GetPasswordHash(email);

            if (testSalt != null && testPasswordHash != null && Password.CheckPassword(password, testSalt, testPasswordHash))
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        public bool isValidEmail(string email)
        {
            String pattern = @"(\w+)@(\w+)\.ru";
            return Regex.IsMatch(email, pattern, RegexOptions.IgnoreCase);
        }

        public bool Transfer(User userFrom, User userTo, double howmuch)
        {
            if (userFrom.Balance >= howmuch)
            {
                using (var conn = CreateConnection())
                {
                    conn.Open();

                    conn.Execute("UPDATE users SET balance = @balance WHERE accountNumber = @AccountNumber;",
                        new
                        {
                            AccountNumber = userFrom.AccountNumber,
                            Balance = userFrom.Balance - howmuch
                        });

                    conn.Execute("UPDATE users SET balance = @balance WHERE accountNumber = @AccountNumber;",
                        new
                        {
                            AccountNumber = userTo.AccountNumber,
                            Balance = userTo.Balance + howmuch
                        });

                    return true;
                }
            }
            else
            {
                return false;
            }
        }

        public void TopUpAccount(User user, double howmuch)
        {
            double newBalance = user.Balance + howmuch;
            using (var conn = CreateConnection())
            {
                conn.Open();

                conn.Execute("UPDATE users SET balance = @balance WHERE accountNumber = @AccountNumber;",
                    new
                    {
                        AccountNumber = user.AccountNumber,
                        Balance = newBalance
                    });
            }
        }

        public User GetUser(double accountnumber)
        {
            using (var conn = CreateConnection())
            {
                conn.Open();

                var result = conn.QuerySingleOrDefault<User>("SELECT accountNumber, balance FROM users WHERE accountNumber = @accountnumber;",
                new
                {
                    accountnumber,
                });

                return result;
            }
        }

        private string GetSalt(string email)
        {
            using (var conn = CreateConnection())
            {
                conn.Open();

                var result = conn.QuerySingleOrDefault<string>("SELECT salt FROM users WHERE email = @email;",
                new
                {
                    email,
                });

                return result;
            }
        }

        private string GetPasswordHash(string email)
        {
            using (var conn = CreateConnection())
            {
                conn.Open();

                var result = conn.QuerySingleOrDefault<string>("SELECT passwordhash FROM users WHERE email = @email;",
                new
                {
                    email,
                });

                return result;
            }
        }

        public void CreateUser(UserCredentials user)
        {
            using (var conn = CreateConnection())
            {
                conn.Open();

                User newuser = new User(user);

                conn.Execute("INSERT INTO users (username, email, passwordHash, salt, accountNumber, balance) VALUES(@username, @email, @passwordHash, @salt, @accountNumber, @balance);",
                    new
                    {
                        username = newuser.UserName,
                        email = newuser.Email,
                        passwordHash = newuser.Password,
                        salt = newuser.Salt,
                        accountNumber = GenerateAccountNumber(),
                        balance = 0
                    });
            }
        }

        public bool CheckUniqueNameAndEmail(String username, String email)
        {
            using (var conn = CreateConnection())
            {
                conn.Open();

                var checkUniqueName = conn.QuerySingleOrDefault<User>("SELECT username FROM users WHERE username = @Username;",
                    new
                    {
                        username,
                    });

                var checkUniqueEmail = conn.QuerySingleOrDefault<User>("SELECT email FROM users WHERE email = @Email;",
                    new
                    {
                        email,
                    });

                if (checkUniqueName == null && checkUniqueEmail == null)
                {
                    return true;
                }
                else
                {
                    return false;
                }
            }
        }

        private bool CheckUniqueAccountNumber(long number)
        {
            using (var conn = CreateConnection())
            {
                conn.Open();

                var checkUniqueAccountNumber = conn.QuerySingleOrDefault<User>("SELECT accountNumber FROM users WHERE accountNumber = @number;",
                    new
                    {
                        number,
                    });

                if (checkUniqueAccountNumber != null)
                {
                    return false;
                }
                else
                {
                    return true;
                }
            }
        }

        private long GenerateAccountNumber()
        {
            Random rnd = new Random();
            long newAccountNumber = 0;
            bool uniqueAccountNumber = false;

            while (uniqueAccountNumber == false)
            {
                newAccountNumber = 4;
                for (int i = 0; i < 9; i++)
                {
                    newAccountNumber = newAccountNumber * 10 + rnd.Next(0, 10);
                }

                uniqueAccountNumber = CheckUniqueAccountNumber(newAccountNumber);
            }

            return newAccountNumber;
        }

        private NpgsqlConnection CreateConnection()
        {
            var connection = new NpgsqlConnection($"server=localhost;database=Data1;userid=postgres;password=1212;Pooling=false");

            return connection;
        }

    }
}
