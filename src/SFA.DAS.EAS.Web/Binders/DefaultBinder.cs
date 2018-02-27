using System;
using System.ComponentModel;
using System.Globalization;
using System.Web.Mvc;
using SFA.DAS.EAS.Application.Messages;
using SFA.DAS.EAS.Web.Authorization;

namespace SFA.DAS.EAS.Web.Binders
{
    public class DefaultBinder : DefaultModelBinder
    {
        private readonly Func<IMembershipService> _membershipService;

        public DefaultBinder(Func<IMembershipService> membershipService)
        {
            _membershipService = membershipService;
        }

        protected override void BindProperty(ControllerContext controllerContext, ModelBindingContext bindingContext, PropertyDescriptor propertyDescriptor)
        {
            if (typeof(IAuthorizedMessage).IsAssignableFrom(bindingContext.ModelType))
            {
                var model = (IAuthorizedMessage)bindingContext.Model;
                var key = $"{bindingContext.ModelName}.{propertyDescriptor.Name}";
                var membershipContext = _membershipService().GetMembershipContext();

                switch (propertyDescriptor.Name)
                {
                    case nameof(IAuthorizedMessage.AccountHashedId):
                        model.AccountHashedId = membershipContext?.AccountHashedId;
                        bindingContext.ModelState.SetModelValue(key, new ValueProviderResult(model.AccountHashedId, model.AccountHashedId, CultureInfo.InvariantCulture));
                        return;
                    case nameof(IAuthorizedMessage.AccountPublicHashedId):
                        model.AccountPublicHashedId = membershipContext?.AccountPublicHashedId;
                        bindingContext.ModelState.SetModelValue(key, new ValueProviderResult(model.AccountPublicHashedId, model.AccountPublicHashedId, CultureInfo.InvariantCulture));
                        return;
                    case nameof(IAuthorizedMessage.AccountId):
                        model.AccountId = membershipContext?.AccountId;
                        bindingContext.ModelState.SetModelValue(key, new ValueProviderResult(model.AccountId, model.AccountId?.ToString(), CultureInfo.InvariantCulture));
                        return;
                    case nameof(IAuthorizedMessage.UserExternalId):
                        model.UserExternalId = membershipContext?.UserExternalId;
                        bindingContext.ModelState.SetModelValue(key, new ValueProviderResult(model.UserExternalId, model.UserExternalId?.ToString(), CultureInfo.InvariantCulture));
                        return;
                    case nameof(IAuthorizedMessage.UserId):
                        model.UserId = membershipContext?.UserId;
                        bindingContext.ModelState.SetModelValue(key, new ValueProviderResult(model.UserId, model.UserId?.ToString(), CultureInfo.InvariantCulture));
                        return;
                }
            }

            base.BindProperty(controllerContext, bindingContext, propertyDescriptor);
        }
    }
}