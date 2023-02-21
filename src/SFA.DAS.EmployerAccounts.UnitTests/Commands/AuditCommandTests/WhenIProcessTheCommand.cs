using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Moq;
using NUnit.Framework;
using SFA.DAS.EmployerAccounts.Audit.Types;
using SFA.DAS.EmployerAccounts.Commands.AuditCommand;
using SFA.DAS.EmployerAccounts.Interfaces;

namespace SFA.DAS.EmployerAccounts.UnitTests.Commands.AuditCommandTests
{
    public class WhenIProcessTheCommand
    {
        private Mock<IAuditService> _auditService;
        private CreateAuditCommandHandler _createAuditCommandHandler;
        private Mock<IValidator<CreateAuditCommand>> _validator;

        [SetUp]
        public void Arrange()
        {
            _auditService = new Mock<IAuditService>();
            _validator = new Mock<IValidator<CreateAuditCommand>>();
            _validator.Setup(x => x.Validate(It.IsAny<CreateAuditCommand>())).Returns(new ValidationResult { ValidationDictionary = new Dictionary<string, string> ()});

            _createAuditCommandHandler = new CreateAuditCommandHandler(_auditService.Object, _validator.Object);
        }

        [Test]
        public async Task ThenTheCommandIsValidated()
        {
            //Act
            await _createAuditCommandHandler.Handle(new CreateAuditCommand(), CancellationToken.None);

            //Assert
            _validator.Verify(x=>x.Validate(It.IsAny<CreateAuditCommand>()),Times.Once);
        }

        [Test]
        public void ThenIfTheCommandIsNotValidAnInvalidRequestExceptionIsThrown()
        {
            //Arrange
            _validator.Setup(x => x.Validate(It.IsAny<CreateAuditCommand>())).Returns(new ValidationResult {ValidationDictionary = new Dictionary<string, string> {{"", ""}}});

            //Act Assert
            Assert.ThrowsAsync<InvalidRequestException>(async () => await _createAuditCommandHandler.Handle(new CreateAuditCommand(), CancellationToken.None));
        }

        [Test]
        public async Task ThenTheCommandIfValidIsPassedToTheService()
        {
            //Arrange
            var auditCommand = new CreateAuditCommand {EasAuditMessage = new AuditMessage {ChangedProperties = new List<PropertyUpdate> {new PropertyUpdate()},Description = "test", RelatedEntities = new List<AuditEntity> {new AuditEntity()} } };

            //Act
            await _createAuditCommandHandler.Handle(auditCommand, CancellationToken.None);

            //Assert
            _auditService.Verify(x=>x.SendAuditMessage(It.Is<AuditMessage>(c=>
                        c.Description.Equals(auditCommand.EasAuditMessage.Description) &&
                        c.ChangedProperties.Count == 1 &&
                        c.RelatedEntities.Count == 1 
                        )));
        }

    }
}
