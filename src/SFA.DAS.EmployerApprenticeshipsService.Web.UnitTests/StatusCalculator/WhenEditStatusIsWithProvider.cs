using FluentAssertions;
using NUnit.Framework;
using SFA.DAS.Commitments.Api.Types;
using SFA.DAS.Commitments.Api.Types.Commitment.Types;
using SFA.DAS.EAS.Web.Enums;
using SFA.DAS.EAS.Web.Orchestrators;

namespace SFA.DAS.EAS.Web.UnitTests.StatusCalculator
{
    [TestFixture]
    public sealed class WhenEditStatusIsWithProvider
    {
        private static readonly ICommitmentStatusCalculator _calculator = new CommitmentStatusCalculator();

        [TestCase(RequestStatus.SentToProvider, LastAction.None, TestName = "New request sent")]
        public void WhenThereAreNoApprentices(RequestStatus expectedResult, LastAction lastAction)
        {
            var status = _calculator.GetStatus(EditStatus.ProviderOnly, 0, lastAction, AgreementStatus.NotAgreed);

            status.Should().Be(expectedResult);
        }

        [TestCase(RequestStatus.SentForReview, LastAction.Amend, AgreementStatus.NotAgreed, TestName = "Sent for review")]
        [TestCase(RequestStatus.SentForReview, LastAction.Amend, AgreementStatus.ProviderAgreed, TestName = "Sent for review that was approved by provider")]
        [TestCase(RequestStatus.WithProviderForApproval, LastAction.Approve, AgreementStatus.EmployerAgreed, TestName = "Sent for approval")]
        [TestCase(RequestStatus.WithProviderForApproval, LastAction.Approve, AgreementStatus.NotAgreed, TestName = "Sent for approval but changed by provider")]
        public void WhenThereAreApprentices(RequestStatus expectedResult, LastAction lastAction, AgreementStatus overallAgreementStatus)
        {
            var status = _calculator.GetStatus(EditStatus.ProviderOnly, 2, lastAction, overallAgreementStatus);

            status.Should().Be(expectedResult);
        }
    }
}
