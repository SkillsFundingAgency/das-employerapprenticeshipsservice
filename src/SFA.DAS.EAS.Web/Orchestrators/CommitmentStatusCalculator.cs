using System;
using SFA.DAS.Commitments.Api.Types;
using SFA.DAS.EAS.Web.Models;

namespace SFA.DAS.EAS.Web.Orchestrators
{
    public sealed class CommitmentStatusCalculator : ICommitmentStatusCalculator
    {
        public RequestStatus GetStatus(CommitmentStatus commitmentStatus, EditStatus editStatus, int apprenticeshipCount, AgreementStatus? overallAgreementStatus)
        {
            bool hasApprenticeships = apprenticeshipCount > 0;

            if (editStatus == EditStatus.Both)
                return RequestStatus.Approved;

            if (editStatus == EditStatus.EmployerOnly)
            {
                if (commitmentStatus != CommitmentStatus.New)
                {
                    if (hasApprenticeships && overallAgreementStatus == AgreementStatus.NotAgreed)
                        return RequestStatus.ReadyForReview;

                    if (hasApprenticeships && overallAgreementStatus == AgreementStatus.ProviderAgreed)
                        return RequestStatus.ReadyForApproval;
                }

                return RequestStatus.NewRequest;
            }

            if (hasApprenticeships && overallAgreementStatus == AgreementStatus.EmployerAgreed)
                return RequestStatus.WithProviderForApproval;

            return RequestStatus.SentToProvider;
        }
    }
}