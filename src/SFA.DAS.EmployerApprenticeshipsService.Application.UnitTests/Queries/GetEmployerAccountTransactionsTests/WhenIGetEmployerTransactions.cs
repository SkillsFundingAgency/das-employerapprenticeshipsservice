using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Moq;
using NUnit.Framework;
using SFA.DAS.EAS.Application.Queries.GetEmployerAccountTransactions;
using SFA.DAS.EAS.Application.Validation;
using SFA.DAS.EAS.Domain;
using SFA.DAS.EAS.Domain.Interfaces;
using SFA.DAS.EAS.Domain.Models.Levy;

namespace SFA.DAS.EAS.Application.UnitTests.Queries.GetEmployerAccountTransactionsTests
{
    public class WhenIGetEmployerTransactions : QueryBaseTest<GetEmployerAccountTransactionsHandler, GetEmployerAccountTransactionsQuery, GetEmployerAccountTransactionsResponse>
    {
        private Mock<IDasLevyService> _dasLevyService;
        private GetEmployerAccountTransactionsQuery _request;

        public override GetEmployerAccountTransactionsQuery Query { get; set; }
        public override GetEmployerAccountTransactionsHandler RequestHandler { get; set; }
        public override Mock<IValidator<GetEmployerAccountTransactionsQuery>> RequestValidator { get; set; }

        [SetUp]
        public void Arrange()
        {
            SetUp();
          
            _request = new GetEmployerAccountTransactionsQuery
            {
                AccountId = 1,
                ExternalUserId = "3EFR",
                HashedId = "RTF34"
            };

            _dasLevyService = new Mock<IDasLevyService>();
            _dasLevyService.Setup(x => x.GetTransactionsByAccountId(It.IsAny<long>()))
                           .ReturnsAsync(new List<TransactionLine>());
           
            RequestHandler = new GetEmployerAccountTransactionsHandler(_dasLevyService.Object, RequestValidator.Object);
            Query = new GetEmployerAccountTransactionsQuery();
        }

        [Test]
        public override async Task ThenIfTheMessageIsValidTheRepositoryIsCalled()
        {
            //Act
            await RequestHandler.Handle(_request);

            //Assert
            _dasLevyService.Verify(x => x.GetTransactionsByAccountId(_request.AccountId));
        }

        [Test]
        public override async Task ThenIfTheMessageIsValidTheValueIsReturnedInTheResponse()
        {
            //Arrange
            var transactions = new List<TransactionLine>
                {
                    new TransactionLine
                    {
                        AccountId = 1,
                        SubmissionId = 1,
                        TransactionDate = DateTime.Now.AddMonths(-3),
                        Amount = 1000,
                        TransactionType = LevyItemType.TopUp,
                        EmpRef = "123"
                    }
                };

            _dasLevyService.Setup(x => x.GetTransactionsByAccountId(It.IsAny<long>()))
                           .ReturnsAsync(transactions);

            //Act
            var response = await RequestHandler.Handle(_request);

            //Assert
            Assert.AreEqual(_request.HashedId, response.Data.HashedId);
            Assert.AreEqual(_request.AccountId, response.Data.AccountId);
            Assert.AreEqual(1, response.Data.TransactionLines.Count);
        }

        [Test]
        public async Task ThenMessagesWithSameSubmissionIdShouldBeAggregatedIntoSingleTransaction()
        {
            //Arrange
            var transactions = new List<TransactionLine>
                {
                    new TransactionLine
                    {
                        AccountId = 1,
                        SubmissionId = 1,
                        TransactionDate = DateTime.Now.AddMonths(-3),
                        Amount = 1000,
                        TransactionType = LevyItemType.TopUp,
                        EmpRef = "123"
                    },
                     new TransactionLine
                    {
                        AccountId = 1,
                        SubmissionId = 1,
                        TransactionDate = DateTime.Now.AddMonths(-3),
                        Amount = 500,
                        TransactionType = LevyItemType.TopUp,
                        EmpRef = "123"
                    }
                };

            _dasLevyService.Setup(x => x.GetTransactionsByAccountId(It.IsAny<long>()))
                           .ReturnsAsync(transactions);

            //Act
            var response = await RequestHandler.Handle(_request);

            //Assert
            Assert.AreEqual(_request.HashedId, response.Data.HashedId);
            Assert.AreEqual(_request.AccountId, response.Data.AccountId);
            Assert.AreEqual(1, response.Data.TransactionLines.Count);
            Assert.AreEqual(1500, response.Data.TransactionLines.ElementAt(0).Amount);
        }

        [Test]
        public async Task ThenMessagesInTheSameMonthShouldBeAggregatedTogether()
        {
            //Arrange
            var transactions = new List<TransactionLine>
                {
                    new TransactionLine
                    {
                        AccountId = 1,
                        SubmissionId = 1,
                        TransactionDate = DateTime.Now.AddMonths(-3),
                        Amount = 1000,
                        TransactionType = LevyItemType.TopUp,
                        EmpRef = "123"
                    },
                     new TransactionLine
                    {
                        AccountId = 1,
                        SubmissionId = 2,
                        TransactionDate = DateTime.Now.AddMonths(-3),
                        Amount = 500,
                        TransactionType = LevyItemType.TopUp,
                        EmpRef = "123"
                    },
                      new TransactionLine
                    {
                        AccountId = 1,
                        SubmissionId = 3,
                        TransactionDate = DateTime.Now.AddMonths(-2),
                        Amount = 500,
                        TransactionType = LevyItemType.TopUp,
                        EmpRef = "123"
                    }
                };

            _dasLevyService.Setup(x => x.GetTransactionsByAccountId(It.IsAny<long>()))
                           .ReturnsAsync(transactions);

            //Act
            var response = await RequestHandler.Handle(_request);

            //Assert
            Assert.AreEqual(_request.HashedId, response.Data.HashedId);
            Assert.AreEqual(_request.AccountId, response.Data.AccountId);
            Assert.AreEqual(2, response.Data.TransactionLines.Count);
            Assert.AreEqual(500, response.Data.TransactionLines.ElementAt(0).Amount);
            Assert.AreEqual(1500, response.Data.TransactionLines.ElementAt(1).Amount);
        }

        [Test]
        public async Task ThenIfNoTransactionAreFoundAnEmptyTransactionListIsReturned()
        {
            //Act
            var response = await RequestHandler.Handle(_request);

            //Assert
            Assert.AreEqual(_request.HashedId, response.Data.HashedId);
            Assert.AreEqual(_request.AccountId, response.Data.AccountId);
            Assert.IsEmpty(response.Data.TransactionLines);
        }

        [Test]
        public async Task ThenTheBalanceShouldBeInDescendingDateOrder()
        {
            //Arrange
            var transactions = new List<TransactionLine>
                {
                    new TransactionLine
                    {
                        AccountId = 1,
                        SubmissionId = 1,
                        TransactionDate = DateTime.Now.AddMonths(-3),
                        Amount = 1000,
                        TransactionType = LevyItemType.TopUp,
                        EmpRef = "123"
                    },
                     new TransactionLine
                    {
                        AccountId = 1,
                        SubmissionId = 2,
                        TransactionDate = DateTime.Now.AddMonths(-3),
                        Amount = 500,
                        TransactionType = LevyItemType.TopUp,
                        EmpRef = "123"
                    },
                      new TransactionLine
                    {
                        AccountId = 1,
                        SubmissionId = 3,
                        TransactionDate = DateTime.Now.AddMonths(-2),
                        Amount = 600,
                        TransactionType = LevyItemType.TopUp,
                        EmpRef = "123"
                    },
                      new TransactionLine
                    {
                        AccountId = 1,
                        SubmissionId = 3,
                        TransactionDate = DateTime.Now.AddMonths(-1),
                        Amount = 250,
                        TransactionType = LevyItemType.TopUp,
                        EmpRef = "123"
                    }
                };

            _dasLevyService.Setup(x => x.GetTransactionsByAccountId(It.IsAny<long>()))
                           .ReturnsAsync(transactions);

            //Act
            var response = await RequestHandler.Handle(_request);

            //Assert
            Assert.AreEqual(_request.HashedId, response.Data.HashedId);
            Assert.AreEqual(_request.AccountId, response.Data.AccountId);
            Assert.AreEqual(2, response.Data.TransactionLines.Count);
            Assert.AreEqual(1500, response.Data.TransactionLines.ElementAt(1).Balance);
            Assert.AreEqual(2350, response.Data.TransactionLines.ElementAt(0).Balance);
        }
    }
}
