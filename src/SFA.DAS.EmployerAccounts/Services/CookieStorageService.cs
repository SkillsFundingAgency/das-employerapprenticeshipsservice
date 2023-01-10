using SFA.DAS.CookieService;
using SFA.DAS.EmployerAccounts.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SFA.DAS.EmployerAccounts.Services
{
    public class CookieStorageService<T> : ICookieStorageService<T>
    {
        private readonly ICookieService<T> _cookieService;
        private readonly HttpContextBase _httpContextBase;

        public CookieStorageService(ICookieService<T> cookieService, HttpContextBase httpContextBase)
        {
            _cookieService = cookieService;
            _httpContextBase = httpContextBase;
        }

        public void Create(T item, string cookieName, int expiryDays = 1)
        {
            _cookieService.Create(_httpContextBase, cookieName, item, expiryDays);
        }

        public T Get(string cookieName)
        {
            return _cookieService.Get(_httpContextBase, cookieName);
        }

        public void Delete(string cookieName)
        {
            _cookieService.Delete(_httpContextBase, cookieName);
        }

        public void Update(string cookieName, T item)
        {
            _cookieService.Update(_httpContextBase, cookieName, item);
        }
    }
}
