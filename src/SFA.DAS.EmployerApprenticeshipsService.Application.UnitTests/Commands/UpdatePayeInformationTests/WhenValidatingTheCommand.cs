using System.Collections.Generic;
using NUnit.Framework;
using SFA.DAS.EAS.Application.Commands.UpdatePayeInformation;

namespace SFA.DAS.EAS.Application.UnitTests.Commands.UpdatePayeInformationTests
{
    public class WhenValidatingTheCommand
    {
        private UpdatePayeInformationValidator _validator;

        [SetUp]
        public void Arrange()
        {
            _validator = new UpdatePayeInformationValidator();
        }

        [Test]
        public void ThenTrueIsReturnedWhenAllFieldsArePopulated()
        {
            //act
            var actual = _validator.Validate(new UpdatePayeInformationCommand {PayeRef = "1234rf"});

            //Assert
            Assert.IsNotNull(actual);
            Assert.IsTrue(actual.IsValid());
        }

        [Test]
        public void ThenFalseIsReturnedWhenTheFieldsArentPopulatedAndTheErrorDictionaryIsPopulated()
        {
            //Act
            var actual = _validator.Validate(new UpdatePayeInformationCommand());

            //Assert
            Assert.IsFalse(actual.IsValid());
            Assert.Contains(new KeyValuePair<string,string>("PayeRef","PayeRef has not been supplied"),actual.ValidationDictionary );
        }
    }
}