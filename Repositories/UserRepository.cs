using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Net;
using System.Runtime.InteropServices;
using System.Security.Cryptography;
using System.Security;
using System.Security.Principal;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Documents;
using PaintShopManagement.Models;

namespace PaintShopManagement.Repositories
{
    public class UserRepository : RepositoryBase, IUserRepository
    {

        public void Add(UserModel userModel)
        {
            throw new NotImplementedException();
        }

        public bool AuthenticateUser(NetworkCredential credential)
        {
            bool validUser;
            using (var connection = GetConnection())
            using (var command = new SqlCommand())
            {
                connection.Open();
                command.Connection = connection;
                command.CommandText = "select * from Users where userName=@username";
                command.Parameters.Add("@username", SqlDbType.NVarChar).Value = credential.UserName;

                using (var reader = command.ExecuteReader())
                {
                    if (reader.Read())
                    {
                        string storedHashedPassword = reader["password"].ToString(); // Retrieve the stored hashed password from the database
                        string enteredPassword = credential.Password;

                        // Compare the entered password with the stored hashed password
                        validUser = VerifyPassword(enteredPassword, storedHashedPassword);
                    }
                    else
                    {
                        validUser = false;
                    }
                }
            }
            return validUser;
        }

        private bool VerifyPassword(string enteredPassword, string storedHashedPassword)
        {
            // Extract the salt and hash from the stored hashed password
            byte[] storedHashBytes = Convert.FromBase64String(storedHashedPassword);
            byte[] salt = storedHashBytes.Take(16).ToArray();
            byte[] storedHash = storedHashBytes.Skip(16).ToArray();

            // Hash the entered password with the retrieved salt
            using (var pbkdf2 = new Rfc2898DeriveBytes(enteredPassword, salt, 10000))
            {
                byte[] enteredHash = pbkdf2.GetBytes(32); // 32 bytes for a 256-bit key

                // Compare the entered hash with the stored hash
                return enteredHash.SequenceEqual(storedHash);
            }
        }

        public void Delete(int Id)
        {
            throw new NotImplementedException();
        }

        public IEnumerable<UserModel> GetByAll()
        {
            throw new NotImplementedException();
        }

        public UserModel GetById(int Id)
        {
            throw new NotImplementedException();
        }

        public UserModel GetByUsername(string username)
        {
            UserModel user = null;
            using (var connection = GetConnection())
            using (var command = new SqlCommand())
            {
                connection.Open();
                command.Connection = connection;
                command.CommandText = "select * from Users where userName=@username";
                command.Parameters.Add("@username", SqlDbType.NVarChar).Value = username;
                using (var reader = command.ExecuteReader())
                {
                    if (reader.Read())
                    {
                        user = new UserModel()
                        {
                            Id = reader[0].ToString(),
                            Username = reader[1].ToString(),
                            Password = string.Empty,
                            Firstname = reader[3].ToString(),
                            Lastname = reader[4].ToString(),
                            Email = reader[5].ToString(),
                            ProfilePic = reader["profilePic"] as byte[]
                        };
                        if (reader[8] != DBNull.Value)
                        {
                            var positionValue = reader[8].ToString();
                            if (int.TryParse(positionValue, out int position))
                            {
                                user.Position = position;
                            }
                            else
                            {
                                // Log or debug the unexpected positionValue
                                Console.WriteLine($"Unexpected Position value: {positionValue}");

                                // if the Position is not a valid integer, set as 2
                                user.Position = 2;
                            }
                        }
                        else
                        {
                            // if the Position is DBNull, set as 2
                            user.Position = 2;
                        }
                    }
                }
            }
            return user;
        }

        public object GetByUsername(IPrincipal currentPrincipal)
        {
            throw new NotImplementedException();
        }

        public void Update(UserModel userModel)
        {
            throw new NotImplementedException();
        }
    }
}
