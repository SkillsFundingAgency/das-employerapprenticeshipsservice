using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Moq;
using NUnit.Framework;
using SFA.DAS.EAS.Application.Queries.AccountTransactions.GetEnglishFrationDetail;
using SFA.DAS.EAS.Application.Validation;
using SFA.DAS.EAS.Domain.Data;
using SFA.DAS.EAS.Domain.Data.Repositories;
using SFA.DAS.EAS.Domain.Models.Levy;

namespace SFA.DAS.EAS.Application.UnitTests.Queries.GetEnglishFrationDetail
{
    public class WhenIGetEnglishFractionDetail : QueryBaseTest<GetEnglishFractionDetailByEmpRefQueryHandler,GetEnglishFractionDetailByEmpRefQuery,GetEnglishFractionDetailResposne>
    {
        private Mock<IDasLevyRepository> _dasLevyRepository;

        public const string ExpectedEmpRef = "123/ABC";
        public override GetEnglishFractionDetailByEmpRefQuery Query { get; set; }
        public override GetEnglishFractionDetailByEmpRefQueryHandler RequestHandler { get; set; }
        public override Mock<IValidator<GetEnglishFractionDetailByEmpRefQuery>> RequestValidator { get; set; }

        [SetUp]
        public void Arrange()
        {
            SetUp();

            _dasLevyRepository = new Mock<IDasLevyRepository>();
            _dasLevyRepository.Setup(x => x.GetEnglishFractionHistory(ExpectedEmpRef))
                .ReturnsAsync(new List<DasEnglishFraction>
                {
                    new DasEnglishFraction
                    {
                        Amount = 1m,
                        DateCalculated = new DateTime(2016, 01, 01),
                        EmpRef = ExpectedEmpRef,
                        Id = "1"
                    }
                });

            Query = new GetEnglishFractionDetailByEmpRefQuery {EmpRef = ExpectedEmpRef};
            
            RequestHandler = new GetEnglishFractionDetailByEmpRefQueryHandler(RequestValidator.Object, _dasLevyRepository.Object);
        }

        [Test]
        public override async Task ThenIfTheMessageIsValidTheRepositoryIsCalled()
        {
            //Act
            await RequestHandler.Handle(Query);

            //Assert
            _dasLevyRepository.Verify(x=>x.GetEnglishFractionHistory(ExpectedEmpRef),Times.Once);
        }

        [Test]
        public override async Task ThenIfTheMessageIsValidTheValueIsReturnedInTheResponse()
        {
            //Act
            var actual = await RequestHandler.Handle(Query);

            //Assert
            Assert.AreEqual(1,actual.FractionDetail.Count());
        }
        
    }
}
