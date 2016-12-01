using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Moq;
using NUnit.Framework;
using SFA.DAS.EAS.Application.Queries.AccountTransactions.GetEnglishFrationDetail;
using SFA.DAS.EAS.Domain;
using SFA.DAS.EAS.Domain.Data;

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
