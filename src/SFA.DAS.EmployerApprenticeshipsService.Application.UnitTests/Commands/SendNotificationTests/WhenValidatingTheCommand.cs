using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NUnit.Framework;
using SFA.DAS.EmployerApprenticeshipsService.Application.Commands.SendNotification;
using SFA.DAS.EmployerApprenticeshipsService.Domain.Models.Notification;

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
            var actual = _validator.Validate(new SendNotificationCommand());

            //Assert
            Assert.Contains(new KeyValuePair<string,string> ("UserId", "User Id has not been supplied"), actual.ValidationDictionary);
            Assert.Contains(new KeyValuePair<string,string> ("Data", "EmailContent has not been supplied"), actual.ValidationDictionary);
        }

        [Test]
        public void ThenTheErrorDictionaryIsPopulatedWhenNotValidAndHasEmailContentObject()
        {
            //Act
            var actual = _validator.Validate(new SendNotificationCommand {Data = new EmailContent()});

            //Assert
            Assert.Contains(new KeyValuePair<string, string>("UserId", "User Id has not been supplied"), actual.ValidationDictionary);
            Assert.Contains(new KeyValuePair<string, string>("RecipientsAddress", "Recipients Address has not been supplied"), actual.ValidationDictionary);
        }

        [Test]
        public void ThenIsValidWhenAllFieldsHaveBeenSupplied()
        {
            //Assert
            var actual = _validator.Validate(new SendNotificationCommand { Data = new EmailContent {RecipientsAddress = "test"}, UserId = 1});

            //Assert
            Assert.IsTrue(actual.IsValid());
        }
    }
}
