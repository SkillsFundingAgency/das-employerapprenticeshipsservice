using NUnit.Framework;
using SFA.DAS.EAS.Application.Commands.RefreshAccountTransfers;
using System.Collections.Generic;

namespace SFA.DAS.EAS.Application.UnitTests.Commands.RefreshAccountTransfersTests
{
    public class WhenIValidateTheCommand
    {
        private RefreshAccountTransfersCommandValidator _validator;

        [SetUp]
        public void Arrange()
        {
            _validator = new RefreshAccountTransfersCommandValidator();
        }

        [Test]
        public void ThenTrueIsReturnedWhenAllFieldsAreValid()
        {
            //Act
            var actual = _validator.Validate(new RefreshAccountTransfersCommand
            {
                AccountId = 123123,
                PeriodEnd = "123"
            });

            //Assert
            Assert.IsTrue(actual.IsValid());
        }

        [Test]
        public void ThenFalseIsReturnedWhenAllFieldsArentPopulatedAndTheErrorDictionaryIsPopulated()
        {
            //Act
            var actual = _validator.Validate(new RefreshAccountTransfersCommand());

            //Assert
            Assert.IsFalse(actual.IsValid());
            Assert.Contains(new KeyValuePair<string, string>("AccountId", "AccountId has not been supplied"), actual.ValidationDictionary);
            Assert.Contains(new KeyValuePair<string, string>("PeriodEnd", "PeriodEnd has not been supplied"), actual.ValidationDictionary);
        }

        [Test]
        public void ThenFalseIsReturnedWhenPeriodEndIsEmpty()
        {
            //Act
            var actual = _validator.Validate(new RefreshAccountTransfersCommand
            {
                AccountId = 123123,
                PeriodEnd = string.Empty
            });

            //Assert
            Assert.IsFalse(actual.IsValid());
            Assert.Contains(new KeyValuePair<string, string>("PeriodEnd", "PeriodEnd has not been supplied"), actual.ValidationDictionary);
        }

        [Test]
        public void ThenFalseIsReturnedWhenAccountIdIsNegative()
        {
            //Act
            var actual = _validator.Validate(new RefreshAccountTransfersCommand
            {
                AccountId = -123123,
                PeriodEnd = "123"
            });

            //Assert
            Assert.IsFalse(actual.IsValid());
            Assert.Contains(new KeyValuePair<string, string>("AccountId", "AccountId cannot be negative"), actual.ValidationDictionary);
        }
    }
}
