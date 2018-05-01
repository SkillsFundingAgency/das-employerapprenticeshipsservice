using SFA.DAS.Commitments.Api.Client;
using SFA.DAS.Commitments.Api.Client.Configuration;
using SFA.DAS.Commitments.Api.Client.Interfaces;
using SFA.DAS.EAS.Domain;
using SFA.DAS.EAS.Domain.Configuration;
using SFA.DAS.EAS.Infrastructure.DependencyResolution;
using StructureMap;

namespace SFA.DAS.EAS.Application.DependencyResolution
{
    public class CommitmentsRegistry : Registry
    {
        public CommitmentsRegistry()
        {
            var config = ConfigurationHelper.GetConfiguration<EmployerApprenticeshipsServiceConfiguration>(Constants.ServiceName);

            For<IApprenticeshipApi>().Use<ApprenticeshipApi>().Ctor<ICommitmentsApiClientConfiguration>().Is(config.CommitmentsApi);
            For<IEmployerCommitmentApi>().Use<EmployerCommitmentApi>().Ctor<ICommitmentsApiClientConfiguration>().Is(config.CommitmentsApi);
            For<IValidationApi>().Use<ValidationApi>().Ctor<ICommitmentsApiClientConfiguration>().Is(config.CommitmentsApi);
        }
    }
}