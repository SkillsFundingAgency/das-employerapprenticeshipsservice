using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SFA.DAS.Extensions
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
    }
}
