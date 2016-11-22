using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Moq;
using NUnit.Framework;
using SFA.DAS.EAS.Application.Queries.GetApprenticeshipDetails;
using SFA.DAS.EAS.Application.Validation;
using SFA.DAS.EAS.Domain;
using SFA.DAS.EAS.Domain.Interfaces;

namespace SFA.DAS.EAS.Application.UnitTests.Queries.GetApprenticeshipDetails
{
    class WhenIGetApprenticeshipDetails : QueryBaseTest<GetApprenticeshipDetailsHandler, GetApprenticeshipDetailsQuery, GetApprenticeshipDetailsResponse>
    {
        private Mock<IApprenticeshipInfoServiceWrapper> _apprenticeshipInfoService;
        private Provider _provider;
        
        public override GetApprenticeshipDetailsQuery Query { get; set; }
        public override GetApprenticeshipDetailsHandler RequestHandler { get; set; }
        public override Mock<IValidator<GetApprenticeshipDetailsQuery>> RequestValidator { get; set; }

        [SetUp]
        public void Arrange()
        {
            base.SetUp();

            _provider = new Provider
            {
                Name = "Test Provider"
            };

            _apprenticeshipInfoService = new Mock<IApprenticeshipInfoServiceWrapper>();

            _apprenticeshipInfoService.Setup(x => x.GetProvider(It.IsAny<int>())).Returns(new ProvidersView
            {
                CreatedDate = DateTime.Now,
                Providers = new List<Provider>
                {
                    _provider
                }
            });

            Query = new GetApprenticeshipDetailsQuery
            {
                ApprenticeshipId = 1,
                ProviderId = 12
            };

            RequestHandler = new GetApprenticeshipDetailsHandler(RequestValidator.Object, _apprenticeshipInfoService.Object);
        }

        public override async Task ThenIfTheMessageIsValidTheRepositoryIsCalled()
        {
            //Act
            await RequestHandler.Handle(Query);

            //Assert
            _apprenticeshipInfoService.Verify(x => x.GetProvider(Query.ProviderId), Times.Once);
        }

        public override async Task ThenIfTheMessageIsValidTheValueIsReturnedInTheResponse()
        {
            //Act
            var result = await RequestHandler.Handle(Query);

            //Assert
            Assert.AreEqual(_provider.Name, result.ProviderName);
        }

        [Test]
        public async Task ThenIfNoProvidersAreReturnsAnUnknownProviderLabelShouldBeAdded()
        {
            _apprenticeshipInfoService.Setup(x => x.GetProvider(It.IsAny<int>())).Returns(new ProvidersView
            {
                CreatedDate = DateTime.Now,
                Providers = new List<Provider>()
            });

            //Act
            var result = await RequestHandler.Handle(Query);

            //Assert
            Assert.AreEqual("Unknown provider", result.ProviderName);
        }

        [Test]
        public async Task ThenIfNoProviderIsFoundUnknownProviderLabelShouldBeAdded()
        {
            _apprenticeshipInfoService.Setup(x => x.GetProvider(It.IsAny<int>())).Returns(() => null);

            //Act
            var result = await RequestHandler.Handle(Query);

            //Assert
            Assert.AreEqual("Unknown provider", result.ProviderName);
        }
    }
}
