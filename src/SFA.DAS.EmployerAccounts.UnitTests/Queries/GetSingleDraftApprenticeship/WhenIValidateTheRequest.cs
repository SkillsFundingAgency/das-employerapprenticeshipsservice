using NUnit.Framework;
using SFA.DAS.EmployerAccounts.Queries.GetSingleDraftApprenticeship;

namespace SFA.DAS.EmployerAccounts.UnitTests.Queries.GetSingleDraftApprenticeship
{
    public class WhenIValidateTheRequest
    {
        private GetSingleDraftApprenticeshipRequestValidator _validator;

        [SetUp]
        public void Arrange()
        {
            _validator = new GetSingleDraftApprenticeshipRequestValidator();
        }

        [Test]
        public void ThenShouldReturnValidIfRequestIsValid()
        {
            //Act
            var result = _validator.Validate(new GetSingleDraftApprenticeshipRequest { CohortId = 123 });

            //Assert
            Assert.IsTrue(result.IsValid());
        }
    }
}
