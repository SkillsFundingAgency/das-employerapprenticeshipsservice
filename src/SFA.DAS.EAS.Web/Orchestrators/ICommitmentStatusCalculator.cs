using SFA.DAS.Commitments.Api.Types;
using SFA.DAS.EAS.Web.Models;

namespace SFA.DAS.EAS.Web.Orchestrators
{
    public interface ICommitmentStatusCalculator
    {
        RequestStatus GetStatus(CommitmentStatus commitmentStatus, EditStatus editStatus, int apprenticeshipCount, AgreementStatus? overallAgreementStatus);
    }
}