using System.Collections.Generic;
using NUnit.Framework;
using SFA.DAS.EAS.Application.Commands.CreateOrganisationAddress;

namespace SFA.DAS.EAS.Application.UnitTests.Commands.CreateOrganisationAddressTests
{
    class WhenIValidateTheRequest
    {
        private CreateOrganisationAddressValidator _validator;

        [SetUp]
        public void Arrange()
        {
            _validator = new CreateOrganisationAddressValidator();
        }

        [Test]
        public void ThenIShouldGetNoErrorsIfAddressIsCorrect()
        {
            //Arange

            //Valid postcode and address for the validator
            var request = new CreateOrganisationAddressRequest
            {
                AddressFirstLine = "Further Education Funding Council",
                AddressSecondLine = "Quinton Road",
                TownOrCity = "Coventry",
                County = "West Midlands",
                Postcode = "CV1 2WT"
            };

            //Act
            var result = _validator.Validate(request);

            //Assert
            Assert.IsTrue(result.IsValid());
        }

        [Test]
        public void ThenIShouldGetNoErrorsIfpostcodeIsLowercase()
        {
            //Arange

            //Valid postcode and address for the validator
            var request = new CreateOrganisationAddressRequest
            {
                AddressFirstLine = "Further Education Funding Council",
                AddressSecondLine = "Quinton Road",
                TownOrCity = "Coventry",
                County = "West Midlands",
                Postcode = "cv1 2wt"
            };

            //Act
            var result = _validator.Validate(request);

            //Assert
            Assert.IsTrue(result.IsValid());
        }

        [Test]
        public void ThenIShouldGetErrorsWhenAddressIsEmpty()
        {
            //Act
            var result = _validator.Validate(new CreateOrganisationAddressRequest());

            //Assert
            Assert.Contains(new KeyValuePair<string, string>("AddressFirstLine", "Enter house number or name, building or street"), result.ValidationDictionary);
            Assert.Contains(new KeyValuePair<string, string>("TownOrCity", "Enter town or city"), result.ValidationDictionary);
            Assert.Contains(new KeyValuePair<string, string>("Postcode", "Enter a valid postcode"), result.ValidationDictionary);
        }

        [Test]
        public void ThenIShouldGetErrorWhenAddressPostcodeIsInvalid()
        {
            var request = new CreateOrganisationAddressRequest
            {
                AddressFirstLine = "123, Test Lane",
                AddressSecondLine = "Test Garden",
                TownOrCity = "Test Town",
                County = "Testshire",
                Postcode = "TEST"
            };

            //Act
            var result = _validator.Validate(new CreateOrganisationAddressRequest());

            //Assert
            Assert.Contains(new KeyValuePair<string, string>("Postcode", "Enter a valid postcode"), result.ValidationDictionary);
        }
    }
}
