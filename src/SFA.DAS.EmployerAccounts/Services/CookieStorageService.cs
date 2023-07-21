using Microsoft.AspNetCore.Http;

namespace SFA.DAS.EmployerAccounts.Services;

public class CookieStorageService<T> : ICookieStorageService<T>
{
    private readonly ICookieService<T> _cookieService;
    private readonly IHttpContextAccessor _httpContextAccessor;

    public CookieStorageService(ICookieService<T> cookieService, IHttpContextAccessor httpContextAccessor)
    {
        _cookieService = cookieService;
        _httpContextAccessor = httpContextAccessor;
    }

    public void Create(T item, string cookieName, int expiryDays = 1)
    {
        _cookieService.Create(_httpContextAccessor, cookieName, item, expiryDays);
    }

    public T Get(string cookieName)
    {
        return _cookieService.Get(_httpContextAccessor, cookieName);
    }

    public void Delete(string cookieName)
    {
        _cookieService.Delete(_httpContextAccessor, cookieName);
    }

    public void Update(string cookieName, T item)
    {
        _cookieService.Update(_httpContextAccessor, cookieName, item);
    }
}