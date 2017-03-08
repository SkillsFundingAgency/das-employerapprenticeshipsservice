using SFA.DAS.Commitments.Api.Types;
using SFA.DAS.EAS.Web.ViewModels.ManageApprenticeships;

namespace SFA.DAS.EAS.Web.Orchestrators.Mappers
{
    public interface IApprenticeshipMapper
    {
        ApprenticeshipDetailsViewModel MapFrom(Apprenticeship apprenticeship);
    }
}