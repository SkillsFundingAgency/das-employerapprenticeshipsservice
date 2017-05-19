using System.Collections.Generic;

using SFA.DAS.EAS.Web.ViewModels;
using SFA.DAS.Commitments.Api.Types.Apprenticeship;
using SFA.DAS.EAS.Web.ViewModels.ManageApprenticeships;
using System.Threading.Tasks;

using SFA.DAS.EAS.Application.Queries.GetOverlappingApprenticeships;
using SFA.DAS.EAS.Application.Queries.GetProviderPaymentPriority;

namespace SFA.DAS.EAS.Web.Orchestrators.Mappers
{
    public interface IApprenticeshipMapper
    {
        Task<Apprenticeship> MapFromAsync(ApprenticeshipViewModel viewModel);

        ApprenticeshipDetailsViewModel MapToApprenticeshipDetailsViewModel(Apprenticeship apprenticeship);

        ApprenticeshipViewModel MapToApprenticeshipViewModel(Apprenticeship apprenticeship);

        Task<UpdateApprenticeshipViewModel> CompareAndMapToApprenticeshipViewModel(Apprenticeship original, ApprenticeshipViewModel edited);

        Task<Apprenticeship> MapFrom(ApprenticeshipViewModel viewModel);
        
        Dictionary<string, string> MapOverlappingErrors(GetOverlappingApprenticeshipsQueryResponse overlappingErrors);

        ApprenticeshipUpdate MapFrom(UpdateApprenticeshipViewModel viewModel);

        UpdateApprenticeshipViewModel MapFrom(ApprenticeshipUpdate apprenticeshipUpdate);

        PaymentOrderViewModel MapPayment(List<GetProviderPaymentPriorityHandler.ProviderPaymentPriorityItemAPI> data);

        List<GetProviderPaymentPriorityHandler.ProviderPaymentPriorityItemAPI> MapPayment(
            IEnumerable<PaymentOrderItem> paymentItems);
    }
}