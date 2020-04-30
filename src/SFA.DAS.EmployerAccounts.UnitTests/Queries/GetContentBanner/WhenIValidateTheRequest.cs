using NUnit.Framework;
using SFA.DAS.EmployerAccounts.Queries.GetContentBanner;

namespace SFA.DAS.EmployerAccounts.UnitTests.Queries.GetContentBanner
{
    public class WhenIValidateTheRequest
    {
        private GetContentBannerRequestValidator _validator;

        [SetUp]
        public void Arrange()
        {
            _validator = new GetContentBannerRequestValidator();
        }

        [Test]
        public void ThenShouldReturnValidIfRequestIsValid()
        {
            //Act
            var result = _validator.Validate(new GetContentBannerRequest()
            {
                BannerId = 1,
                UseCDN = false
            });

            //Assert
            Assert.IsTrue(result.IsValid());
        }

        [Test]
        public void ThenShouldReturnInvalidIfNoBannerIdIsProvided()
        {
            //Act
            var result = _validator.Validate(new GetContentBannerRequest());

            //Assert
            Assert.IsFalse(result.IsValid());
        }

        [Test]
        public void ThenShouldReturnInvalidIfBannerIdIsZero()
        {
            //Act
            var result = _validator.Validate(new GetContentBannerRequest { BannerId = 0 });

            //Assert
            Assert.IsFalse(result.IsValid());
        }
    }
}
