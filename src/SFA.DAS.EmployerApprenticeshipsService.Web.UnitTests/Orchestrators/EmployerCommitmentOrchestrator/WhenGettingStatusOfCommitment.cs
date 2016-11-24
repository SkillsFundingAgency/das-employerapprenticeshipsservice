using FluentAssertions;
using NUnit.Framework;
using SFA.DAS.Commitments.Api.Types;
using SFA.DAS.EAS.Web.Models;
using SFA.DAS.EAS.Web.Orchestrators;

namespace SFA.DAS.EAS.Web.UnitTests.Orchestrators.EmployerCommitmentOrchestrator
{
    [TestFixture]
    public sealed class WhenGettingStatusOfCommitment
    {
        private static readonly ICommitmentStatusCalculator _calculator = new CommitmentStatusCalculator();

        [TestCase(RequestStatus.SentToProvider, CommitmentStatus.Active, EditStatus.ProviderOnly, 0, null, TestName = "With Provider, no apprenticeships")]
        [TestCase(RequestStatus.NewRequest, CommitmentStatus.New, EditStatus.EmployerOnly, 0, null, TestName = "With Employer, no apprenticeships")]
        [TestCase(RequestStatus.SentToProvider, CommitmentStatus.Active, EditStatus.ProviderOnly, 2, AgreementStatus.NotAgreed, TestName = "With Provider, all apprenticeships NotAgreed")]
        [TestCase(RequestStatus.ReadyForReview, CommitmentStatus.Active, EditStatus.EmployerOnly, 2, AgreementStatus.NotAgreed, TestName = "With Employer, all apprenticeships NotAgreed")]
        [TestCase(RequestStatus.WithProviderForApproval, CommitmentStatus.Active, EditStatus.ProviderOnly, 2, AgreementStatus.EmployerAgreed, TestName = "With Provider, all apprenticeships EmployerAgreed")]
        [TestCase(RequestStatus.SentToProvider, CommitmentStatus.Active, EditStatus.ProviderOnly, 2, AgreementStatus.ProviderAgreed, TestName = "With Provider, all apprenticeships ProviderAgreed")]
        [TestCase(RequestStatus.ReadyForApproval, CommitmentStatus.Active, EditStatus.EmployerOnly, 2, AgreementStatus.ProviderAgreed, TestName = "With Employer, all apprenticeships ProviderAgreed")]
        [TestCase(RequestStatus.Approved, CommitmentStatus.Active, EditStatus.Both, 2, AgreementStatus.BothAgreed, TestName = "With Both, all apprenticeships BothAgreed")]
        [TestCase(RequestStatus.NewRequest, CommitmentStatus.New, EditStatus.EmployerOnly, 1, AgreementStatus.NotAgreed, TestName = "With Employer, all apprenticeships NotAgreed")]
        public void ThenReturnsStatusBasedOnEditStatusAndApprenticeships(RequestStatus expectedResult, CommitmentStatus commitmentStatus, EditStatus editStatus, int numberOfApprenticeships, AgreementStatus? overallAgreementStatus)
        {
            var status = _calculator.GetStatus(commitmentStatus, editStatus, numberOfApprenticeships, overallAgreementStatus);

            status.Should().Be(expectedResult);
        }
    }
}
