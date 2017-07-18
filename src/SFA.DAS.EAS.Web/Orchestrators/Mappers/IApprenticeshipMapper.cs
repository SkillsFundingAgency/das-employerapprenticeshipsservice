using System.Collections.Generic;

using SFA.DAS.EAS.Web.ViewModels;
using SFA.DAS.Commitments.Api.Types.Apprenticeship;
using SFA.DAS.EAS.Web.ViewModels.ManageApprenticeships;
using System.Threading.Tasks;

using SFA.DAS.Commitments.Api.Types.ProviderPayment;
using SFA.DAS.EAS.Application.Queries.GetOverlappingApprenticeships;

namespace SFA.DAS.EAS.Web.Orchestrators.Mappers
{
    public interface IApprenticeshipMapper
    {
        Task<Apprenticeship> MapFrom(ApprenticeshipViewModel viewModel);

        ApprenticeshipDetailsViewModel MapToApprenticeshipDetailsViewModel(Apprenticeship apprenticeship);

        ApprenticeshipViewModel MapToApprenticeshipViewModel(Apprenticeship apprenticeship);

        Task<UpdateApprenticeshipViewModel> CompareAndMapToApprenticeshipViewModel(Apprenticeship original, ApprenticeshipViewModel edited);

        Dictionary<string, string> MapOverlappingErrors(GetOverlappingApprenticeshipsQueryResponse overlappingErrors);

        ApprenticeshipUpdate MapFrom(UpdateApprenticeshipViewModel viewModel);

        UpdateApprenticeshipViewModel MapFrom(ApprenticeshipUpdate apprenticeshipUpdate);

        PaymentOrderViewModel MapPayment(IList<ProviderPaymentPriorityItem> data);
    }
}