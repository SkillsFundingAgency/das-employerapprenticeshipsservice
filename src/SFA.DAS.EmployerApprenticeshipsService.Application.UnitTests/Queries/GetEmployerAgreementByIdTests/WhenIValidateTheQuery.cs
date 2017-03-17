using NUnit.Framework;
using SFA.DAS.EAS.Application.Queries.GetEmployerAgreementById;

namespace SFA.DAS.EAS.Application.UnitTests.Queries.GetEmployerAgreementByIdTests
{
    internal class WhenIValidateTheQuery
    {
        private GetEmployerAgreementByIdRequestValidator _validtor;

        [SetUp]
        public void Arrange()
        {
            _validtor = new GetEmployerAgreementByIdRequestValidator();
        }

        [Test]
        public void ThenValidationShouldIfThereIsNoHashAgreementId()
        {
            //Arrange
            var request = new GetEmployerAgreementByIdRequest();

            //Act
            var result = _validtor.Validate(request);
            
            //Assert
            Assert.IsFalse(result.IsValid());
            Assert.AreEqual(1, result.ValidationDictionary.Count);
        }
    }
}
