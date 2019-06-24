using System.Collections.Generic;
using NUnit.Framework;
using SFA.DAS.EmployerAccounts.Queries.GetOrganisationsByAorn;

namespace SFA.DAS.EmployerAccounts.UnitTests.Queries.GetOrganisationsByAornTests
{
    public class WhenIValidateTheQuery
    {
        private GetOrganisationsByAornValidator _validator;

        [SetUp]
        public void Arrange()
        {
            _validator = new GetOrganisationsByAornValidator();
        }

        [Test]
        public void ThenTrueIsReturnedWhenAllFieldsArePopulated()
        {
            //Act
            var actual = _validator.Validate(new GetOrganisationsByAornRequest("aorn", "PayeRefTest"));

            //Assert
            Assert.IsNotNull(actual);
            Assert.IsTrue(actual.IsValid());
        }

        [Test]
        public void ThenFalseIsReturnedAndTheErrorDictionaryPopulatedWhenAornIsntPopulated()
        {
            //Act
            var actual = _validator.Validate(new GetOrganisationsByAornRequest("", "paye"));

            //Assert
            Assert.IsFalse(actual.IsValid());
            Assert.Contains(new KeyValuePair<string,string>("Aorn","Aorn has not been supplied"), actual.ValidationDictionary);
        }

        [Test]
        public void ThenFalseIsReturnedAndTheErrorDictionaryPopulatedWhenPayeRefIsntPopulated()
        {
            //Act
            var actual = _validator.Validate(new GetOrganisationsByAornRequest("aorn", ""));

            //Assert
            Assert.IsFalse(actual.IsValid());
            Assert.Contains(new KeyValuePair<string, string>("PayeRef", "PayeRef has not been supplied"), actual.ValidationDictionary);
        }
    }
}
