using System.Collections.Generic;
using System.ComponentModel;
using System.Dynamic;

namespace SFA.DAS.EAS.Web.Extensions
{
    public static class DynamicExtensions
    {
        public static bool ParseHideNavFromViewBag(dynamic viewBag)
        {
            if (viewBag.HideNav == null)
            {
                return false;
            }

            var hideNavString = viewBag.HideNav as string;

            if (hideNavString != null)
            {
                bool hideNavFlag;

                if (bool.TryParse(hideNavString, out hideNavFlag))
                {
                    return hideNavFlag;
                }

                return false;
            }

            if (viewBag.HideNav is bool)
            {
                return viewBag.HideNav;
            }

            return false;
        }

        public static IDictionary<string, object> ToDictionary(this object obj)
        {
            IDictionary<string, object> expando = new ExpandoObject();

            foreach (PropertyDescriptor property in TypeDescriptor.GetProperties(obj.GetType()))
            {
                expando.Add(property.Name, property.GetValue(obj));
            }

            return expando;
        }
    }
}