using NUnit.Framework;
using SFA.DAS.EAS.Application.Queries.GetTransferBalance;

namespace SFA.DAS.EAS.Application.UnitTests.Queries.GetTransferBalanceTests
{
    public class WhenIValidateTheRequest
    {
        private GetTransferBalanceRequestValidator _validator;

        [SetUp]
        public void Arrange()
        {
            _validator = new GetTransferBalanceRequestValidator();

        }

        [Test]
        public void ThenTheRequestIsInvalidIfTheFieldsAreNotPopulated()
        {
            //Act
            var actual = _validator.Validate(new GetTransferBalanaceRequest { HashedAccountId = string.Empty });

            //Assert
            Assert.IsFalse(actual.IsValid());
            Assert.IsTrue(actual.ValidationDictionary.ContainsKey(nameof(GetTransferBalanaceRequest.HashedAccountId)));
        }

        [Test]
        public void ThenTheRequestIsInvalidIfTheFieldIsNull()
        {
            //Act
            var actual = _validator.Validate(new GetTransferBalanaceRequest());

            //Assert
            Assert.IsFalse(actual.IsValid());
        }

        [Test]
        public void ThenTheRequestIsValidIfTheFieldsArePopulated()
        {
            //Act
            var actual = _validator.Validate(new GetTransferBalanaceRequest { HashedAccountId = "1234" });

            //Assert
            Assert.IsTrue(actual.IsValid());
        }
    }
}
