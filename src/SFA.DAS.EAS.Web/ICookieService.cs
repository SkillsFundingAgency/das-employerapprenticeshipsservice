using System;
using System.Text;
using System.Web;
using System.Web.Security;

namespace SFA.DAS.EAS.Web
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
            var encodedContent = Convert.ToBase64String(MachineKey.Protect(Encoding.UTF8.GetBytes(content)));

            var userCookie = new HttpCookie(name, encodedContent)
            {
                Expires = DateTime.Now.AddDays(expireDays),
                Secure = true,
                HttpOnly = true,
            };

            context.Response.Cookies.Add(userCookie);
        }

        public void Update(HttpContextBase context, string name, string content)
        {
            var cookie = context.Request.Cookies[name];

            var encodedContent = Convert.ToBase64String(MachineKey.Protect(Encoding.UTF8.GetBytes(content)));

            if (cookie != null)
            {
                cookie.Value = encodedContent;
                
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
            if (context.Request.Cookies[name] == null)
                return null;

            var base64EncodedBytes = Convert.FromBase64String(context.Request.Cookies[name].Value);
            return Encoding.UTF8.GetString(MachineKey.Unprotect(base64EncodedBytes));
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
        public bool EmpRefNotFound { get; set; }
        public string CompanyStatus { get; set; }
        public string EmployerRefName { get; set; }
    }
}