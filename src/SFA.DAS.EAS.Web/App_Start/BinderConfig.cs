using System.Web.Mvc;
using SFA.DAS.EAS.Infrastructure.Authorization;
using SFA.DAS.EAS.Web.Binders;

namespace SFA.DAS.EAS.Web
{
    public class BinderConfig
    {
        public static void RegisterBinders(ModelBinderDictionary binders)
        {
            binders.DefaultBinder = new MessageModelBinder(() => DependencyResolver.Current.GetService<IAuthorizationService>());
            binders.Add(typeof(string), new TrimStringModelBinder());
        }
    }
}
