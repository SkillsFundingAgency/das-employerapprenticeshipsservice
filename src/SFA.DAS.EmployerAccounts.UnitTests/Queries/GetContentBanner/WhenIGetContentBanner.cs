using System.Threading.Tasks;
using Moq;
using NUnit.Framework;
using SFA.DAS.EmployerAccounts.Interfaces;
using SFA.DAS.EmployerAccounts.Queries.GetContentBanner;
using SFA.DAS.NLog.Logger;
using SFA.DAS.Validation;

namespace SFA.DAS.EmployerAccounts.UnitTests.Queries.GetContentBanner
{
    public class WhenIGetContentBanner : QueryBaseTest<GetContentBannerRequestHandler, GetContentBannerRequest, GetContentBannerResponse>
    {
        public override GetContentBannerRequest Query { get; set; }
        public override GetContentBannerRequestHandler RequestHandler { get; set; }
        public override Mock<IValidator<GetContentBannerRequest>> RequestValidator { get; set; }

        private Mock<IContentBannerService> _contentBannerService;
        private string _contentBanner;
        private int _bannerId;
        private bool _useCDN;
        private Mock<ILog> _logger;

        [SetUp]
        public void Arrange()
        {
            SetUp();

            _contentBanner = "<p>find out how you can pause your apprenticeships<p>";
            _bannerId = 123;
            _useCDN = false;
            _logger = new Mock<ILog>();
            _contentBannerService = new Mock<IContentBannerService>();
            _contentBannerService
                .Setup(cbs => cbs.GetBannerContent(_bannerId, _useCDN))
                .ReturnsAsync(_contentBanner);

            RequestHandler = new GetContentBannerRequestHandler(RequestValidator.Object, _logger.Object, _contentBannerService.Object);

            Query = new GetContentBannerRequest
            {
                BannerId = _bannerId,
                UseCDN = _useCDN
            };
        }

        [Test]
        public async Task ThenIfTheMessageIsValidTheServiceIsCalled()
        {
            //Act
            await RequestHandler.Handle(Query);

            //Assert
            _contentBannerService.Verify(x => x.GetBannerContent(_bannerId,_useCDN), Times.Once);
        }

        public override Task ThenIfTheMessageIsValidTheRepositoryIsCalled()
        {
            throw new System.NotImplementedException();
        }

        [Test]
        public override async Task ThenIfTheMessageIsValidTheValueIsReturnedInTheResponse()
        {
            //Act
            var response = await RequestHandler.Handle(Query);

            //Assert
            Assert.AreEqual(_contentBanner, response.ContentBanner);
        }
    }
}
