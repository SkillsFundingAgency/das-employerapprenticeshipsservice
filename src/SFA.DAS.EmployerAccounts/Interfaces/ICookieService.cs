using Microsoft.AspNetCore.Http;

namespace SFA.DAS.EmployerAccounts.Interfaces;

public interface ICookieService<T>
{
    void Create(IHttpContextAccessor contextAccessor, string name, T content, int expireDays);

    void Update(IHttpContextAccessor contextAccessor, string name, T content);

    void Delete(IHttpContextAccessor contextAccessor, string name);

    T Get(IHttpContextAccessor contextAccessor, string name);
}