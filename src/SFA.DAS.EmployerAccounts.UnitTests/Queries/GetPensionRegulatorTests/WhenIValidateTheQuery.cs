using System.Collections.Generic;
using NUnit.Framework;
using SFA.DAS.EmployerAccounts.Queries.GetPensionRegulator;

namespace SFA.DAS.EmployerAccounts.UnitTests.Queries.GetPensionRegulatorTests
{
    public class WhenIValidateTheQuery
    {
        private GetPensionRegulatorValidator _validator;

        [SetUp]
        public void Arrange()
        {
            _validator = new GetPensionRegulatorValidator();
        }

        [Test]
        public void ThenTrueIsReturnedWhenAllFieldsArePopulated()
        {
            //Act
            var actual = _validator.Validate(new GetPensionRegulatorRequest {PayeRef = "PayeRefTest"});

            //Assert
            Assert.IsNotNull(actual);
            Assert.IsTrue(actual.IsValid());
        }

        [Test]
        public void ThenFalseIsReturnedAndTheErrorDictionaryPopulatedWhenTheFieldsArentPopulated()
        {
            //Act
            var actual = _validator.Validate(new GetPensionRegulatorRequest { PayeRef = "" });

            //Assert
            Assert.IsFalse(actual.IsValid());
            Assert.Contains(new KeyValuePair<string,string>("PayeRef","PayeRef has not been supplied"), actual.ValidationDictionary);
        }
    }
}
