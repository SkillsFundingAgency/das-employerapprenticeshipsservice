using System.Collections.Generic;
using NUnit.Framework;
using SFA.DAS.Common.Domain.Types;
using SFA.DAS.EmployerAccounts.Queries.GetOrganisationById;

namespace SFA.DAS.EmployerAccounts.UnitTests.Queries.GetOrganisationByIdTests
{
    public class WhenIValidateTheQuery
    {
        private GetOrganisationByIdValidator _validator;

        [SetUp]
        public void Arrange()
        {
            _validator = new GetOrganisationByIdValidator();
        }

        [Test]
        public void ThenTrueIsReturnedWhenAllFieldsArePopulated()
        {
            //Act
            var actual = _validator.Validate(new GetOrganisationByIdRequest { Identifier = "Test", OrganisationType = OrganisationType.Other});

            //Assert
            Assert.IsNotNull(actual);
            Assert.IsTrue(actual.IsValid());
        }

        [Test]
        public void ThenFalseIsReturnedAndTheErrorDictionaryPopulatedWhenTheFieldsArentPopulated()
        {
            //Act
            var actual = _validator.Validate(new GetOrganisationByIdRequest { Identifier = "" });

            //Assert
            Assert.IsFalse(actual.IsValid());
            Assert.Contains(new KeyValuePair<string,string>("Identifier", "Identifier has not been supplied"), actual.ValidationDictionary);
        }
    }
}
