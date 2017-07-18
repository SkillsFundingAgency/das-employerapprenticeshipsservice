using System;
using SFA.DAS.Commitments.Api.Types.Apprenticeship;
using SFA.DAS.EAS.Web.ViewModels.ManageApprenticeships;

namespace SFA.DAS.EAS.Web.Orchestrators.Mappers
{
    public interface IApprenticeshipFiltersMapper
    {
        ApprenticeshipSearchQuery MapToApprenticeshipSearchQuery(ApprenticeshipFiltersViewModel filters);
        ApprenticeshipFiltersViewModel Map(Facets facets);
    }
}
