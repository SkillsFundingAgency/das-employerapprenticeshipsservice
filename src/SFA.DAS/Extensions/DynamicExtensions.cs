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

            if (viewBag.HideNav is string hideNavString)
            {
                return bool.TryParse(hideNavString, out var hideNavFlag) && hideNavFlag;
            }

            return viewBag.HideNav is bool && (bool) viewBag.HideNav;
        }
    }
}
