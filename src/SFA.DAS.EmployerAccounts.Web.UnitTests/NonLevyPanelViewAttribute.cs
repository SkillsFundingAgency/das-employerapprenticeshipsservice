using System.Reflection;
using AutoFixture;
using AutoFixture.NUnit3;
using SFA.DAS.Common.Domain.Types;
using SFA.DAS.EmployerAccounts.Web.ViewComponents;

namespace SFA.DAS.EmployerAccounts.Web.UnitTests
{
    [AttributeUsage(AttributeTargets.Parameter)]
    public class NonLevyPanelViewAttribute : CustomizeAttribute
    {
        public override ICustomization GetCustomization(ParameterInfo parameter)
        {
            if (parameter == null)
            {
                throw new ArgumentNullException(nameof(parameter));
            }

            if (parameter.ParameterType != typeof(PanelViewModel<AccountDashboardViewModel>))
            {
                throw new ArgumentException("Parameter.ParameterType is not the correct type.", nameof(parameter));
            }

            return new ArrangeNonLevyPanelViewCustomisation();
        }
    }

    public class ArrangeNonLevyPanelViewCustomisation : ICustomization
    {
        public void Customize(IFixture fixture)
        {
            fixture.Customize<PanelViewModel<AccountDashboardViewModel>>(composer => composer
                .With(panel => panel.ComponentName, ComponentConstants.Empty));
            fixture.Customize<AccountDashboardViewModel>(composer => composer
                .With(dash => dash.ApprenticeshipEmployerType, ApprenticeshipEmployerType.NonLevy)
                .With(dash => dash.PendingAgreements, new List<PendingAgreementsViewModel>()));
        }
    }
}
