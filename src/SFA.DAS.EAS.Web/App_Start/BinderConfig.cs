using System.Web.Mvc;
using SFA.DAS.EAS.Web.Authentication;
using SFA.DAS.EAS.Web.Binders;

namespace SFA.DAS.EAS.Web
{
    public class BinderConfig
    {
        public static void RegisterBinders(ModelBinderDictionary binders)
        {
            binders.DefaultBinder = new DefaultBinder(() => DependencyResolver.Current.GetService<ICurrentUserService>());
            binders.Add(typeof(string), new TrimStringBinder());
        }
    }
}
