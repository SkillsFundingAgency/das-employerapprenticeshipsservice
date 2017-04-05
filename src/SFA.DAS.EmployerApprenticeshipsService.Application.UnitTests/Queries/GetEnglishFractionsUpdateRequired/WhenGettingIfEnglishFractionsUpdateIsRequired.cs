using System;
using System.Threading.Tasks;
using Moq;
using NUnit.Framework;
using SFA.DAS.EAS.Application.Queries.GetEnglishFractionUpdateRequired;
using SFA.DAS.EAS.Domain.Data.Repositories;
using SFA.DAS.EAS.Domain.Interfaces;
using SFA.DAS.EAS.Infrastructure.Caching;

namespace SFA.DAS.EAS.Application.UnitTests.Queries.GetEnglishFractionsUpdateRequired
{
    class WhenGettingIfEnglishFractionsUpdateIsRequired
    {
        private GetEnglishFractionsUpdateRequiredQueryHandler _handler;
        private Mock<IHmrcService> _hmrcService;
        private Mock<IEnglishFractionRepository> _englishFractionRepository;
        private Mock<ICacheProvider> _cacheProvider;

        [SetUp]
        public void Arrange()
        {
            _hmrcService = new Mock<IHmrcService>();
            _englishFractionRepository = new Mock<IEnglishFractionRepository>();

            _cacheProvider = new Mock<ICacheProvider>();
            _cacheProvider.SetupSequence(c => c.Get<DateTime?>("HmrcFractionLastCalculatedDate"))
                .Returns(null)
                .Returns(new DateTime());

            _handler = new GetEnglishFractionsUpdateRequiredQueryHandler(_hmrcService.Object, _englishFractionRepository.Object, _cacheProvider.Object);    
        }

        [Test]
        public async Task ThenIShouldGetThatAnUpdateIsRequiredIfIDoNotHaveTheLatestUpdate()
        {
            //Assign
            _hmrcService.Setup(x => x.GetLastEnglishFractionUpdate()).ReturnsAsync(DateTime.Now);
            _englishFractionRepository.Setup(x => x.GetLastUpdateDate()).ReturnsAsync(DateTime.Now.AddDays(-1));

            //Act
            var result = await _handler.Handle(new GetEnglishFractionUpdateRequiredRequest());

            //Assert
            Assert.IsTrue(result.UpdateRequired); 
        }

        [Test]
        public async Task ThenIShouldGetThatAnUpdateIsNotRequiredIfIDoHaveTheLatestUpdate()
        {
            //Assign
            var updateTime = DateTime.Now;
            _hmrcService.Setup(x => x.GetEnglishFractions(It.IsAny<string>()));
            _hmrcService.Setup(x => x.GetLastEnglishFractionUpdate()).ReturnsAsync(updateTime);
            _englishFractionRepository.Setup(x => x.GetLastUpdateDate()).ReturnsAsync(updateTime);

            //Act
            var result = await _handler.Handle(new GetEnglishFractionUpdateRequiredRequest());

            //Assert
            Assert.IsFalse(result.UpdateRequired);
        }

        [Test]
        public async Task ThenTheFractionLastCaclulatedDateIsReadFromTheCacheOnSubsequentReads()
        {
            //Act
            await _handler.Handle(new GetEnglishFractionUpdateRequiredRequest());
            await _handler.Handle(new GetEnglishFractionUpdateRequiredRequest());

            //Assert
            _hmrcService.Verify(x=>x.GetLastEnglishFractionUpdate(), Times.Once);
            _cacheProvider.Verify(x=>x.Set("HmrcFractionLastCalculatedDate",It.IsAny<DateTime>(),It.Is<TimeSpan>(c=>c.Days.Equals(1))));
        }
    }
}
