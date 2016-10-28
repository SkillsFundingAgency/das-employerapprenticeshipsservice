using System.Collections.Generic;
using NUnit.Framework;
using SFA.DAS.EAS.Application.Queries.GetPayeSchemeInUse;

namespace SFA.DAS.EAS.Application.UnitTests.Queries.GetPayeSchemeInUseTests
{
    public class WhenIValidateTheQuery
    {
        private GetPayeSchemeInUseValidator _validator;

        [SetUp]
        public void Arrange()
        {
            _validator = new GetPayeSchemeInUseValidator();
        }

        [Test]
        public void ThenFalseIsReturnedWhenTheQueryIsEmpty()
        {
            //Act
            var actual = _validator.Validate(new GetPayeSchemeInUseQuery());

            //Assety
            Assert.IsFalse(actual.IsValid());
            Assert.Contains(new KeyValuePair<string,string>("Empref", "Empref has not been supplied"),actual.ValidationDictionary );
        }

        [Test]
        public void ThenTrueIsReturnedWhenTheQueryIsPopulated()
        {
            //Act
            var actual = _validator.Validate(new GetPayeSchemeInUseQuery {Empref = "AFV123"});

            //Assety
            Assert.IsTrue(actual.IsValid());
        }
    }
}