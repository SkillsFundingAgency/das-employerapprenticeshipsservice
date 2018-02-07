using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Moq;
using NUnit.Framework;
using SFA.DAS.Audit.Client;
using SFA.DAS.Audit.Types;
using SFA.DAS.EAS.Domain.Models.Audit;
using SFA.DAS.EAS.Infrastructure.Services;
using SFA.DAS.NLog.Logger;
using Entity = SFA.DAS.Audit.Types.Entity;

namespace SFA.DAS.EAS.Infrastructure.UnitTests.Services.AuditServiceTests
{
    public class WhenISendAnAuditMessage
    {
        private AuditService _auditService;
        private Mock<IAuditApiClient> _auditApiClient;
        private Mock<IAuditMessageFactory> _auditMessageFactory;
        private Mock<ILog> _logger;

        [SetUp]
        public void Arrange()
        {
            _auditApiClient = new Mock<IAuditApiClient>();
            _auditMessageFactory = new Mock<IAuditMessageFactory>();
            _logger = new Mock<ILog>();

            _auditService = new AuditService(_auditApiClient.Object, _auditMessageFactory.Object, _logger.Object);
        }

        [Test]
        public async Task ThenTheServiceIsCalledWithTheBuildAuditMessage()
        {
            //Arrange
            var easAuditMessage = new EasAuditMessage
            {
                Description = "Some stuff",
                RelatedEntities = new List<Entity> { new Entity { Id = "12345", Type = "test" } },
                ChangedProperties = new List<PropertyUpdate> { new PropertyUpdate { NewValue = "New", PropertyName = "Name" } }
            };
            _auditMessageFactory.Setup(x => x.Build()).Returns(new AuditMessage());

            //Act
            await _auditService.SendAuditMessage(easAuditMessage);

            //Assert
            _auditApiClient.Verify(x => x.Audit(It.Is<AuditMessage>(c => 
                                c.Description.Equals("Some stuff") &&
                                c.ChangedProperties.SingleOrDefault(y=>y.NewValue.Equals("New") && y.PropertyName.Equals("Name")) != null &&
                                c.RelatedEntities.SingleOrDefault(y=>y.Id.Equals("12345") && y.Type.Equals("test")) != null 
                                )), Times.Once);
        }

        [Test]
        public async Task ThenAnErrorIsLoggedWhenTheAuditServiceThrowsAnException()
        {
            //Arrange
            var easAuditMessage = new EasAuditMessage
            {
                Description = "Some stuff",
                RelatedEntities = new List<Entity> { new Entity { Id = "12345", Type = "test" } },
                ChangedProperties = new List<PropertyUpdate> { new PropertyUpdate { NewValue = "New", PropertyName = "Name" } }
            };
            _auditMessageFactory.Setup(x => x.Build()).Returns(new AuditMessage());
            _auditApiClient.Setup(x => x.Audit(It.IsAny<AuditMessage>())).Throws(new Exception());

            //Act
            await _auditService.SendAuditMessage(easAuditMessage);

            //Assert
            _logger.Verify(x=>x.Error(It.IsAny<Exception>(),It.IsAny<string>()));
        }
    }
}
