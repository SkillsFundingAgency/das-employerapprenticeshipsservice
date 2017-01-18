using System.Collections.Generic;
using NUnit.Framework;
using SFA.DAS.Audit.Types;
using SFA.DAS.EAS.Application.Commands.AuditCommand;
using SFA.DAS.EAS.Domain.Models.Audit;

namespace SFA.DAS.EAS.Application.UnitTests.Commands.AuditCommandTests
{
    public class WhenIValidateTheCommand
    {
        private AuditCommandValidator _validator;

        [SetUp]
        public void Arrange()
        {
            _validator = new AuditCommandValidator();
        }

        [Test]
        public void ThenTrueIsReturnedWhenAllFieldsArePopulated()
        {
            //Act
            var actual = _validator.Validate(new AuditCommand {EasAuditMessage = new EasAuditMessage
            {
                Description = "descriptiosn",
                RelatedEntities = new List<Entity> { new Entity () },
                ChangedProperties = new List<PropertyUpdate> { new PropertyUpdate()}
            } });

            //Assert
            Assert.IsTrue(actual.IsValid());
        }

        [Test]
        public void ThenFalseIsReturnedAndTheDictionaryIsPopulatedWhenTheCommandIsNotValid()
        {
            //Act
            var actual = _validator.Validate(new AuditCommand { EasAuditMessage = new EasAuditMessage() });

            //Assert
            Assert.IsFalse(actual.IsValid());
            Assert.Contains(new KeyValuePair<string,string>("ChangedProperties", "ChangedProperties has not been supplied"), actual.ValidationDictionary);
            Assert.Contains(new KeyValuePair<string,string>("Description", "Description has not been supplied"), actual.ValidationDictionary);
            Assert.Contains(new KeyValuePair<string,string>("RelatedEntities", "RelatedEntities has not been supplied"), actual.ValidationDictionary);
        }

        [Test]
        public void ThenFalseIsReturnedAndTheDictionaryIsPopulatedWhenTheListsAreEmpty()
        {
            //Act
            var actual = _validator.Validate(new AuditCommand { EasAuditMessage = new EasAuditMessage {Description = "test", RelatedEntities = new List<Entity>(),ChangedProperties = new List<PropertyUpdate>()} });

            //Assert
            Assert.IsFalse(actual.IsValid());
            Assert.Contains(new KeyValuePair<string, string>("ChangedProperties", "ChangedProperties has not been supplied"), actual.ValidationDictionary);
            Assert.Contains(new KeyValuePair<string, string>("RelatedEntities", "RelatedEntities has not been supplied"), actual.ValidationDictionary);
        }

        [Test]
        public void ThenFalseIsReturnedAndTheDictionaryIsPopulatedWhenTheCommandEasAuditMessageIsNull()
        {
            //Act
            var actual = _validator.Validate(new AuditCommand ());

            //Assert
            Assert.IsFalse(actual.IsValid());
            Assert.Contains(new KeyValuePair<string, string>("EasAuditMessage", "EasAuditMessage has not been supplied"), actual.ValidationDictionary);
            
        }
    }
}
