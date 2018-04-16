using System.Collections.Generic;
using System.Threading.Tasks;
using Moq;
using NUnit.Framework;
using SFA.DAS.EAS.Account.Worker.Jobs;
using SFA.DAS.EAS.Domain.Data.Repositories;
using SFA.DAS.EAS.Domain.Interfaces;
using SFA.DAS.EAS.Domain.Models.Payments;
using SFA.DAS.NLog.Logger;

namespace SFA.DAS.EAS.Account.Worker.UnitTests.Jobs.PaymentIntegrityCheckerJobTests
{
    public class WhenCheckerJobIsRun
    {
        private const long AccountId = 1234;

        private PaymentIntegrityCheckerJob _job;
        private Mock<IPaymentService> _paymentService;
        private Mock<IDasLevyRepository> _levyRepository;
        private Mock<ILog> _logger;
        private Mock<IEmployerAccountRepository> _employerAccountRepository;
        private PeriodEnd _periodEnd;



        [SetUp]
        public void Arrange()
        {
            _paymentService = new Mock<IPaymentService>();
            _employerAccountRepository = new Mock<IEmployerAccountRepository>();
            _levyRepository = new Mock<IDasLevyRepository>();
            _logger = new Mock<ILog>();

            _job = new PaymentIntegrityCheckerJob(_paymentService.Object, _employerAccountRepository.Object, _levyRepository.Object, _logger.Object);


            _employerAccountRepository.Setup(x => x.GetAllAccounts())
                .ReturnsAsync(new List<Domain.Data.Entities.Account.Account> { new Domain.Data.Entities.Account.Account() { Id = AccountId } });


            _periodEnd = new PeriodEnd { Id = "1718-R01" };

            _levyRepository.Setup(x => x.GetAllPeriodEnds()).ReturnsAsync(new List<PeriodEnd> { _periodEnd });

            var paymentServicePayments = new List<PaymentDetails>
            {
                new PaymentDetails{CollectionPeriodId = _periodEnd.Id, Amount = 200, EmployerAccountId = AccountId}
            };

            _paymentService.Setup(x => x.GetAccountPayments(It.IsAny<string>(), It.IsAny<long>()))
                .ReturnsAsync(paymentServicePayments);

            var repositoryPayments = new List<Payment>
            {
                new Payment {CollectionPeriodId = _periodEnd.Id, Amount = 200, EmployerAccountId = AccountId}
            };

            _levyRepository.Setup(x => x.GetAccountPaymentsByPeriodEnd(It.IsAny<long>(), It.IsAny<string>()))
                .ReturnsAsync(repositoryPayments);

        }

        [Test]
        public async Task ThenIfPaymentsAreCorrectNoLogsAreCreated()
        {
            //Act
            await _job.Run();

            //Assert
            _logger.Verify(x => x.Warn(It.IsAny<string>()), Times.Never);
        }

        [Test]
        public async Task ThenItShouldGetAllAccounts()
        {
            //Act
            await _job.Run();

            //Assert
            _employerAccountRepository.Verify(x => x.GetAllAccounts(), Times.Once);
        }

        [Test]
        public async Task ThenItShouldGetLatestPeriodEnd()
        {
            //Act
            await _job.Run();

            //Assert
            _levyRepository.Verify(x => x.GetAllPeriodEnds(), Times.Once);
        }

        [Test]
        public async Task ThenItShouldGetPaymentsForPeriodEnd()
        {
            //Act
            await _job.Run();

            //Assert
            _paymentService.Verify(x => x.GetAccountPayments(_periodEnd.Id, AccountId), Times.Once);
        }

        [Test]
        public async Task ThenItShouldGetPaymentsFromRepository()
        {
            //Act
            await _job.Run();

            //Assert
            _levyRepository.Verify(x => x.GetAccountPaymentsByPeriodEnd(AccountId, _periodEnd.Id), Times.Once);
        }

        [Test]
        public async Task ThenIfRepositoryIsMissingPaymentItIsLogged()
        {
            //Assign
            var paymentServicePayments = new List<PaymentDetails>
            {
                new PaymentDetails{PeriodEnd = _periodEnd.Id, Amount = 200, EmployerAccountId = AccountId},
                new PaymentDetails{PeriodEnd = _periodEnd.Id, Amount = 400, EmployerAccountId = AccountId}
            };

            _paymentService.Setup(x => x.GetAccountPayments(It.IsAny<string>(), It.IsAny<long>()))
                .ReturnsAsync(paymentServicePayments);

            //Act
            await _job.Run();

            //Assert
            _logger.Verify(x => x.Warn(It.IsAny<string>()), Times.Once);

        }

        [Test]
        public async Task ThenIfPaymentServiceIsMissingPaymentsItIsLogged()
        {
            //Arrange
            var repositoryPayments = new List<Payment>
            {
                new Payment {CollectionPeriodId = _periodEnd.Id, Amount = 200, EmployerAccountId = AccountId},
                new Payment {CollectionPeriodId = _periodEnd.Id, Amount = 300, EmployerAccountId = AccountId}
            };

            _levyRepository.Setup(x => x.GetAccountPaymentsByPeriodEnd(It.IsAny<long>(), It.IsAny<string>()))
                .ReturnsAsync(repositoryPayments);

            //Act
            await _job.Run();

            //Assert
            _logger.Verify(x => x.Warn(It.IsAny<string>()), Times.Once);
        }

        [Test]
        public async Task ThenIfPaymentTotalIsSameButPaymentsAreNotItShouldBeLogged()
        {
            //Assign
            var paymentServicePayments = new List<PaymentDetails>
            {
                new PaymentDetails{PeriodEnd = _periodEnd.Id, Amount = 200, EmployerAccountId = AccountId, ApprenticeshipId = 1},
                new PaymentDetails{PeriodEnd = _periodEnd.Id, Amount = 200, EmployerAccountId = AccountId, ApprenticeshipId = 2}
            };

            _paymentService.Setup(x => x.GetAccountPayments(It.IsAny<string>(), It.IsAny<long>()))
                .ReturnsAsync(paymentServicePayments);

            var repositoryPayments = new List<Payment>
            {
                new Payment {CollectionPeriodId = _periodEnd.Id, Amount = 200, EmployerAccountId = AccountId, ApprenticeshipId = 1},
                new Payment {CollectionPeriodId = _periodEnd.Id, Amount = 200, EmployerAccountId = AccountId, ApprenticeshipId = 1}
            };

            _levyRepository.Setup(x => x.GetAccountPaymentsByPeriodEnd(It.IsAny<long>(), It.IsAny<string>()))
                .ReturnsAsync(repositoryPayments);

            //Act
            await _job.Run();

            //Assert
            _logger.Verify(x => x.Warn(It.IsAny<string>()), Times.Once);

        }
    }
}
