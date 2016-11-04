using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using MediatR;
using Moq;
using NUnit.Framework;
using SFA.DAS.EAS.Application.Commands.RefreshEmployerLevyData;
using SFA.DAS.EAS.Application.Events;
using SFA.DAS.EAS.Application.Validation;
using SFA.DAS.EAS.Domain.Data;
using SFA.DAS.EAS.Domain.Models.Levy;
using SFA.DAS.EAS.TestCommon.ObjectMothers;

namespace SFA.DAS.EAS.Application.UnitTests.Commands.RefreshEmployerLevyDataTests
{
    public class WhenIReceiveTheCommand
    {
        private RefreshEmployerLevyDataCommandHandler _refreshEmployerLevyDataCommandHandler;
        private Mock<IValidator<RefreshEmployerLevyDataCommand>> _validator;
        private Mock<IDasLevyRepository> _levyRepository;
        private Mock<IEnglishFractionRepository> _englishFractionRepository;
        private Mock<IMediator> _mediator;
        private const string ExpectedEmpRef = "123456";
        private const long ExpectedAccountId = 44321;

        [SetUp]
        public void Arrange()
        {
            _levyRepository = new Mock<IDasLevyRepository>();
            _levyRepository.Setup(x => x.GetLastSubmissionForScheme(ExpectedEmpRef)).ReturnsAsync(new DasDeclaration { LevyDueYtd = 1000m, LevyAllowanceForFullYear = 1200m });

            _englishFractionRepository = new Mock<IEnglishFractionRepository>();
            _validator = new Mock<IValidator<RefreshEmployerLevyDataCommand>>();
            _validator.Setup(x => x.Validate(It.IsAny<RefreshEmployerLevyDataCommand>())).Returns(new ValidationResult());
            
            _mediator = new Mock<IMediator>();

            _refreshEmployerLevyDataCommandHandler = new RefreshEmployerLevyDataCommandHandler(
                _validator.Object, _levyRepository.Object, _englishFractionRepository.Object, _mediator.Object);

        }

        [Test]
        public async Task ThenTheValidatorIsCalled()
        {
            //Act
            await _refreshEmployerLevyDataCommandHandler.Handle(RefreshEmployerLevyDataCommandObjectMother.Create(ExpectedEmpRef));

            //Assert
            _validator.Verify(x => x.Validate(It.IsAny<RefreshEmployerLevyDataCommand>()));
        }

        [Test]
        public void ThenAInvalidRequestExceptionIsThrownIfTheMessageIsNotValid()
        {
            //Arrange
            _validator.Setup(x => x.Validate(It.IsAny<RefreshEmployerLevyDataCommand>())).Returns(new ValidationResult { ValidationDictionary = new Dictionary<string, string> { { "", "" } } });

            //Act
            Assert.ThrowsAsync<InvalidRequestException>(async () => await _refreshEmployerLevyDataCommandHandler.Handle(new RefreshEmployerLevyDataCommand()));
        }

        [Test]
        public async Task ThenDataIsTakenFromTheDataStoreForTheEmployerRefAndEachDeclarationRow()
        {
            //Arrange
            var refreshEmployerLevyDataCommand = RefreshEmployerLevyDataCommandObjectMother.Create(ExpectedEmpRef);

            //Act
            await _refreshEmployerLevyDataCommandHandler.Handle(refreshEmployerLevyDataCommand);

            //Assert
            _levyRepository.Verify(x => x.GetEmployerDeclaration(It.Is<string>(c => c.Equals("1") || c.Equals("2") || c.Equals("4")), ExpectedEmpRef), Times.Exactly(refreshEmployerLevyDataCommand.EmployerLevyData[0].Declarations.Declarations.Count(c=>!c.NoPaymentForPeriod)));
        }

        [Test]
        public async Task ThenEveryFractionIsAddedToTheRepository()
        {
            //Act
            var refreshEmployerLevyDataCommand = RefreshEmployerLevyDataCommandObjectMother.Create(ExpectedEmpRef, ExpectedAccountId);
            await _refreshEmployerLevyDataCommandHandler.Handle(refreshEmployerLevyDataCommand);

            //Assert
            _englishFractionRepository.Verify(x=>x.CreateEmployerFraction(It.IsAny<DasEnglishFraction>(),ExpectedEmpRef),Times.Exactly(refreshEmployerLevyDataCommand.EmployerLevyData[0].Fractions.Fractions.Count));
        }

        [Test]
        public async Task ThenTheLevyRepositoryIsUpdatedIfTheDeclarationDoesNotExist()
        {
            //Arrange
            _levyRepository.Setup(x => x.GetEmployerDeclaration("1", ExpectedEmpRef)).ReturnsAsync(null);
            _levyRepository.Setup(x => x.GetEmployerDeclaration("2", ExpectedEmpRef)).ReturnsAsync(new DasDeclaration());

            //Act
            await _refreshEmployerLevyDataCommandHandler.Handle(RefreshEmployerLevyDataCommandObjectMother.Create(ExpectedEmpRef, ExpectedAccountId));

            //Assert
            _levyRepository.Verify(x => x.CreateEmployerDeclaration(It.IsAny<DasDeclaration>(), ExpectedEmpRef, ExpectedAccountId));
        }
        

        [Test]
        public async Task ThenIfTheDeclarationHasNoSubmissionForPeriodThenTheLAstSubmissionIsReadFromTheRepository()
        {
            //Arrange
            var data = RefreshEmployerLevyDataCommandObjectMother.Create(ExpectedEmpRef, ExpectedAccountId);

            //Act
            await _refreshEmployerLevyDataCommandHandler.Handle(data);

            //Assert
            _levyRepository.Verify(x=>x.GetLastSubmissionForScheme(ExpectedEmpRef),Times.Exactly(data.EmployerLevyData[0].Declarations.Declarations.Count(c=>c.NoPaymentForPeriod)));
        }

        [Test]
        public async Task ThenIfTheDeclarationHasNoSubmissionThenTheLevyDueYtdIsTakenFromThePreviousSubmission()
        {
            //Act
            var data = RefreshEmployerLevyDataCommandObjectMother.Create(ExpectedEmpRef, ExpectedAccountId);
            await _refreshEmployerLevyDataCommandHandler.Handle(data);

            //Assert
            _levyRepository.Verify(x=>x.CreateEmployerDeclaration(It.Is<DasDeclaration>(c=>c.NoPaymentForPeriod && c.LevyDueYtd.Equals(1000) && c.LevyAllowanceForFullYear.Equals(1200m)),ExpectedEmpRef,ExpectedAccountId),Times.Once);
        }

        [Test]
        public async Task ThenIfThereAreDeclarationsToProcessTheProcessDeclarationsEventIsPublished()
        {
            //Arrange
            var data = RefreshEmployerLevyDataCommandObjectMother.Create(ExpectedEmpRef, ExpectedAccountId);

            //Act
            await _refreshEmployerLevyDataCommandHandler.Handle(data);

            //Assert
            _mediator.Verify(x => x.PublishAsync(It.IsAny<ProcessDeclarationsEvent>()), Times.Once);
        }

        [Test]
        public async Task ThenIfThereAreNoNewDeclarationsThenTheProcessDeclarationEventIsNotPublished()
        {
            //Arrange
            _levyRepository.Setup(x => x.GetEmployerDeclaration(It.IsAny<string>(), ExpectedEmpRef)).ReturnsAsync(new DasDeclaration());
            _englishFractionRepository.Setup(x => x.GetEmployerFraction(It.IsAny<DateTime>(), It.IsAny<string>())).ReturnsAsync(new DasEnglishFraction());
            var data = RefreshEmployerLevyDataCommandObjectMother.Create(ExpectedEmpRef, ExpectedAccountId);

            //Act
            await _refreshEmployerLevyDataCommandHandler.Handle(data);

            //Assert
            _mediator.Verify(x => x.PublishAsync(It.IsAny<ProcessDeclarationsEvent>()), Times.Never);
        }
    }
}
