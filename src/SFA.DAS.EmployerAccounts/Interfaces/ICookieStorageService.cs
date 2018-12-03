using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SFA.DAS.EmployerAccounts.Interfaces
{
    public interface ICookieStorageService<T>
    {
        void Create(T item, string cookieName, int expiryDays = 1);
        void Delete(string cookieName);
        T Get(string cookieName);
        void Update(string cookieName, T item);
    }
}
