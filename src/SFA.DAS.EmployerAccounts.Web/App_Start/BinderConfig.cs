using System.Web.Mvc;
using SFA.DAS.Authorization.Mvc.Extensions;
using SFA.DAS.EmployerAccounts.Web.Binders;

namespace SFA.DAS.EmployerAccounts.Web
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
