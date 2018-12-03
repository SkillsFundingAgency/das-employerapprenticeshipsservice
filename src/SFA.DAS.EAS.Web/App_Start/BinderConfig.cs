using System.Web.Mvc;
using SFA.DAS.Authorization;
using SFA.DAS.Authorization.Mvc;
using SFA.DAS.Binders;

namespace SFA.DAS.EAS.Web
{
    public class BinderConfig
    {
        public static void RegisterBinders(ModelBinderDictionary binders)
        {
            binders.DefaultBinder = new MessageModelBinder(() => DependencyResolver.Current.GetService<ICallerContextProvider>());
            binders.Add(typeof(string), new TrimStringModelBinder());
        }
    }
}
