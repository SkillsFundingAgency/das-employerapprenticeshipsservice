using System;
using System.Threading.Tasks;
using Moq;
using NUnit.Framework;
using SFA.DAS.EAS.Application.Queries.GetEnglishFractionUpdateRequired;
using SFA.DAS.EAS.Domain.Data;
using SFA.DAS.EAS.Domain.Interfaces;

namespace SFA.DAS.EAS.Application.UnitTests.Queries.GetEnglishFractionsUpdateRequired
{
    class WhenGettingIfEnglishFractionsUpdateIsRequired
    {
        private GetEnglishFractionsUpdateRequiredQueryHandler _handler;
        private Mock<IHmrcService> _hmrcService;
        private Mock<IEnglishFractionRepository> _englishFractionRepository;

        [SetUp]
        public void Arrange()
        {
            _hmrcService = new Mock<IHmrcService>();
            _englishFractionRepository = new Mock<IEnglishFractionRepository>();
            _handler = new GetEnglishFractionsUpdateRequiredQueryHandler(_hmrcService.Object, _englishFractionRepository.Object);    
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
    }
}
