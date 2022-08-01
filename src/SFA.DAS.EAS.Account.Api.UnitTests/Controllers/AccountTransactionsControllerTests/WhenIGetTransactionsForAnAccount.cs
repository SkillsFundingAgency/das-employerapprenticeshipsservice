using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Web.Http.Results;
using System.Web.Http.Routing;
using FluentAssertions;
using MediatR;
using Moq;
using NUnit.Framework;
using SFA.DAS.EAS.Account.Api.Types;
using SFA.DAS.EAS.Account.Api.Controllers;
using SFA.DAS.EAS.Account.Api.Orchestrators;
using SFA.DAS.EAS.Application.Queries.GetEmployerAccountTransactions;
using SFA.DAS.EAS.Domain.Models.Transaction;
using SFA.DAS.EAS.TestCommon.Extensions;
using SFA.DAS.EAS.TestCommon.ObjectMothers;
using SFA.DAS.NLog.Logger;
using SFA.DAS.EmployerFinance.Services;
using AutoMapper;
using AutoFixture;
using System.Reflection;
using System.Linq;
using Castle.Core.Internal;

namespace SFA.DAS.EAS.Account.Api.UnitTests.Controllers.AccountTransactionsControllerTests
{
    [TestFixture]
    public class WhenIGetTransactionsForAnAccount
    {
        private AccountTransactionsController _controller;
        private Mock<IMediator> _mediator;
        private Mock<ILog> _logger;
        private Mock<UrlHelper> _urlHelper;
        private Mock<IMapper> _mapper;
        private Mock<IEmployerFinanceApiService> _financeApiService;
        protected IMapper Mapper;

        [SetUp]
        public void Arrange()
        {
            _mediator = new Mock<IMediator>();
            _logger = new Mock<ILog>();
            _urlHelper = new Mock<UrlHelper>();
            _urlHelper.Setup(x => x.Route(It.IsAny<string>(), It.IsAny<object>())).Returns("dummyurl");
            _mapper = new Mock<IMapper>();
            _financeApiService = new Mock<IEmployerFinanceApiService>();
            Mapper = ConfigureMapper();
            var orchestrator = new AccountTransactionsOrchestrator(_mediator.Object, Mapper, _logger.Object, _financeApiService.Object);
            _controller = new AccountTransactionsController(orchestrator);
            _controller.Url = _urlHelper.Object;
            
        }

        [Test]
        public async Task ThenTheTransactionsAreReturned()
        {            
            //Arrange
            var hashedAccountId = "ABC123";
            var year = 2017;
            var month = 3;
            var transactionsResponse = new GetEmployerAccountTransactionsResponse
            {
                Data = new AggregationData { TransactionLines = new List<TransactionLine> { TransactionLineObjectMother.Create() } },
                AccountHasPreviousTransactions = false
            };
            _mediator.Setup(x => x.SendAsync(It.Is<GetEmployerAccountTransactionsQuery>(q => q.HashedAccountId == hashedAccountId && q.Year == year && q.Month == month))).ReturnsAsync(transactionsResponse);


            //var fixture = new Fixture();
            //var apiResponse = new SFA.DAS.EAS.Finance.Api.Types.TransactionsViewModel()
            //{
            //    Year = fixture.Create<int>(),
            //    Month = fixture.Create<int>(),
            //    HasPreviousTransactions = fixture.Create<bool>(),
            //    PreviousMonthUri = fixture.Create<string>()
            //};

            //ICollection<SFA.DAS.EAS.Finance.Api.Types.TransactionsViewModel> apiResponse = new List<SFA.DAS.EAS.Finance.Api.Types.TransactionsViewModel>()
            //{
            //     fixture.Create<SFA.DAS.EAS.Finance.Api.Types.TransactionsViewModel>(),
            //    fixture.Create<SFA.DAS.EAS.Finance.Api.Types.TransactionsViewModel>()
            //};
            var isNotZero = 100m;
            var isTxDateCreated = DateTime.Today;
            var transactionsViewModel = new SFA.DAS.EAS.Finance.Api.Types.TransactionsViewModel
            {
                new SFA.DAS.EAS.Finance.Api.Types.TransactionViewModel
                {
                    Description = "Is Not Null",
                    Amount = isNotZero,
                    DateCreated = isTxDateCreated
                },
                new SFA.DAS.EAS.Finance.Api.Types.TransactionViewModel
                {
                    Description = "Is Not Null 2",
                    Amount = isNotZero,
                    DateCreated = isTxDateCreated
                }
            };
            _financeApiService.Setup(x => x.GetTransactions(hashedAccountId, year, month)).ReturnsAsync(transactionsViewModel);

            //Act
            var response = await _controller.GetTransactions(hashedAccountId, year, month);
                        
            //Assert
            Assert.IsNotNull(response);
            Assert.IsInstanceOf<OkNegotiatedContentResult<SFA.DAS.EAS.Account.Api.Types.TransactionsViewModel>>(response);
            var model = response as OkNegotiatedContentResult<SFA.DAS.EAS.Account.Api.Types.TransactionsViewModel>;

            model?.Content.Should().NotBeNull();
            //model?.Content.ShouldAllBeEquivalentTo(transactionsResponse.Data.TransactionLines, options => options.Excluding(x => x.ResourceUri));
        }

        [Test]
        public async Task AndThereAreNoPreviousTransactionThenTheUrlIsNotSet()
        {
            //Arrange
            var hashedAccountId = "ABC123";
            var year = 2017;
            var month = 3;
            var transactionsResponse = new GetEmployerAccountTransactionsResponse
            {
                Data = new AggregationData { TransactionLines = new List<TransactionLine> { TransactionLineObjectMother.Create() } },
                AccountHasPreviousTransactions = false
            };
            _mediator.Setup(x => x.SendAsync(It.Is<GetEmployerAccountTransactionsQuery>(q => q.HashedAccountId == hashedAccountId && q.Year == year && q.Month == month))).ReturnsAsync(transactionsResponse);
            var isNotZero = 100m;
            var isTxDateCreated = DateTime.Today;
            var transactionsViewModel = new SFA.DAS.EAS.Finance.Api.Types.TransactionsViewModel
            {
                new SFA.DAS.EAS.Finance.Api.Types.TransactionViewModel
                {
                    Description = "Is Not Null",
                    Amount = isNotZero,
                    DateCreated = isTxDateCreated
                },
                new SFA.DAS.EAS.Finance.Api.Types.TransactionViewModel
                {
                    Description = "Is Not Null 2",
                    Amount = isNotZero,
                    DateCreated = isTxDateCreated
                }
            };
            _financeApiService.Setup(x => x.GetTransactions(hashedAccountId, year, month)).ReturnsAsync(transactionsViewModel);

            
            //Act
            var response = await _controller.GetTransactions(hashedAccountId, year, month);

            
            //Assert
            Assert.IsNotNull(response);
            Assert.IsInstanceOf<OkNegotiatedContentResult<TransactionsViewModel>>(response);
            var model = response as OkNegotiatedContentResult<TransactionsViewModel>;

            model?.Content.Should().NotBeNull();
            model?.Content.PreviousMonthUri.Should().BeNullOrEmpty();
            _urlHelper.Verify(x => x.Route("GetTransactions", It.IsAny<object>()), Times.Never);
        }

        [Test]
        public async Task AndThereArePreviousTransactionsThenTheLinkIsCorrect()
        {
            //Arrange
            var hashedAccountId = "ABC123";
            var year = 2017;
            var month = 1;
            var transactionsResponse = new GetEmployerAccountTransactionsResponse
            {
                Data = new AggregationData { TransactionLines = new List<TransactionLine> { TransactionLineObjectMother.Create() } },
                AccountHasPreviousTransactions = true,
                Year = year,
                Month = month
            };
            _mediator.Setup(x => x.SendAsync(It.Is<GetEmployerAccountTransactionsQuery>(q => q.HashedAccountId == hashedAccountId && q.Year == year && q.Month == month))).ReturnsAsync(transactionsResponse);


            var isNotZero = 100m;
            var isTxDateCreated = DateTime.Today;
            var transactionsViewModel = new SFA.DAS.EAS.Finance.Api.Types.TransactionsViewModel
            {             
                new SFA.DAS.EAS.Finance.Api.Types.TransactionViewModel
                {
                    Description = "Is Not Null",
                    Amount = isNotZero,
                    DateCreated = isTxDateCreated
                },
                new SFA.DAS.EAS.Finance.Api.Types.TransactionViewModel
                {
                    Description = "Is Not Null 2",
                    Amount = isNotZero,
                    DateCreated = isTxDateCreated
                }                
            };
            transactionsViewModel.HasPreviousTransactions = true;
            transactionsViewModel.Year = year;
            transactionsViewModel.Month = month;

            _financeApiService.Setup(x => x.GetTransactions(hashedAccountId, year, month)).ReturnsAsync(transactionsViewModel);

            var expectedUri = "someuri";
            _urlHelper.Setup(x => x.Route("GetTransactions", It.Is<object>(o => o.IsEquivalentTo(new { hashedAccountId, year = year - 1, month = 12 })))).Returns(expectedUri);

            //Act
            var response = await _controller.GetTransactions(hashedAccountId, year, month);
            var model = response as OkNegotiatedContentResult<TransactionsViewModel>;

            
            //Assert
            model?.Content.PreviousMonthUri.Should().Be(expectedUri);
        }

        [Test]
        public async Task AndNoMonthIsProvidedThenTheCurrentMonthIsUsed()
        {
            var hashedAccountId = "ABC123";
            var year = 2017;
            var transactionsResponse = new GetEmployerAccountTransactionsResponse
            {
                Data = new AggregationData { TransactionLines = new List<TransactionLine> { TransactionLineObjectMother.Create() } },
                AccountHasPreviousTransactions = false
            };
            _mediator.Setup(x => x.SendAsync(It.Is<GetEmployerAccountTransactionsQuery>(q => q.HashedAccountId == hashedAccountId && q.Year == year && q.Month == DateTime.Now.Month))).ReturnsAsync(transactionsResponse);


            var isNotZero = 100m;
            var isTxDateCreated = DateTime.Today;
            var transactionsViewModel = new SFA.DAS.EAS.Finance.Api.Types.TransactionsViewModel
            {
                new SFA.DAS.EAS.Finance.Api.Types.TransactionViewModel
                {
                    Description = "Is Not Null",
                    Amount = isNotZero,
                    DateCreated = isTxDateCreated
                },
                new SFA.DAS.EAS.Finance.Api.Types.TransactionViewModel
                {
                    Description = "Is Not Null 2",
                    Amount = isNotZero,
                    DateCreated = isTxDateCreated
                }
            };
            transactionsViewModel.HasPreviousTransactions = false;
            transactionsViewModel.Year = year;            

            _financeApiService.Setup(x => x.GetTransactions(hashedAccountId, year, DateTime.Now.Month)).ReturnsAsync(transactionsViewModel);

            //Act
            var response = await _controller.GetTransactions(hashedAccountId, year);

            //Assert
            Assert.IsNotNull(response);
            Assert.IsInstanceOf<OkNegotiatedContentResult<TransactionsViewModel>>(response);
            var model = response as OkNegotiatedContentResult<TransactionsViewModel>;

            model?.Content.Should().NotBeNull();
            //model?.Content.ShouldAllBeEquivalentTo(transactionsResponse.Data.TransactionLines, options => options.Excluding(x => x.ResourceUri));
            model?.Content.PreviousMonthUri.Should().BeNullOrEmpty();
            _urlHelper.Verify(x => x.Route("GetTransactions", It.IsAny<object>()), Times.Never);
        }

        [Test]
        public async Task AndThereAreLevyTransactionsThenTheLinkIsCorrect()
        {
            //Arrange
            var hashedAccountId = "ABC123";
            var year = 2017;
            var month = 1;
            var levyTransaction = TransactionLineObjectMother.Create();
            var transactionsResponse = new GetEmployerAccountTransactionsResponse
            {
                Data = new AggregationData { TransactionLines = new List<TransactionLine> { levyTransaction } },
                AccountHasPreviousTransactions = false,
                Year = year,
                Month = month
            };
            _mediator.Setup(x => x.SendAsync(It.Is<GetEmployerAccountTransactionsQuery>(q => q.HashedAccountId == hashedAccountId && q.Year == year && q.Month == month))).ReturnsAsync(transactionsResponse);

            var isNotZero = 100m;
            var isTxDateCreated = DateTime.Today;
            var transactionsViewModel = new SFA.DAS.EAS.Finance.Api.Types.TransactionsViewModel
            {
                new SFA.DAS.EAS.Finance.Api.Types.TransactionViewModel
                {
                    Description = "Is Not Null",
                    Amount = isNotZero,
                    DateCreated = isTxDateCreated,
                    ResourceUri = "someuri"
                },
                new SFA.DAS.EAS.Finance.Api.Types.TransactionViewModel
                {
                    Description = "Is Not Null 2",
                    Amount = isNotZero,
                    DateCreated = isTxDateCreated
                }
            };
            transactionsViewModel.HasPreviousTransactions = true;
            transactionsViewModel.Year = year;
            transactionsViewModel.Month = month;

            _financeApiService.Setup(x => x.GetTransactions(hashedAccountId, year, month)).ReturnsAsync(transactionsViewModel);

            var expectedUri = "someuri";
            _urlHelper.Setup(
                    x =>
                        x.Route("GetLevyForPeriod",
                            It.Is<object>(o => o.IsEquivalentTo(new { hashedAccountId, payrollYear = levyTransaction.PayrollYear, payrollMonth = levyTransaction.PayrollMonth }))))
                .Returns(expectedUri);

            //Act
            var response = await _controller.GetTransactions(hashedAccountId, year, month);
            var model = response as OkNegotiatedContentResult<TransactionsViewModel>;

            
            //Assert
            model?.Content[0].ResourceUri.Should().Be(expectedUri);
        }

        [Test]
        public async Task AndNoYearIsProvidedThenTheCurrentYearIsUsed()
        {
            var hashedAccountId = "ABC123";
            var transactionsResponse = new GetEmployerAccountTransactionsResponse
            {
                Data = new AggregationData { TransactionLines = new List<TransactionLine> { TransactionLineObjectMother.Create() } },
                AccountHasPreviousTransactions = false
            };
            _mediator.Setup(x => x.SendAsync(It.Is<GetEmployerAccountTransactionsQuery>(q => q.HashedAccountId == hashedAccountId && q.Year == DateTime.Now.Year && q.Month == DateTime.Now.Month))).ReturnsAsync(transactionsResponse);

            var isNotZero = 100m;
            var isTxDateCreated = DateTime.Today;
            var transactionsViewModel = new SFA.DAS.EAS.Finance.Api.Types.TransactionsViewModel
            {
                new SFA.DAS.EAS.Finance.Api.Types.TransactionViewModel
                {
                    Description = "Is Not Null",
                    Amount = isNotZero,
                    DateCreated = isTxDateCreated,
                    ResourceUri = "someuri"
                },
                new SFA.DAS.EAS.Finance.Api.Types.TransactionViewModel
                {
                    Description = "Is Not Null 2",
                    Amount = isNotZero,
                    DateCreated = isTxDateCreated
                }
            };

            _financeApiService.Setup(x => x.GetTransactions(hashedAccountId, DateTime.Now.Year, DateTime.Now.Month)).ReturnsAsync(transactionsViewModel);

            //Act
            var response = await _controller.GetTransactions(hashedAccountId);

            
            //Assert
            Assert.IsNotNull(response);
            Assert.IsInstanceOf<OkNegotiatedContentResult<TransactionsViewModel>>(response);
            var model = response as OkNegotiatedContentResult<TransactionsViewModel>;

            model?.Content.Should().NotBeNull();
            //model?.Content.ShouldAllBeEquivalentTo(transactionsResponse.Data.TransactionLines, options => options.Excluding(x => x.ResourceUri));
        }

        private IMapper ConfigureMapper()
        {
            var profiles = Assembly.Load($"SFA.DAS.EAS.Account.Api")
                .GetTypes()
                .Where(t => typeof(Profile).IsAssignableFrom(t))
                .Select(t => (Profile)Activator.CreateInstance(t));

            var config = new MapperConfiguration(c =>
            {
                profiles.ForEach(c.AddProfile);
            });

            return config.CreateMapper();
        }
    }
}
