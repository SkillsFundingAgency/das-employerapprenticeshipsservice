using Microsoft.AspNetCore.Http;

namespace SFA.DAS.EmployerAccounts.Interfaces
{
    public interface ICookieService<T>
    {
        void Create(HttpContext context, string name, T content, int expireDays);

        void Update(HttpContext context, string name, T content);

        void Delete(HttpContext context, string name);

        T Get(HttpContext context, string name);
    }
}
