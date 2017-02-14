using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NUnit.Framework;
using SFA.DAS.EAS.Application.Queries.AccountTransactions.GetLastLevyDeclaration;

namespace SFA.DAS.EAS.Application.UnitTests.Queries.GetLastLevyDeclarationsTests
{
    class WhenIValidateTheRequest
    {
        private GetLastLevyDeclarationValidator _validator;

        [SetUp]
        public void Arrange()
        {
            _validator = new GetLastLevyDeclarationValidator();
        }

        [Test]
        public void ThenTheRequestIsValidWhenAllFieldsArePopulated()
        {
            //Arrange
            var actual = _validator.Validate(new GetLastLevyDeclarationQuery {EmpRef = "asdasd"});

            //Act
            Assert.IsTrue(actual.IsValid());
        }

        [Test]
        public void ThenTheRequestIsNotValidWhenTheFieldsArentPopulatedAndTheErrorDictionaryIsPopulated()
        {
            //Arrange
            var actual = _validator.Validate(new GetLastLevyDeclarationQuery());

            //Act
            Assert.IsFalse(actual.IsValid());
            Assert.Contains(new KeyValuePair<string,string>("EmpRef", "EmpRef has not been supplied"), actual.ValidationDictionary );
        }
    }
}
