using System.Collections.Generic;
using NUnit.Framework;
using SFA.DAS.EmployerApprenticeshipsService.Application.Commands.SendNotification;
using SFA.DAS.Notifications.Api.Types;

namespace SFA.DAS.EmployerApprenticeshipsService.Application.UnitTests.Commands.SendNotificationTests
{
    public class WhenValidatingTheCommand
    {
        private SendNotificationCommandValidator _validator;

        [SetUp]
        public void Arrange()
        {
            _validator = new SendNotificationCommandValidator();
        }

        [Test]
        public void ThenTheCommandIsNotValidIfNoFieldsArePopulated()
        {
            //Act
            var actual = _validator.Validate(new SendNotificationCommand());

            //Assert
            Assert.IsFalse(actual.IsValid());
        }
        

        [Test]
        public void ThenTheErrorDictionaryIsPopulatedWhenNotValid()
        {
            //Act
            var actual = _validator.Validate(new SendNotificationCommand {Email = new Email()});

            //Assert
            Assert.Contains(new KeyValuePair<string,string> ("RecipientsAddress", "RecipientsAddress has not been supplied"), actual.ValidationDictionary);
            Assert.Contains(new KeyValuePair<string,string> ("TemplateId", "TemplateId has not been supplied"), actual.ValidationDictionary);
        }
        

        [Test]
        public void ThenIsValidWhenAllFieldsHaveBeenSupplied()
        {
            //Assert
            var actual = _validator.Validate(new SendNotificationCommand { Email = new Email { RecipientsAddress = "test",ReplyToAddress = "test",Subject = "test",TemplateId = "test",SystemId = "test"}});

            //Assert
            Assert.IsTrue(actual.IsValid());
        }
    }
}
