using NUnit.Framework;
using SFA.DAS.CommitmentsV2.Types;
using SFA.DAS.EmployerAccounts.Web.Extensions;

namespace SFA.DAS.EmployerAccounts.Web.UnitTests.Extensions
{
    [TestFixture]
    public class CohortSummaryExtensionTests
    {
        [TestCase(false, Party.Employer, CohortStatus.Review)]
        [TestCase(true, Party.Employer, CohortStatus.Draft)]
        [TestCase(false, Party.Provider, CohortStatus.WithTrainingProvider)]
        [TestCase(true, Party.Provider, CohortStatus.Unknown)]
        public void CohortSummary_GetStatus_Returns_Correct_Status(bool isDraft, Party withParty, CohortStatus cohortStatus)
        {
            //Arrange
            var cohortSummary = new CohortSummary
            {
                CohortId = 1,
                IsDraft = isDraft,
                WithParty = withParty
            };

            //Act
            var status = cohortSummary.GetStatus();

            //Assert
            Assert.AreEqual(cohortStatus, status);
        }
    }
}
