using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Security.Principal;
using System.Text;
using System.Threading.Tasks;

namespace PaintShopManagement.Models
{
    public interface IUserRepository
    {
        bool AuthenticateUser(NetworkCredential credential);
        void Add(UserModel userModel);
        void Update(UserModel userModel);
        void Delete(int Id);
        UserModel GetById(int Id);
        UserModel GetByUsername(string username);
        IEnumerable<UserModel> GetByAll();
    }
}
