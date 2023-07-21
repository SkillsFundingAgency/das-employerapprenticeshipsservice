using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Moq;
using NUnit.Framework;
using SFA.DAS.EmployerAccounts.Commands.CreateOrganisationAddress;

namespace SFA.DAS.EmployerAccounts.UnitTests.Commands.CreateOrganisationAddressTests
{
    class WhenICreateAnAddress
    {
        private CreateOrganisationAddressHandler _handler;
        private Mock<IValidator<CreateOrganisationAddressRequest>> _validator;
        private CreateOrganisationAddressRequest _request;

        [SetUp]
        public void Arrange()
        {
            _request = new CreateOrganisationAddressRequest
            {
                AddressFirstLine = "123, Test Lane",
                AddressSecondLine = "Test Garden",
                AddressThirdLine = "Testing",
                TownOrCity = "Test Town",
                County = "Testshire",
                Postcode = "TE12 3ST"
            };

            _validator = new Mock<IValidator<CreateOrganisationAddressRequest>>();

            _validator.Setup(x => x.Validate(It.IsAny<CreateOrganisationAddressRequest>()))
              .Returns(new ValidationResult());

            _handler = new CreateOrganisationAddressHandler(_validator.Object);
        }

        [Test]
        public async Task ThenIShouldGetBackAAddressInAValidFormatIfTheAddressIsValid()
        {
            //Arange
            var expectedAddress = $"{_request.AddressFirstLine}, {_request.AddressSecondLine}, {_request.AddressThirdLine}, " +
                                  $"{_request.TownOrCity}, {_request.County}, {_request.Postcode}";

            //Act
            var response = await _handler.Handle(_request, It.IsAny<CancellationToken>());

            //Assert
            _validator.Verify(x => x.Validate(_request), Times.Once);
            Assert.AreEqual(expectedAddress, response.Address);
        }

        [Test]
        public void ThenIShouldGetAInvalidRequestExceptionIfValidationFails()
        {
            //Arange
            _validator.Setup(x => x.Validate(It.IsAny<CreateOrganisationAddressRequest>()))
                .Returns(new ValidationResult
                {
                    ValidationDictionary = new Dictionary<string, string>
                    {
                        { "test", "test"}
                    }
                });

            //Act + Assert
            Assert.ThrowsAsync<InvalidRequestException>(() => _handler.Handle(_request, It.IsAny<CancellationToken>()));
        }

        [Test]
        public async Task ThenShouldNotAddAddressFieldsThatAreOptionalAndMissing()
        {
            //Arange
            var expectedAddress = $"{_request.AddressFirstLine}, {_request.TownOrCity}, {_request.Postcode}";

            _request = new CreateOrganisationAddressRequest
            {
                AddressFirstLine = "123, Test Lane",
                TownOrCity = "Test Town",
                Postcode = "TE12 3ST"
            };

            //Act
            var response = await _handler.Handle(_request, CancellationToken.None);

            //Assert
            Assert.AreEqual(expectedAddress, response.Address);
        }
    }
}
