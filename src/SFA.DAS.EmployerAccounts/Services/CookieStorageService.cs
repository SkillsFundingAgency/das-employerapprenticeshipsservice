using Microsoft.AspNetCore.Http;

namespace SFA.DAS.EmployerAccounts.Services;

public class CookieStorageService<T> : ICookieStorageService<T>
{
    private readonly ICookieService<T> _cookieService;
    private readonly HttpContext _httpContext;

    public CookieStorageService(ICookieService<T> cookieService, HttpContext httpContext)
    {
        _cookieService = cookieService;
        _httpContext = httpContext;
    }

    public void Create(T item, string cookieName, int expiryDays = 1)
    {
        _cookieService.Create(_httpContext, cookieName, item, expiryDays);
    }

    public T Get(string cookieName)
    {
        return _cookieService.Get(_httpContext, cookieName);
    }

    public void Delete(string cookieName)
    {
        _cookieService.Delete(_httpContext, cookieName);
    }

    public void Update(string cookieName, T item)
    {
        _cookieService.Update(_httpContext, cookieName, item);
    }
}