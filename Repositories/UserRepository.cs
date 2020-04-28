using liblib_backend.Common;
using liblib_backend.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace liblib_backend.Repositories
{
 
    public interface IUserRepository : ITransientService
    {
        Account GetAccountWithUsername(string username);
        bool CreateAccount(Account account);
    }

    public class UserRepository : IUserRepository
    {
        private MyLibContext DbContext;

        public UserRepository(MyLibContext DbContext)
        {
            this.DbContext = DbContext;
        }

        public bool CreateAccount(Account account)
        {
            try
            {
                DbContext.Account.Add(account);
                DbContext.SaveChanges();
                return true;
            } 
            catch (Exception)
            {
               return false;
            }
        }

        public Account GetAccountWithUsername(string username)
        {
           return DbContext.Account.FirstOrDefault(x => x.Username == username);
        }

    }
}
