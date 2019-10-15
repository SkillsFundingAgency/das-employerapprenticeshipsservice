using System.Web.Mvc;
using SFA.DAS.Authorization.Mvc.Extensions;
using SFA.DAS.Binders;

namespace SFA.DAS.EAS.Web
{
    public class BinderConfig
    {
        public static void RegisterBinders(ModelBinderDictionary binders)
        {
            binders.UseAuthorizationModelBinder();
            binders.Add(typeof(string), new TrimStringModelBinder());
        }
    }
}
