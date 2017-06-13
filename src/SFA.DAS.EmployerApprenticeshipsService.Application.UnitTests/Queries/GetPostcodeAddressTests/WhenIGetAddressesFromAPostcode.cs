using System.Collections.Generic;
using System.Threading.Tasks;
using Moq;
using NUnit.Framework;
using SFA.DAS.EAS.Application.Queries.GetPostcodeAddress;
using SFA.DAS.EAS.Application.Validation;
using SFA.DAS.EAS.Domain.Interfaces;
using SFA.DAS.EAS.Domain.Models.Employer;
using SFA.DAS.NLog.Logger;

namespace SFA.DAS.EAS.Application.UnitTests.Queries.GetPostcodeAddressTests
{
    public class WhenIGetAddressesFromAPostcode : QueryBaseTest<GetPostcodeAddressHandler,GetPostcodeAddressRequest,GetPostcodeAddressResponse>
    {
        private Mock<IAddressLookupService> _addressLookupService;
        private Mock<ILog> _logger;
        private ICollection<Address> _addresses;

        public override GetPostcodeAddressRequest Query { get; set; }
        public override GetPostcodeAddressHandler RequestHandler { get; set; }
        public override Mock<IValidator<GetPostcodeAddressRequest>> RequestValidator { get; set; }

       
        [SetUp]
        public void Arrange()
        {
            base.SetUp();

            _addresses = new List<Address>
            {
                new Address()
            };

            _addressLookupService = new Mock<IAddressLookupService>();
            _logger = new Mock<ILog>();

            _addressLookupService.Setup(x => x.GetAddressesByPostcode(It.IsAny<string>()))
                                 .ReturnsAsync(_addresses);

            Query = new GetPostcodeAddressRequest {Postcode = "TE12 3ST"};

            RequestHandler = new GetPostcodeAddressHandler(
                _addressLookupService.Object, 
                RequestValidator.Object, 
                _logger.Object);
        }
        
        [Test]
        public override async Task ThenIfTheMessageIsValidTheRepositoryIsCalled()
        {
            //Act
            await RequestHandler.Handle(Query);
            
            //Assert
            _addressLookupService.Verify(x => x.GetAddressesByPostcode(Query.Postcode), Times.Once);
        }

        [Test]
        public override async Task ThenIfTheMessageIsValidTheValueIsReturnedInTheResponse()
        {
            //Act
            var result = await RequestHandler.Handle(Query);

            //Assert
            Assert.AreEqual(_addresses, result.Addresses);
        }
    }
}
