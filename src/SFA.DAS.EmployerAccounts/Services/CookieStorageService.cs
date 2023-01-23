using Microsoft.AspNetCore.Http;

namespace SFA.DAS.EmployerAccounts.Services;

public class CookieStorageService<T> : ICookieStorageService<T>
{
    private readonly ICookieService<T> _cookieService;
    private readonly HttpContextAccessor httpContextAccessor;

    public CookieStorageService(ICookieService<T> cookieService, HttpContextAccessor httpContextAccessor)
    {
        _cookieService = cookieService;
        httpContextAccessor = httpContextAccessor;
    }

    public void Create(T item, string cookieName, int expiryDays = 1)
    {
        _cookieService.Create(httpContextAccessor, cookieName, item, expiryDays);
    }

    public T Get(string cookieName)
    {
        return _cookieService.Get(httpContextAccessor, cookieName);
    }

    public void Delete(string cookieName)
    {
        _cookieService.Delete(httpContextAccessor, cookieName);
    }

    public void Update(string cookieName, T item)
    {
        _cookieService.Update(httpContextAccessor, cookieName, item);
    }
}