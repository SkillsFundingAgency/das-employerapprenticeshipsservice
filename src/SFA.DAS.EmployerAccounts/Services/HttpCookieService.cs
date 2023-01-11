using System.Text;
using Microsoft.AspNetCore.DataProtection;
using Microsoft.AspNetCore.Http;
using Newtonsoft.Json;

namespace SFA.DAS.EmployerAccounts.Services;

public class HttpCookieService<T> : ICookieService<T>
{
    private readonly IDataProtector _protector;

    public HttpCookieService(IDataProtectionProvider provider)
    {
        _protector = provider.CreateProtector("SFA.DAS.EmployerAccounts.Services.HttpCookieService");
    }

    public void Create(HttpContext context, string name, T content, int expireDays)
    {
        var cookieContent = JsonConvert.SerializeObject(content);

        var encodedContent = Convert.ToBase64String(_protector.Protect(new UTF8Encoding().GetBytes(cookieContent)));

        context.Response.Cookies.Append(name, encodedContent, new CookieOptions
        {
            Expires = DateTime.Now.AddDays(expireDays),
            Secure = true,
            HttpOnly = true,
        });
    }

    public void Update(HttpContext context, string name, T content)
    {
        var cookie = context.Request.Cookies[name];

        if (cookie != null)
        {
            var cookieContent = JsonConvert.SerializeObject(content);

            var encodedContent = Convert.ToBase64String(_protector.Protect(new UTF8Encoding().GetBytes(cookieContent)));
            context.Response.Cookies.Append(name, encodedContent);
        }
    }

    public void Delete(HttpContext context, string name)
    {
        if (context.Request.Cookies[name] != null)
        {
            context.Response.Cookies.Delete(name);
        }
    }

    public T Get(HttpContext context, string name)
    {
        if (context.Request.Cookies[name] == null)
            return default(T);

        var base64EncodedBytes = Convert.FromBase64String(context.Request.Cookies[name]);
        return JsonConvert.DeserializeObject<T>(new UTF8Encoding().GetString(_protector.Unprotect(base64EncodedBytes)));
    }
}