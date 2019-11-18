using NUnit.Framework;
using SFA.DAS.EmployerAccounts.Queries.GetEmployerAgreementsByAccountId;

namespace SFA.DAS.EmployerAccounts.UnitTests.Queries.GetEmployerAgreementsByAccountIdTests
{
    internal class WhenIValidateTheQuery
    {
        private GetEmployerAgreementsByAccountIdRequestValidator _validtor;

        [SetUp]
        public void Arrange()
        {
            _validtor = new GetEmployerAgreementsByAccountIdRequestValidator();
        }

        [Test]
        public void ThenValidationShouldFailIfThereIsNoAccountId()
        {
            //Arrange
            var request = new GetEmployerAgreementsByAccountIdRequest();

            //Act
            var result = _validtor.Validate(request);
            
            //Assert
            Assert.IsFalse(result.IsValid());
            Assert.AreEqual(1, result.ValidationDictionary.Count);
        }
    }
}
