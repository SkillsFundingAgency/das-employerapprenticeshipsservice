using NUnit.Framework;
using SFA.DAS.EmployerAccounts.Queries.GetClientContent;

namespace SFA.DAS.EmployerAccounts.UnitTests.Queries.GetContentBanner
{
    public class WhenIValidateTheRequest
    {
        private GetClientContentRequestValidator _validator;

        [SetUp]
        public void Arrange()
        {
            _validator = new GetClientContentRequestValidator();
        }

        [Test]
        public void ThenShouldReturnValidIfRequestIsValid()
        {
            //Act
            var result = _validator.Validate(new GetClientContentRequest()
            {
                ContentType = "banner",
                ClientId = "acc-eas"
            });

            //Assert
            Assert.IsTrue(result.IsValid());
        }

        [Test]
        public void ThenShouldReturnInvalidIfNoBannerIdIsProvided()
        {
            //Act
            var result = _validator.Validate(new GetClientContentRequest());

            //Assert
            Assert.IsFalse(result.IsValid());
        }

        [Test]
        public void ThenShouldReturnInvalidIfBannerIdIsZero()
        {
            //Act
            var result = _validator.Validate(new GetClientContentRequest { ContentType = string.Empty});

            //Assert
            Assert.IsFalse(result.IsValid());
        }
    }
}
