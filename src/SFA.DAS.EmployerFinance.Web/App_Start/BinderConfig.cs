using System.Web.Mvc;
using SFA.DAS.Authorization;
using SFA.DAS.Authorization.Mvc;
using SFA.DAS.Authorization.Mvc.Extensions;
using SFA.DAS.EmployerFinance.Web.Binders;

namespace SFA.DAS.EmployerFinance.Web
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
