using Moq;
using NUnit.Framework;
using SFA.DAS.EAS.Domain.Models.FeatureToggles;
using SFA.DAS.EAS.Infrastructure.Pipeline;
using SFA.DAS.EAS.Infrastructure.Pipeline.Features;
using SFA.DAS.NLog.Logger;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SFA.DAS.EAS.Infrastructure.UnitTests.Pipeline.Features.FeatureTogglePipelineTests
{
    class WhenIProcessARequest
    {
        private OperationContext _operationContext;
        private OperationAuthorisation _operationAuthorisation;
        private Mock<IOperationAuthorisationHandler> _pipeSection;

        [SetUp]
        public void Arrange()
        {
            _operationContext = new OperationContext();

            _pipeSection = new Mock<IOperationAuthorisationHandler>();

            _pipeSection.Setup(x => x.CanAccessAsync(It.IsAny<OperationContext>())).ReturnsAsync(true);

            var sections = new[]
            {
                _pipeSection.Object
            };

            _operationAuthorisation = new OperationAuthorisation(sections, Mock.Of<ILog>());
        }

        [Test]
        public async Task ThenACorrectResponseShouldBeReturned()
        {
            //Act
            var result = await _operationAuthorisation.CanAccessAsync(_operationContext);

            //Assert
            Assert.IsTrue(result);
        }

        [Test]
        public async Task ThenAllSectionsShouldBeProcessed()
        {
            //Arrange
            var sectionMocks = new []
            {
                new Mock<IOperationAuthorisationHandler>(),
                new Mock<IOperationAuthorisationHandler>(),
                new Mock<IOperationAuthorisationHandler>()
            };

            foreach (var mock in sectionMocks)
            {
                mock.Setup(x => x.CanAccessAsync(It.IsAny<OperationContext>())).ReturnsAsync(true);
            }

            _operationAuthorisation = new OperationAuthorisation(sectionMocks.Select(x => x.Object).ToArray(), Mock.Of<ILog>());

            //Act
            await _operationAuthorisation.CanAccessAsync(_operationContext);

            //Assert
            foreach (var mock in sectionMocks)
            {
                mock.Verify(x => x.CanAccessAsync(_operationContext), Times.Once);
            }
        }

        [Test]
        public async Task ThenIfSectionReturnsFalseAllRemainingSectionAreNotCalled()
        {
            //Arrange
            var firstSection = new Mock<IOperationAuthorisationHandler>();
            var secondSection = new Mock<IOperationAuthorisationHandler>();
            var thirdSection = new Mock<IOperationAuthorisationHandler>();

            firstSection.Setup(x => x.CanAccessAsync(It.IsAny<OperationContext>()))
                        .ReturnsAsync(false);

            var sectionMocks = new List<Mock<IOperationAuthorisationHandler>>
            {
                firstSection,
                secondSection,
                thirdSection
            };

            _operationAuthorisation = new OperationAuthorisation(sectionMocks.Select(x => x.Object).ToArray(), Mock.Of<ILog>());

            //Act
            await _operationAuthorisation.CanAccessAsync(_operationContext);

            //Assert
            firstSection.Verify(x => x.CanAccessAsync(_operationContext), Times.Once);
            secondSection.Verify(x => x.CanAccessAsync(_operationContext), Times.Never);
            thirdSection.Verify(x => x.CanAccessAsync(_operationContext), Times.Never);
        }
    }
}
