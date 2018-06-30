using System;
using System.ComponentModel;
using System.Globalization;
using System.Web.Mvc;
using SFA.DAS.EAS.Application.Messages;
using SFA.DAS.EAS.Infrastructure.Authorization;

namespace SFA.DAS.EAS.Web.Binders
{
    public class MessageModelBinder : DefaultModelBinder
    {
        private readonly Func<ICallerContextProvider> _callContextProvider;

        public MessageModelBinder(Func<ICallerContextProvider> callContextProvider)
        {
            _callContextProvider = callContextProvider;
        }

        protected override void BindProperty(ControllerContext controllerContext, ModelBindingContext bindingContext, PropertyDescriptor propertyDescriptor)
        {
            if (typeof(IAccountMessage).IsAssignableFrom(bindingContext.ModelType) & propertyDescriptor.Name == nameof(IAccountMessage.AccountHashedId))
            {
                var requestContext = _callContextProvider().GetCallerContext();
                var key = CreateSubPropertyName(bindingContext.ModelName, propertyDescriptor.Name);
                var value = requestContext.AccountHashedId;
                var valueProviderResult = new ValueProviderResult(value, value, CultureInfo.InvariantCulture);
                var model = (IAccountMessage)bindingContext.Model;

                model.AccountHashedId = value;
                bindingContext.ModelState.SetModelValue(key, valueProviderResult);

                return;
            }

            if (typeof(IAccountMessage).IsAssignableFrom(bindingContext.ModelType) & propertyDescriptor.Name == nameof(IAccountMessage.AccountId))
            {
                var requestContext = _callContextProvider().GetCallerContext();
                var key = CreateSubPropertyName(bindingContext.ModelName, propertyDescriptor.Name);
                var value = requestContext.AccountId;
                var valueProviderResult = new ValueProviderResult(value, value?.ToString(), CultureInfo.InvariantCulture);
                var model = (IAccountMessage)bindingContext.Model;

                model.AccountId = value;
                bindingContext.ModelState.SetModelValue(key, valueProviderResult);

                return;
            }

            if (typeof(IUserMessage).IsAssignableFrom(bindingContext.ModelType) && propertyDescriptor.Name == nameof(IUserMessage.UserRef))
            {
                var requestContext = _callContextProvider().GetCallerContext();
                var key = CreateSubPropertyName(bindingContext.ModelName, propertyDescriptor.Name);
                var value = requestContext.UserRef;
                var valueProviderResult = new ValueProviderResult(value, value?.ToString(), CultureInfo.InvariantCulture);
                var model = (IUserMessage)bindingContext.Model;

                model.UserRef = value;
                bindingContext.ModelState.SetModelValue(key, valueProviderResult);

                return;
            }

            base.BindProperty(controllerContext, bindingContext, propertyDescriptor);
        }
    }
}