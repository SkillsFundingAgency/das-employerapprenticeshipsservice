using SFA.DAS.Commitments.Api.Types;
using SFA.DAS.EAS.Web.Models;

namespace SFA.DAS.EAS.Web.Orchestrators
{
    public interface ICommitmentStatusCalculator
    {
        RequestStatus GetStatus(EditStatus editStatus, int apprenticeshipCount, LastAction lastAction, AgreementStatus? overallAgreementStatus);
    }
}