using Moq;
using NUnit.Framework;
using SFA.DAS.EAS.TestCommon;
using SFA.DAS.EAS.TestCommon.Builders;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using SFA.DAS.EAS.Application.Mappings;
using SFA.DAS.EAS.Application.Queries.GetTransferRequests;
using SFA.DAS.EAS.Domain.Models.TransferRequests;
using SFA.DAS.EAS.Infrastructure.Data;

namespace SFA.DAS.EAS.Application.UnitTests.Queries.GetTransferRequestsTests
{
    [TestFixture]
    public class WhenIGetTransferRequests
    {
        private GetTransferRequestsQueryHandler _handler;
        private GetTransferRequestsQuery _query;
        private GetTransferRequestsResponse _response;
        private Mock<EmployerAccountDbContext> _db;
        private DbSetStub<TransferRequest> _transferRequestsDbSet;
        private List<TransferRequest> _transferRequests;
        private TransferRequest _sentTransferRequest;
        private TransferRequest _receivedTransferRequest;
        private Domain.Models.Account.Account _account;
        private IConfigurationProvider _configurationProvider;

        [SetUp]
        public void Arrange()
        {
            _db = new Mock<EmployerAccountDbContext>();

            _account = new Domain.Models.Account.Account
            {
                Id = 333333,
                HashedId = "ABC123",
                Name = "Account"
            };

            _sentTransferRequest = new TransferRequestBuilder()
                .WithCommitmentId(111111)
                .WithCommitmentHashedId("DEF456")
                .WithSenderAccount(_account)
                .WithReceiverAccount(new Domain.Models.Account.Account())
                .WithTransferCost(123.456m)
                .WithCreatedDate(DateTime.UtcNow)
                .Build();

            _receivedTransferRequest = new TransferRequestBuilder()
                .WithCommitmentId(222222)
                .WithCommitmentHashedId("GHI789")
                .WithSenderAccount(new Domain.Models.Account.Account())
                .WithReceiverAccount(_account)
                .WithTransferCost(789.012m)
                .WithCreatedDate(DateTime.UtcNow.AddDays(-1))
                .Build();

            _transferRequests = new List<TransferRequest>
            {
                _sentTransferRequest,
                _receivedTransferRequest,
                new TransferRequestBuilder()
                    .WithSenderAccount(new Domain.Models.Account.Account())
                    .WithReceiverAccount(new Domain.Models.Account.Account())
                    .WithCreatedDate(DateTime.UtcNow.AddDays(-2))
                    .Build()
            };

            _transferRequestsDbSet = new DbSetStub<TransferRequest>(_transferRequests);

            _configurationProvider = new MapperConfiguration(c =>
            {
                c.AddProfile<AccountMappings>();
                c.AddProfile<TransferRequestMappings>();
                c.AddProfile<UserMappings>();
            });

            _db.Setup(d => d.TransferRequests).Returns(_transferRequestsDbSet);

            _handler = new GetTransferRequestsQueryHandler(_db.Object, _configurationProvider);

            _query = new GetTransferRequestsQuery
            {
                AccountId = _account.Id
            };
        }

        [Test]
        public async Task ThenShouldReturnGetTransferRequestsResponse()
        {
            _response = await _handler.Handle(_query);

            Assert.That(_response, Is.Not.Null);
            Assert.That(_response.AccountId, Is.EqualTo(_account.Id));
            Assert.That(_response.TransferRequests.Count(), Is.EqualTo(2));

            var receivedTransferRequest = _response.TransferRequests.ElementAt(0);

            Assert.That(receivedTransferRequest, Is.Not.Null);
            Assert.That(receivedTransferRequest.CommitmentId, Is.EqualTo(_receivedTransferRequest.CommitmentId));
            Assert.That(receivedTransferRequest.CommitmentHashedId, Is.EqualTo(_receivedTransferRequest.CommitmentHashedId));
            Assert.That(receivedTransferRequest.ReceiverAccount.Id, Is.EqualTo(_receivedTransferRequest.ReceiverAccount.Id));
            Assert.That(receivedTransferRequest.SenderAccount.Id, Is.EqualTo(_receivedTransferRequest.SenderAccount.Id));
            Assert.That(receivedTransferRequest.Status, Is.EqualTo(_receivedTransferRequest.Status));
            Assert.That(receivedTransferRequest.TransferCost, Is.EqualTo(_receivedTransferRequest.TransferCost));

            var sentTransferRequest = _response.TransferRequests.ElementAt(1);

            Assert.That(sentTransferRequest, Is.Not.Null);
            Assert.That(sentTransferRequest.CommitmentId, Is.EqualTo(_sentTransferRequest.CommitmentId));
            Assert.That(sentTransferRequest.CommitmentHashedId, Is.EqualTo(_sentTransferRequest.CommitmentHashedId));
            Assert.That(sentTransferRequest.ReceiverAccount.Id, Is.EqualTo(_sentTransferRequest.ReceiverAccount.Id));
            Assert.That(sentTransferRequest.SenderAccount.Id, Is.EqualTo(_sentTransferRequest.SenderAccount.Id));
            Assert.That(sentTransferRequest.Status, Is.EqualTo(_sentTransferRequest.Status));
            Assert.That(sentTransferRequest.TransferCost, Is.EqualTo(_sentTransferRequest.TransferCost));
        }
    }
}