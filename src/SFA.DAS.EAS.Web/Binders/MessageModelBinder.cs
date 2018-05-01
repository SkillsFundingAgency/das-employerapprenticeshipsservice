using System;
using System.ComponentModel;
using System.Globalization;
using System.Web.Mvc;
using SFA.DAS.EAS.Application.Messages;
using SFA.DAS.EAS.Domain.Interfaces;
using SFA.DAS.EAS.Infrastructure.Authorization;

namespace SFA.DAS.EAS.Web.Binders
{
    public class MessageModelBinder : DefaultModelBinder
    {
        private readonly Func<IAuthorizationService> _authorizationService;

        public MessageModelBinder(Func<IAuthorizationService> authorizationService)
        {
            _authorizationService = authorizationService;
        }

        protected override void BindProperty(ControllerContext controllerContext, ModelBindingContext bindingContext, PropertyDescriptor propertyDescriptor)
        {
            if (typeof(IAccountMessage).IsAssignableFrom(bindingContext.ModelType) & propertyDescriptor.Name == nameof(IAccountMessage.AccountId))
            {
                var authorizationContext = _authorizationService().GetAuthorizationContext();
                var key = CreateSubPropertyName(bindingContext.ModelName, propertyDescriptor.Name);
                var value = authorizationContext.AccountContext?.Id;
                var valueProviderResult = new ValueProviderResult(value, value?.ToString(), CultureInfo.InvariantCulture);
                var model = (IAccountMessage)bindingContext.Model;

                model.AccountId = value;
                bindingContext.ModelState.SetModelValue(key, valueProviderResult);

                return;
            }

            if (typeof(IAccountMessage).IsAssignableFrom(bindingContext.ModelType) & propertyDescriptor.Name == nameof(IAccountMessage.AccountHashedId))
            {
                var authorizationContext = _authorizationService().GetAuthorizationContext();
                var key = CreateSubPropertyName(bindingContext.ModelName, propertyDescriptor.Name);
                var value = authorizationContext.AccountContext?.HashedId;
                var valueProviderResult = new ValueProviderResult(value, value, CultureInfo.InvariantCulture);
                var model = (IAccountMessage)bindingContext.Model;

                model.AccountHashedId = value;
                bindingContext.ModelState.SetModelValue(key, valueProviderResult);

                return;
            }

            if (typeof(IUserMessage).IsAssignableFrom(bindingContext.ModelType) && propertyDescriptor.Name == nameof(IUserMessage.UserId))
            {
                var authorizationContext = _authorizationService().GetAuthorizationContext();
                var key = CreateSubPropertyName(bindingContext.ModelName, propertyDescriptor.Name);
                var value = authorizationContext.UserContext?.Id;
                var valueProviderResult = new ValueProviderResult(value, value?.ToString(), CultureInfo.InvariantCulture);
                var model = (IUserMessage)bindingContext.Model;

                model.UserId = value;
                bindingContext.ModelState.SetModelValue(key, valueProviderResult);

                return;
            }

            base.BindProperty(controllerContext, bindingContext, propertyDescriptor);
        }
    }
}