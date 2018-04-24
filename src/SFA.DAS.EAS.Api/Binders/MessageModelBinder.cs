using System.Globalization;
using System.Web.Http.Controllers;
using System.Web.Http.ModelBinding;
using System.Web.Http.ValueProviders;
using SFA.DAS.EAS.Application.Messages;
using SFA.DAS.EAS.Domain.Interfaces;
using SFA.DAS.EAS.Infrastructure.Authorization;
using WebApi.StructureMap;

namespace SFA.DAS.EAS.Account.Api.Binders
{
    public class MessageModelBinder : IModelBinder
    {
        public bool BindModel(HttpActionContext actionContext, ModelBindingContext bindingContext)
        {
            if (typeof(IAccountMessage).IsAssignableFrom(bindingContext.ModelMetadata.ContainerType) && bindingContext.ModelMetadata.PropertyName == nameof(IAccountMessage.AccountId))
            {
                var authorizationContext = actionContext.GetService<IAuthorizationService>().GetAuthorizationContext();
                var key = bindingContext.ModelName;
                var value = authorizationContext.AccountContext?.Id;
                var valueProviderResult = new ValueProviderResult(value, value?.ToString(), CultureInfo.InvariantCulture);

                bindingContext.Model = value;
                bindingContext.ModelState.SetModelValue(key, valueProviderResult);

                return true;
            }

            return false;
        }
    }
}