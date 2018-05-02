using Moq;
using NUnit.Framework;
using SFA.DAS.EAS.TestCommon;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using SFA.DAS.Commitments.Api.Client.Interfaces;
using SFA.DAS.Commitments.Api.Types.Commitment;
using SFA.DAS.EAS.Application.Mappings;
using SFA.DAS.EAS.Application.Queries.GetTransferRequests;
using SFA.DAS.EAS.Infrastructure.Data;
using SFA.DAS.HashingService;

namespace SFA.DAS.EAS.Application.UnitTests.Queries.GetTransferRequestsTests
{
    [TestFixture]
    public class WhenIGetTransferRequests
    {
        private GetTransferRequestsQueryHandler _handler;
        private GetTransferRequestsQuery _query;
        private GetTransferRequestsResponse _response;
        private Mock<EmployerAccountDbContext> _db;
        private IConfigurationProvider _configurationProvider;
        private Mock<IEmployerCommitmentApi> _employerCommitmentApi;
        private Mock<IHashingService> _hashingService;
        private List<TransferRequestSummary> _transferRequests;
        private TransferRequestSummary _sentTransferRequest;
        private TransferRequestSummary _receivedTransferRequest;
        private DbSetStub<Domain.Models.Account.Account> _accountsDbSet;
        private List<Domain.Models.Account.Account> _accounts;
        private Domain.Models.Account.Account _account1;
        private Domain.Models.Account.Account _account2;

        [SetUp]
        public void Arrange()
        {
            _db = new Mock<EmployerAccountDbContext>();
            _employerCommitmentApi = new Mock<IEmployerCommitmentApi>();
            _hashingService = new Mock<IHashingService>();

            _account1 = new Domain.Models.Account.Account
            {
                Id = 111111,
                HashedId = "ABC123",
                Name = "Account 1"
            };

            _account2 = new Domain.Models.Account.Account
            {
                Id = 22222,
                HashedId = "ZYX987",
                Name = "Account 2"
            };

            _sentTransferRequest = new TransferRequestSummary
            {
                HashedTransferRequestId = "DEF456",
                HashedSendingEmployerAccountId = _account1.HashedId,
                HashedReceivingEmployerAccountId = _account2.HashedId,
                TransferCost = 123.456m
            };

            _receivedTransferRequest = new TransferRequestSummary
            {
                HashedTransferRequestId = "GHI789",
                HashedSendingEmployerAccountId = _account2.HashedId,
                HashedReceivingEmployerAccountId = _account1.HashedId,
                TransferCost = 789.012m
            };

            _transferRequests = new List<TransferRequestSummary>
            {
                _sentTransferRequest,
                _receivedTransferRequest
            };

            _accounts = new List<Domain.Models.Account.Account>
            {
                _account1,
                _account2
            };

            _accountsDbSet = new DbSetStub<Domain.Models.Account.Account>(_accounts);

            _configurationProvider = new MapperConfiguration(c =>
            {
                c.AddProfile<AccountMappings>();
            });

            _db.Setup(d => d.Accounts).Returns(_accountsDbSet);
            _employerCommitmentApi.Setup(c => c.GetTransferRequests(_account1.HashedId)).ReturnsAsync(_transferRequests);
            _hashingService.Setup(h => h.DecodeValue(_account1.HashedId)).Returns(_account1.Id);
            _hashingService.Setup(h => h.DecodeValue(_account2.HashedId)).Returns(_account2.Id);

            _handler = new GetTransferRequestsQueryHandler(_db.Object, _configurationProvider, _employerCommitmentApi.Object, _hashingService.Object);

            _query = new GetTransferRequestsQuery
            {
                AccountId = _account1.Id,
                AccountHashedId = _account1.HashedId
            };
        }

        [Test]
        public async Task ThenShouldReturnGetTransferRequestsResponse()
        {
            _response = await _handler.Handle(_query);

            Assert.That(_response, Is.Not.Null);
            Assert.That(_response.TransferRequests.Count(), Is.EqualTo(2));

            var sentTransferRequest = _response.TransferRequests.ElementAt(0);

            Assert.That(sentTransferRequest, Is.Not.Null);
            Assert.That(sentTransferRequest.TransferRequestHashedId, Is.EqualTo(_sentTransferRequest.HashedTransferRequestId));
            Assert.That(sentTransferRequest.ReceiverAccount.HashedId, Is.EqualTo(_account2.HashedId));
            Assert.That(sentTransferRequest.SenderAccount.HashedId, Is.EqualTo(_account1.HashedId));
            Assert.That(sentTransferRequest.Status, Is.EqualTo(_sentTransferRequest.Status));
            Assert.That(sentTransferRequest.TransferCost, Is.EqualTo(_sentTransferRequest.TransferCost));

            var receivedTransferRequest = _response.TransferRequests.ElementAt(1);

            Assert.That(receivedTransferRequest, Is.Not.Null);
            Assert.That(receivedTransferRequest.TransferRequestHashedId, Is.EqualTo(_receivedTransferRequest.HashedTransferRequestId));
            Assert.That(receivedTransferRequest.ReceiverAccount.HashedId, Is.EqualTo(_account1.HashedId));
            Assert.That(receivedTransferRequest.SenderAccount.HashedId, Is.EqualTo(_account2.HashedId));
            Assert.That(receivedTransferRequest.Status, Is.EqualTo(_receivedTransferRequest.Status));
            Assert.That(receivedTransferRequest.TransferCost, Is.EqualTo(_receivedTransferRequest.TransferCost));
        }
    }
}