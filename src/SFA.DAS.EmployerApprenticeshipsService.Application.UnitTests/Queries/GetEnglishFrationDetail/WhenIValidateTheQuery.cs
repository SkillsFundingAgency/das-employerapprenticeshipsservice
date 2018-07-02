using System.Collections.Generic;
using NUnit.Framework;
using SFA.DAS.EAS.Application.Queries.AccountTransactions.GetEnglishFrationDetail;

namespace SFA.DAS.EAS.Application.UnitTests.Queries.GetEnglishFrationDetail
{
    public class WhenIValidateTheQuery
    {
        private GetEnglishFractionDetailValidator _validator;

        [SetUp]
        public void Arrange()
        {
            
            _validator = new GetEnglishFractionDetailValidator();
        }

        [Test]
        public void ThenWhenAllFieldsAreSuppliedTheMessageIsValid()
        {
            //Act
            var actual = _validator.Validate(new GetEnglishFractionDetailByEmpRefQuery
            {
                EmpRef="123ABC"
            });

            //Assert
            Assert.IsTrue(actual.IsValid());
        }

        [Test]
        public void ThenFalseIsReturnedWhenTheFieldsArePopulated()
        {
            //Act
            var actual = _validator.Validate(new GetEnglishFractionDetailByEmpRefQuery ());

            //Assert
            Assert.IsFalse(actual.IsValid());
            Assert.Contains(new KeyValuePair<string,string>("EmpRef","EmpRef has not been supplied"),actual.ValidationDictionary );
        }

        
    }
}
