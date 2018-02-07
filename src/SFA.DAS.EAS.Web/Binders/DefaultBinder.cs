using System;
using System.ComponentModel;
using System.Globalization;
using System.Web.Mvc;
using SFA.DAS.EAS.Application.Messages;
using SFA.DAS.EAS.Web.Authentication;
using SFA.DAS.EAS.Web.Helpers;

namespace SFA.DAS.EAS.Web.Binders
{
    public class DefaultBinder : DefaultModelBinder
    {
        private readonly Func<ICurrentUserService> _currentUserService;

        public DefaultBinder(Func<ICurrentUserService> currentUserService)
        {
            _currentUserService = currentUserService;
        }

        protected override void BindProperty(ControllerContext controllerContext, ModelBindingContext bindingContext, PropertyDescriptor propertyDescriptor)
        {
            if (typeof(IAuthorizedMessage).IsAssignableFrom(bindingContext.ModelType))
            {
                var model = (IAuthorizedMessage)bindingContext.Model;
                var key = $"{bindingContext.ModelName}.{propertyDescriptor.Name}";

                if (propertyDescriptor.Name == nameof(IAuthorizedMessage.UserExternalId))
                {
                    var currentUser = _currentUserService().GetCurrentUser();

                    model.UserExternalId = currentUser.ExternalId;
                    bindingContext.ModelState.SetModelValue(key, new ValueProviderResult(model.UserExternalId, model.UserExternalId.ToString(), CultureInfo.InvariantCulture));

                    return;
                }

                if (propertyDescriptor.Name == nameof(IAuthorizedMessage.AccountHashedId))
                {
                    var accountHashedId = controllerContext.RouteData.Values[ControllerConstants.HashedAccountIdKeyName].ToString();

                    model.AccountHashedId = accountHashedId;
                    bindingContext.ModelState.SetModelValue(key, new ValueProviderResult(model.AccountHashedId, model.AccountHashedId, CultureInfo.InvariantCulture));

                    return;
                }
            }

            base.BindProperty(controllerContext, bindingContext, propertyDescriptor);
        }
    }
}