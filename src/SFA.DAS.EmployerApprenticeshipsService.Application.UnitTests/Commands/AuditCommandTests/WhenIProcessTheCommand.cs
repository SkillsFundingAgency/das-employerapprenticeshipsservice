using System.Collections.Generic;
using System.Threading.Tasks;
using Moq;
using NUnit.Framework;
using SFA.DAS.Audit.Types;
using SFA.DAS.EAS.Application.Commands.AuditCommand;
using SFA.DAS.EAS.Application.Validation;
using SFA.DAS.EAS.Domain.Interfaces;
using SFA.DAS.EAS.Domain.Models.Audit;

namespace SFA.DAS.EAS.Application.UnitTests.Commands.AuditCommandTests
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
            await _createAuditCommandHandler.Handle(new CreateAuditCommand());

            //Assert
            _validator.Verify(x=>x.Validate(It.IsAny<CreateAuditCommand>()),Times.Once);
        }

        [Test]
        public void ThenIfTheCommandIsNotValidAnInvalidRequestExceptionIsThrown()
        {
            //Arrange
            _validator.Setup(x => x.Validate(It.IsAny<CreateAuditCommand>())).Returns(new ValidationResult {ValidationDictionary = new Dictionary<string, string> {{"", ""}}});

            //Act Assert
            Assert.ThrowsAsync<InvalidRequestException>(async () => await _createAuditCommandHandler.Handle(new CreateAuditCommand()));
        }

        [Test]
        public async Task ThenTheCommandIfValidIsPassedToTheService()
        {
            //Arrange
            var auditCommand = new CreateAuditCommand {EasAuditMessage = new EasAuditMessage {ChangedProperties = new List<PropertyUpdate> {new PropertyUpdate()},Description = "test", RelatedEntities = new List<Entity> {new Entity()} } };

            //Act
            await _createAuditCommandHandler.Handle(auditCommand);

            //Assert
            _auditService.Verify(x=>x.SendAuditMessage(It.Is<EasAuditMessage>(c=>
                        c.Description.Equals(auditCommand.EasAuditMessage.Description) &&
                        c.ChangedProperties.Count == 1 &&
                        c.RelatedEntities.Count == 1 
                        )));
        }

    }
}
