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
            var result = _validator.Validate(request);

            //Assert
            Assert.Contains(new KeyValuePair<string, string>("Postcode", "Enter a valid postcode"), result.ValidationDictionary);
        }

        [Test]
        public void ThenPostCodesMustBeNoMoreThanEightCharactersInLength()
        {
            var request = new CreateOrganisationAddressRequest
            {
                AddressFirstLine = "123, Test Lane",
                AddressSecondLine = "Test Garden",
                TownOrCity = "Test Town",
                County = "Testshire",
                Postcode = "CV1 2WTXY"
            };

            //Act
            var result = _validator.Validate(request);

            //Assert
            Assert.Contains(new KeyValuePair<string, string>("Postcode", "Enter a valid postcode"), result.ValidationDictionary);
        }

        [Test]
        public void ThenPostCodesLeadingSpacesShouldBeIgnored()
        {
            var request = new CreateOrganisationAddressRequest
            {
                AddressFirstLine = "123, Test Lane",
                AddressSecondLine = "Test Garden",
                TownOrCity = "Test Town",
                County = "Testshire",
                Postcode = "     CV1 2WT"
            };

            //Act
            var result = _validator.Validate(request);

            //Assert
            Assert.IsTrue(result.IsValid());
        }

        [Test]
        public void ThenPostCodesTrailingSpacesAShouldBeIgnored()
        {
            var request = new CreateOrganisationAddressRequest
            {
                AddressFirstLine = "123, Test Lane",
                AddressSecondLine = "Test Garden",
                TownOrCity = "Test Town",
                County = "Testshire",
                Postcode = "CV1 2WT     "
            };

            //Act
            var result = _validator.Validate(request);

            //Assert
            Assert.IsTrue(result.IsValid());
        }

        [Test]
        public void ThenPostCodesWithoutSpacesButOtherwiseValidShouldBeAccepted()
        {
            var request = new CreateOrganisationAddressRequest
            {
                AddressFirstLine = "123, Test Lane",
                AddressSecondLine = "Test Garden",
                TownOrCity = "Test Town",
                County = "Testshire",
                Postcode = "CV12WT"
            };

            //Act
            var result = _validator.Validate(request);

            //Assert
            Assert.IsTrue(result.IsValid());
        }

        [Test]
        [TestCase("HD7 5UZ", true)]
        [TestCase("CH5 3QW", true)]
        [TestCase("GIR 0AA", true)]
        [TestCase("CB1 1AA", true)]
        [TestCase("CB11AA", true)]
        [TestCase("W2 1JB", true)]
        [TestCase("EC1A 1BB", true)]
        [TestCase("W1A 0AX", true)]
        [TestCase("M1 1AE", true)]
        [TestCase("B33 8TH", true)]
        [TestCase("CR2 6XH", true)]
        [TestCase("DN55 1PT", true)]
        [TestCase("DE55 4SW", true)]
        [TestCase("SW1A 2AA", true)]
        [TestCase("BS98 1TL", true)]
        [TestCase("", false)]
        [TestCase("B61", false)]
        [TestCase("W2 XXX", false)]
        [TestCase("X61 X11", false)]
        [TestCase("99 999", false)]
        public void ThenPostCodesShouldBeValidated(string postCode, bool isValid)
        {
            //Arrange
            var request = new CreateOrganisationAddressRequest
            {
                AddressFirstLine = "123, Test Lane",
                AddressSecondLine = "Test Garden",
                TownOrCity = "Test Town",
                County = "Testshire",
                Postcode = postCode
            };

            var requestWithoutSpaces = new CreateOrganisationAddressRequest
            {
                AddressFirstLine = "123, Test Lane",
                AddressSecondLine = "Test Garden",
                TownOrCity = "Test Town",
                County = "Testshire",
                Postcode = postCode.Replace(" ","")
            };

            //Act
            var result = _validator.Validate(request);
            var resultWithoutSpaces = _validator.Validate(requestWithoutSpaces);

            //Assert
            Assert.AreEqual(isValid, result.IsValid());
            Assert.AreEqual(isValid, resultWithoutSpaces.IsValid());
        }

    }
}
