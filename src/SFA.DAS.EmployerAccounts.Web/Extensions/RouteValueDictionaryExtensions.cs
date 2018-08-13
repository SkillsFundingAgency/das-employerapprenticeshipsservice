using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;
using System.Threading.Tasks;
using System.Web.Routing;

namespace SFA.DAS.EmployerAccounts.Web.Extensions
{
    public static class RouteValueDictionaryExtensions
    {
        public static void Merge(this RouteValueDictionary routeValues, NameValueCollection queryString)
        {
            foreach (string key in queryString.Keys)
            {
                routeValues[key] = queryString[key];
            }
        }
    }
}
