using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using AutoMapper;
using FluentAssertions;
using MediatR;
using Moq;
using NUnit.Framework;
using SFA.DAS.EmployerAccounts.Data;
using SFA.DAS.EmployerAccounts.Models.Account;
using SFA.DAS.EmployerAccounts.Queries.GetAccountLegalEntitiesCountByHashedAccountId;
using SFA.DAS.EmployerAccounts.TestCommon;
using SFA.DAS.HashingService;
using SFA.DAS.Validation;

namespace SFA.DAS.EmployerAccounts.UnitTests.Queries.GetAccountLegalEntitiesCountByHashedAccountId
{
    [TestFixture]
    public class GetAccountLegalEntitiesCountByHashedAccountIdTests : Testing.FluentTest<GetAccountLegalEntitiesCountByHashedAccountIdTestsFixture>
    {
        [Test]
        public Task Handle_WhenHandlingAGetAccountLegalEntitiesCountByHashedAccountIdRequest_ThenShouldReturnTheNumberOfLegalEntitiesForAnAccount()
        {
            return TestAsync(f => f.Handle(), (f, r) =>
            {
                r.Should().NotBeNull();
                r.LegalEntitiesCount.Should().Be(1);
            });
        }
    }

    public class GetAccountLegalEntitiesCountByHashedAccountIdTestsFixture
    {
        private const string HashedId = "123";
        private const long AccountId = 456;

        public GetAccountLegalEntitiesCountByHashedAccountIdRequest GetAccountLegalEntitiesCountByHashedAccountIdRequest { get; set; }
        public IRequestHandler<GetAccountLegalEntitiesCountByHashedAccountIdRequest, GetAccountLegalEntitiesCountByHashedAccountIdResponse> Handler { get; set; }
        public Mock<EmployerAccountsDbContext> Db { get; set; }
        public IConfigurationProvider ConfigurationProvider { get; set; }
        public List<AccountLegalEntity> AccountLegalEntities { get; set; }
        public Mock<IHashingService> HashingService { get; set; }
        public Mock<IValidator<GetAccountLegalEntitiesCountByHashedAccountIdRequest>> RequestValidator { get; set; }

        public GetAccountLegalEntitiesCountByHashedAccountIdTestsFixture()
        {
            long decodeAccountId = AccountId;

            GetAccountLegalEntitiesCountByHashedAccountIdRequest = new GetAccountLegalEntitiesCountByHashedAccountIdRequest { HashedAccountId = HashedId };
            Db = new Mock<EmployerAccountsDbContext>();

            RequestValidator = new Mock<IValidator<GetAccountLegalEntitiesCountByHashedAccountIdRequest>>();
            RequestValidator.Setup(x => x.Validate(It.IsAny<GetAccountLegalEntitiesCountByHashedAccountIdRequest>())).Returns(new ValidationResult());

            HashingService = new Mock<IHashingService>();
            HashingService.Setup(m => m.TryDecodeValue(HashedId, out decodeAccountId)).Returns(true);

            AccountLegalEntities = new List<AccountLegalEntity>
            {
                new AccountLegalEntity { AccountId = AccountId },
                new AccountLegalEntity { AccountId = 0 }
            };

            Db.Setup(d => d.AccountLegalEntities).Returns(new DbSetStub<AccountLegalEntity>(AccountLegalEntities));

            Handler = new GetAccountLegalEntitiesCountByHashedAccountIdQueryHandler(
                HashingService.Object,
                new Lazy<EmployerAccountsDbContext>(() => Db.Object), 
                RequestValidator.Object);
        }

        public Task<GetAccountLegalEntitiesCountByHashedAccountIdResponse> Handle()
        {
            return Handler.Handle(GetAccountLegalEntitiesCountByHashedAccountIdRequest, CancellationToken.None);
        }
    }
}