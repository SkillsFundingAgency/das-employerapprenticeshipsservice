using NUnit.Framework;
using SFA.DAS.EAS.Domain.Models.Account;
using SFA.DAS.EAS.Web.Helpers;

namespace SFA.DAS.EAS.Web.UnitTests.Helpers.AccountTaskHelperTests
{
    [TestFixture]
    public class WhenIGetAPriority
    {

        [TestCase(1, "LevyDeclarationDue")]
        [TestCase(2, "AgreementToSign")]
        [TestCase(3, "AddApprentices")]
        [TestCase(4, "ApprenticeChangesToReview")]
        [TestCase(5, "CohortRequestReadyForApproval")]
        [TestCase(6, "IncompleteApprenticeshipDetails")]
        public void ThenIShouldRecieveTheCorrectValue(int priority, string taskType)
        {
            //Assert
            Assert.AreEqual(priority, AccountTaskHelper.GetTaskPriority(new AccountTask { Type = taskType }));
        }
    }
}