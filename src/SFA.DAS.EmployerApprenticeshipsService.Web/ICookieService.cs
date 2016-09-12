using System;
using System.Web;

namespace SFA.DAS.EmployerApprenticeshipsService.Web
{
    public interface ICookieService
    {
        void Create(HttpContextBase context, string name, string content, int expireDays);
        void Update(HttpContextBase context, string name, string content);
        void Delete(HttpContextBase context, string name);
        object Get(HttpContextBase context, string name);
    }

    public class HttpCookieService : ICookieService
    {
        public void Create(HttpContextBase context, string name, string content, int expireDays)
        {
            var userCookie = new HttpCookie(name, content)
            {
                Expires = DateTime.Now.AddDays(expireDays),
                Secure = true,
                HttpOnly = true
            };
            context.Response.Cookies.Add(userCookie);
        }

        public void Update(HttpContextBase context, string name, string content)
        {
            var cookie = context.Request.Cookies[name];

            if (cookie != null)
            {
                cookie.Value = content;

                context.Response.SetCookie(cookie);
            }
        }

        public void Delete(HttpContextBase context, string name)
        {
            if (context.Request.Cookies[name] != null)
            {
                var user = new HttpCookie(name)
                {
                    Expires = DateTime.Now.AddDays(-1),
                    Value = null
                };
                context.Response.SetCookie(user);
            }
        }

        public object Get(HttpContextBase context, string name)
        {
            return context.Request.Cookies[name].Value;
        }
    }

    public class EmployerAccountData
    {
        public string CompanyNumber { get; set; }
        public string CompanyName { get; set; }
        public DateTime DateOfIncorporation { get; set; }

        public string RegisteredAddress { get; set; }

        public string EmployerRef { get; set; }
        public string AccessToken { get; set; }
        public string RefreshToken { get; set; }
    }
}