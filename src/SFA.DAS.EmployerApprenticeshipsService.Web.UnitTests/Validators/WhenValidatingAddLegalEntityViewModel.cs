using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NUnit.Framework;
using SFA.DAS.EAS.Domain;
using SFA.DAS.EAS.Web.Models;
using SFA.DAS.EAS.Web.Validators;

namespace SFA.DAS.EAS.Web.UnitTests.Validators
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

    }
}
