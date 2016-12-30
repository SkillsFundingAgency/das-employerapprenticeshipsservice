using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Moq;
using NUnit.Framework;
using SFA.DAS.EAS.Application.Queries.GetCharity;
using SFA.DAS.EAS.Application.Validation;
using SFA.DAS.EAS.Domain.Interfaces;
using SFA.DAS.EAS.Domain.Models.ReferenceData;

namespace SFA.DAS.EAS.Application.UnitTests.Queries.GetCharity
{
    public class WhenIGetCharity: QueryBaseTest<GetCharityQueryHandler, GetCharityQueryRequest, GetCharityQueryResponse>
    {
        public override GetCharityQueryRequest Query { get; set; }
        public override GetCharityQueryHandler RequestHandler { get; set; }
        public override Mock<IValidator<GetCharityQueryRequest>> RequestValidator { get; set; }

        private Mock<IReferenceDataService> _referenceDataService;

        [SetUp]
        public void Arrange()
        {
            base.SetUp();

            _referenceDataService = new Mock<IReferenceDataService>();

            _referenceDataService.Setup(x => x.GetCharity(It.IsAny<int>())).ReturnsAsync(new Charity
            {
                RegistrationNumber = 12345,
                Name = "Test Charity"
            });

            

            RequestHandler = new GetCharityQueryHandler(_referenceDataService.Object,RequestValidator.Object);

            Query = new GetCharityQueryRequest
            {
                RegistrationNumber = 12345
            };
        }

        [Test]
        public override async Task ThenIfTheMessageIsValidTheRepositoryIsCalled()
        {
            //Act
            await RequestHandler.Handle(Query);

            //Assert
            _referenceDataService.Verify(x=> x.GetCharity(It.IsAny<int>()), Times.Once);
        }

        [Test]
        public override async Task ThenIfTheMessageIsValidTheValueIsReturnedInTheResponse()
        {
            //Act
            var response = await RequestHandler.Handle(Query);
           
            //Assert
            Assert.IsNotNull(response.Charity);
            Assert.AreEqual(12345, response.Charity.RegistrationNumber);
        }
    }
}
