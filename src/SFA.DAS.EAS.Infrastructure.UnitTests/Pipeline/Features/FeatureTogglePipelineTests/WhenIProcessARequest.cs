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
        private FeatureToggleRequest _request;
        private FeatureTogglePipeline _pipeline;
        private Mock<IPipelineSection<FeatureToggleRequest, bool>> _pipeSection;

        [SetUp]
        public void Arrange()
        {
            _request = new FeatureToggleRequest();

            _pipeSection = new Mock<IPipelineSection<FeatureToggleRequest, bool>>();

            _pipeSection.Setup(x => x.Priority).Returns(1);
            _pipeSection.Setup(x => x.ProcessAsync(It.IsAny<FeatureToggleRequest>())).ReturnsAsync(true);

            var sections = new List<IPipelineSection<FeatureToggleRequest, bool>>
            {
                _pipeSection.Object
            };

            _pipeline = new FeatureTogglePipeline(sections, Mock.Of<ILog>());
        }

        [Test]
        public async Task ThenACorrectResponseShouldBeReturned()
        {
            //Act
            var result = await _pipeline.ProcessAsync(_request);

            //Assert
            Assert.IsTrue(result);
        }

        [Test]
        public async Task ThenAllSectionsShouldBeProcessed()
        {
            //Arrange
            var sectionMocks = new List<Mock<IPipelineSection<FeatureToggleRequest, bool>>>
            {
                new Mock<IPipelineSection<FeatureToggleRequest, bool>>(),
                new Mock<IPipelineSection<FeatureToggleRequest, bool>>(),
                new Mock<IPipelineSection<FeatureToggleRequest, bool>>()
            };

            foreach (var mock in sectionMocks)
            {
                mock.Setup(x => x.ProcessAsync(It.IsAny<FeatureToggleRequest>())).ReturnsAsync(true);
            }

            _pipeline = new FeatureTogglePipeline(sectionMocks.Select(x => x.Object), Mock.Of<ILog>());

            //Act
            await _pipeline.ProcessAsync(_request);

            //Assert
            foreach (var mock in sectionMocks)
            {
                mock.Verify(x => x.ProcessAsync(_request), Times.Once);
            }
        }

        [Test]
        public async Task ThenSectionsShouldBeProcessedInPriorityOrder()
        {
            //Arrange
            var firstSectionCalled = false;
            var secondSectionCalled = false;
            var thirdSectionCalled = false;


            var firstSection = new Mock<IPipelineSection<FeatureToggleRequest, bool>>();
            var secondSection = new Mock<IPipelineSection<FeatureToggleRequest, bool>>();
            var thirdSection = new Mock<IPipelineSection<FeatureToggleRequest, bool>>();

            firstSection.Setup(x => x.Priority).Returns(1);
            secondSection.Setup(x => x.Priority).Returns(2);
            thirdSection.Setup(x => x.Priority).Returns(3);

            firstSection.Setup(x => x.ProcessAsync(It.IsAny<FeatureToggleRequest>()))
                .ReturnsAsync(true)
                .Callback(() => { firstSectionCalled = true; });

            secondSection.Setup(x => x.ProcessAsync(It.IsAny<FeatureToggleRequest>()))
                .ReturnsAsync(true)
                .Callback(() => { secondSectionCalled = firstSectionCalled; });

            thirdSection.Setup(x => x.ProcessAsync(It.IsAny<FeatureToggleRequest>()))
                .ReturnsAsync(true)
                .Callback(() => { thirdSectionCalled = secondSectionCalled; });

            var sectionMocks = new List<Mock<IPipelineSection<FeatureToggleRequest, bool>>>
            {
                thirdSection,
                firstSection,
                secondSection
            };

            _pipeline = new FeatureTogglePipeline(sectionMocks.Select(x => x.Object), Mock.Of<ILog>());

            //Act
            await _pipeline.ProcessAsync(_request);

            //Assert
            Assert.IsTrue(thirdSectionCalled);
        }

        [Test]
        public async Task ThenIfSectionReturnsFalseAllRemainingSectionAreNotCalled()
        {
            //Arrange
            var firstSection = new Mock<IPipelineSection<FeatureToggleRequest, bool>>();
            var secondSection = new Mock<IPipelineSection<FeatureToggleRequest, bool>>();
            var thirdSection = new Mock<IPipelineSection<FeatureToggleRequest, bool>>();

            firstSection.Setup(x => x.Priority).Returns(1);
            secondSection.Setup(x => x.Priority).Returns(2);
            thirdSection.Setup(x => x.Priority).Returns(3);

            firstSection.Setup(x => x.ProcessAsync(It.IsAny<FeatureToggleRequest>()))
                        .ReturnsAsync(false);

            var sectionMocks = new List<Mock<IPipelineSection<FeatureToggleRequest, bool>>>
            {
                thirdSection,
                firstSection,
                secondSection
            };

            _pipeline = new FeatureTogglePipeline(sectionMocks.Select(x => x.Object), Mock.Of<ILog>());

            //Act
            await _pipeline.ProcessAsync(_request);

            //Assert
            firstSection.Verify(x => x.ProcessAsync(_request), Times.Once);
            secondSection.Verify(x => x.ProcessAsync(_request), Times.Never);
            thirdSection.Verify(x => x.ProcessAsync(_request), Times.Never);
        }
    }
}
