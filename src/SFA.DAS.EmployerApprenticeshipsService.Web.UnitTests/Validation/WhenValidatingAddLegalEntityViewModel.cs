using System.Linq;
using System.Threading.Tasks;
using FluentValidation;
using NUnit.Framework;
using SFA.DAS.Common.Domain.Types;
using SFA.DAS.EAS.Web.Validation;
using SFA.DAS.EAS.Web.ViewModels.Organisation;

namespace SFA.DAS.EAS.Web.UnitTests.Validation
{
    [TestFixture]
    public class WhenValidatingAddLegalEntityViewModel
    {

        private AddLegalEntityViewModelValidator _validator;

        [SetUp]
        public void Arrange()
        {
            _validator = new AddLegalEntityViewModelValidator();
        }

        [Test]
        public async Task ThenAnOrganisationTypeMustBeSelected()
        {
            //Arrange
            var model = new AddLegalEntityViewModel();

            //Act
            var result = await _validator.ValidateAsync(model);

            //Assert
            Assert.IsFalse(result.IsValid);
            Assert.AreEqual(1, result.Errors.Count);
            Assert.IsNotNull(result.Errors.SingleOrDefault(x => x.PropertyName == "OrganisationType"));
        }

        [Test]
        [TestCase(OrganisationType.CompaniesHouse, "CompaniesHouseNumber")]
        [TestCase(OrganisationType.Charities, "CharityRegistrationNumber")]
        [TestCase(OrganisationType.PublicBodies, "PublicBodyName")]
        public async Task ThenGivenAnOrganisationTypeTheCorrespondingSearchTermMustBePopulated(OrganisationType organisationType, string searchTermPropertyName)
        {
            //Arrange
            var model = new AddLegalEntityViewModel
            {
                OrganisationType = organisationType
            };

            //Act
            var result = await _validator.ValidateAsync(model);

            //Assert
            Assert.IsFalse(result.IsValid);
            Assert.AreEqual(1, result.Errors.Count);
            Assert.IsNotNull(result.Errors.SingleOrDefault(x => x.PropertyName == searchTermPropertyName));
        }

        [Test]
        public async Task ThenCharityRegistrationNumberMustBeNumeric()
        {
            //Arrange
            var invalidModel = new AddLegalEntityViewModel
            {
                OrganisationType = OrganisationType.Charities,
                CharityRegistrationNumber = "SOMESTRING"
            };

            var validModel = new AddLegalEntityViewModel
            {
                OrganisationType = OrganisationType.Charities,
                CharityRegistrationNumber = "12345"
            };

            //Act
            var invalidResult = await _validator.ValidateAsync(invalidModel);
            var validResult = await _validator.ValidateAsync(validModel);

            //Assert
            Assert.IsFalse(invalidResult.IsValid);
            Assert.AreEqual(1, invalidResult.Errors.Count);
            Assert.IsNotNull(invalidResult.Errors.SingleOrDefault(x => x.PropertyName == "CharityRegistrationNumber"));

            Assert.IsTrue(validResult.IsValid);

        }

        [Test]
        public async Task ThenCharityRegistrationNumberMustBeAnInt32()
        {
            //Arrange
            var invalidModel = new AddLegalEntityViewModel
            {
                OrganisationType = OrganisationType.Charities,
                CharityRegistrationNumber = "99999999999"
            };

            //Act
            var invalidResult = await _validator.ValidateAsync(invalidModel);

            //Assert
            Assert.IsFalse(invalidResult.IsValid);
            Assert.AreEqual(1, invalidResult.Errors.Count);
            Assert.IsNotNull(invalidResult.Errors.SingleOrDefault(x => x.PropertyName == "CharityRegistrationNumber"));  
        }
    }
}
