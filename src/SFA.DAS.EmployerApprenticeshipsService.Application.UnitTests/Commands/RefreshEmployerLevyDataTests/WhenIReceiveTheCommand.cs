using System.Collections.Generic;
using System.Threading.Tasks;
using Moq;
using NUnit.Framework;
using SFA.DAS.EmployerApprenticeshipsService.Application.Commands.RefreshEmployerLevyData;
using SFA.DAS.EmployerApprenticeshipsService.Application.Messages;
using SFA.DAS.EmployerApprenticeshipsService.Application.Validation;
using SFA.DAS.EmployerApprenticeshipsService.Domain.Data;
using SFA.DAS.EmployerApprenticeshipsService.Domain.Models.Levy;
using SFA.DAS.EmployerApprenticeshipsService.TestCommon.ObjectMothers;
using SFA.DAS.Messaging;

namespace SFA.DAS.EmployerApprenticeshipsService.Application.UnitTests.Commands.RefreshEmployerLevyDataTests
{
    public class WhenIReceiveTheCommand
    {
        private RefreshEmployerLevyDataCommandHandler _refreshEmployerLevyDataCommandHandler;
        private Mock<IValidator<RefreshEmployerLevyDataCommand>> _validator;
        private Mock<IDasLevyRepository> _levyRepository;
        private Mock<IMessagePublisher> _messagePublisher;
        private const string ExpectedEmpRef = "123456";
        private const long ExpectedAccountId = 44321;

        [SetUp]
        public void Arrange()
        {
            _levyRepository = new Mock<IDasLevyRepository>();
            _validator = new Mock<IValidator<RefreshEmployerLevyDataCommand>>();
            _validator.Setup(x => x.Validate(It.IsAny<RefreshEmployerLevyDataCommand>())).Returns(new ValidationResult());
            _messagePublisher = new Mock<IMessagePublisher>();
            _refreshEmployerLevyDataCommandHandler = new RefreshEmployerLevyDataCommandHandler(_validator.Object, _levyRepository.Object, _messagePublisher.Object);

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
            _levyRepository.Verify(x => x.GetEmployerDeclaration(It.Is<string>(c => c.Equals("1") || c.Equals("2")), ExpectedEmpRef), Times.Exactly(refreshEmployerLevyDataCommand.EmployerLevyData[0].Declarations.Declarations.Count));
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
        public async Task ThenTheMessageIsPublishedIfThereAreChanges()
        {
            //Arrange
            _levyRepository.Setup(x => x.GetEmployerDeclaration("2", ExpectedEmpRef)).ReturnsAsync(new DasDeclaration());

            //Act
            await _refreshEmployerLevyDataCommandHandler.Handle(RefreshEmployerLevyDataCommandObjectMother.Create(ExpectedEmpRef, ExpectedAccountId));

            //Assert
            _messagePublisher.Verify(x=>x.PublishAsync(It.Is< EmployerRefreshLevyQueueMessage>(c=>c.AccountId.Equals(ExpectedAccountId))));
        }
    }
}
