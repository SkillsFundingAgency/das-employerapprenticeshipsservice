using System;

namespace SFA.DAS.EAS.Web.Extensions
{
    public static class DynamicExtentions
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