using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using AutoMapper;
using Moq;
using NUnit.Framework;
using SFA.DAS.CommitmentsV2.Api.Client;
using SFA.DAS.CommitmentsV2.Api.Types.Responses;
using SFA.DAS.EmployerAccounts.Data;
using SFA.DAS.EmployerAccounts.Mappings;
using SFA.DAS.EmployerAccounts.Models.Account;
using SFA.DAS.EmployerAccounts.Queries.GetTransferRequests;
using SFA.DAS.HashingService;
using SFA.DAS.Testing.EntityFramework;

namespace SFA.DAS.EmployerAccounts.UnitTests.Queries.GetTransferRequestsTests
{
    [TestFixture]
    public class WhenIGetTransferRequests
    {
        private GetTransferRequestsQueryHandler _handler;
        private GetTransferRequestsQuery _query;
        private GetTransferRequestsResponse _response;
        private Mock<EmployerAccountsDbContext> _db;
        private IConfigurationProvider _configurationProvider;
        private Mock<ICommitmentsApiClient> _employerCommitmentApi;
        private Mock<IHashingService> _hashingService;       
        private GetTransferRequestSummaryResponse _getTransferRequestSummaryResponse;
        private DbSetStub<Account> _accountsDbSet;
        private List<Account> _accounts;
        private Account _account1;
        private Account _account2;

        [SetUp]
        public void Arrange()
        {
            _db = new Mock<EmployerAccountsDbContext>();
            _employerCommitmentApi = new Mock<ICommitmentsApiClient>();
            _hashingService = new Mock<IHashingService>();

            _account1 = new Account
            {
                Id = 111111,
                HashedId = "ABC123",
                Name = "Account 1"
            };

            _account2 = new Account
            {
                Id = 22222,
                HashedId = "ZYX987",
                Name = "Account 2"
            };           

            _accounts = new List<Account>
            {
                _account1,
                _account2
            };

            _accountsDbSet = new DbSetStub<Account>(_accounts);

            _configurationProvider = new MapperConfiguration(c =>
            {
                c.AddProfile<AccountMappings>();
            });


            _getTransferRequestSummaryResponse = new GetTransferRequestSummaryResponse
            {
                TransferRequestSummaryResponse = new List<TransferRequestSummaryResponse>
                {
                    new TransferRequestSummaryResponse
                    {
                        HashedSendingEmployerAccountId = _account1.HashedId,
                        HashedReceivingEmployerAccountId = _account2.HashedId,
                        CreatedOn  = DateTime.UtcNow,
                        Status = CommitmentsV2.Types.TransferApprovalStatus.Pending,
                        TransferCost =  123.456m,
                        HashedTransferRequestId = "DEF456",
                        TransferType = CommitmentsV2.Types.TransferType.AsSender
                    },
                     new TransferRequestSummaryResponse
                    {
                        HashedSendingEmployerAccountId = _account2.HashedId,
                        HashedReceivingEmployerAccountId = _account1.HashedId,
                        CreatedOn  = DateTime.UtcNow,
                        Status = CommitmentsV2.Types.TransferApprovalStatus.Pending,
                        TransferCost =  789.012m,
                        HashedTransferRequestId = "GHI789",
                        TransferType = CommitmentsV2.Types.TransferType.AsReceiver
                    }
                }
            };


            _db.Setup(d => d.Accounts).Returns(_accountsDbSet);
            _employerCommitmentApi.Setup(c => c.GetTransferRequests(_account1.Id, CancellationToken.None))
               .ReturnsAsync(_getTransferRequestSummaryResponse);
            _hashingService.Setup(h => h.DecodeValue(_account1.HashedId)).Returns(_account1.Id);
            _hashingService.Setup(h => h.DecodeValue(_account2.HashedId)).Returns(_account2.Id);
            _hashingService.Setup(h => h.HashValue(_account1.Id)).Returns(_account1.HashedId);
            _hashingService.Setup(h => h.HashValue(_account2.Id)).Returns(_account2.HashedId);

            _handler = new GetTransferRequestsQueryHandler(new Lazy<EmployerAccountsDbContext>(() => _db.Object), _configurationProvider, _employerCommitmentApi.Object, _hashingService.Object);

            _query = new GetTransferRequestsQuery
            {
                AccountId = _account1.Id
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
            Assert.That(sentTransferRequest.TransferRequestHashedId,
                Is.EqualTo(_getTransferRequestSummaryResponse.TransferRequestSummaryResponse.ElementAt(0).HashedTransferRequestId));
            Assert.That(sentTransferRequest.ReceiverAccount.HashedId, Is.EqualTo(_account2.HashedId));
            Assert.That(sentTransferRequest.SenderAccount.HashedId, Is.EqualTo(_account1.HashedId));
            Assert.That(sentTransferRequest.Status,
                Is.EqualTo(_getTransferRequestSummaryResponse.TransferRequestSummaryResponse.ElementAt(0).Status));
            Assert.That(sentTransferRequest.TransferCost,
                Is.EqualTo(_getTransferRequestSummaryResponse.TransferRequestSummaryResponse.ElementAt(0).TransferCost));

            var receivedTransferRequest = _response.TransferRequests.ElementAt(1);

            Assert.That(receivedTransferRequest, Is.Not.Null);
            Assert.That(receivedTransferRequest.TransferRequestHashedId, 
                Is.EqualTo(_getTransferRequestSummaryResponse.TransferRequestSummaryResponse.ElementAt(1).HashedTransferRequestId));
            Assert.That(receivedTransferRequest.ReceiverAccount.HashedId, Is.EqualTo(_account1.HashedId));
            Assert.That(receivedTransferRequest.SenderAccount.HashedId, Is.EqualTo(_account2.HashedId));
            Assert.That(receivedTransferRequest.Status, 
                Is.EqualTo(_getTransferRequestSummaryResponse.TransferRequestSummaryResponse.ElementAt(1).Status));
            Assert.That(receivedTransferRequest.TransferCost, 
                Is.EqualTo(_getTransferRequestSummaryResponse.TransferRequestSummaryResponse.ElementAt(1).TransferCost));
        }
    }
}