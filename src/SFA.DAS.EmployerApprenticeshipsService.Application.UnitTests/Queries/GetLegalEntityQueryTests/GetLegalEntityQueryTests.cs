using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using FluentAssertions;
using Moq;
using NUnit.Framework;
using SFA.DAS.EAS.Application.Mappings;
using SFA.DAS.EAS.Application.Queries.GetLegalEntity;
using SFA.DAS.EAS.Domain.Models.Account;
using SFA.DAS.EAS.Domain.Models.EmployerAgreement;
using SFA.DAS.EAS.Infrastructure.Data;
using SFA.DAS.EAS.TestCommon;

namespace SFA.DAS.EAS.Application.UnitTests.Queries.GetLegalEntityQueryTests
{
    [TestFixture]
    public class GetLegalEntityQueryTests : FluentTest<GetLegalEntityQueryTestsFixture>
    {
        [Test]
        public Task Handle_WhenGettingLegalEntity_ThenShouldReturnLegalEntity()
        {
            return RunAsync(f => f.Handle(), (f, r) => r.Should().NotBeNull()
                .And.Match<GetLegalEntityResponse>(r2 => 
                    r2.LegalEntity.LegalEntityId == f.LegalEntity.Id &&
                    r2.LegalEntity.Agreements.Count == 2 &&
                    r2.LegalEntity.Agreements.Any(a => a.TemplateVersionNumber == 1 && a.Status == Account.Api.Types.EmployerAgreementStatus.Signed) &&
                    r2.LegalEntity.Agreements.Any(a => a.TemplateVersionNumber == 2 && a.Status == Account.Api.Types.EmployerAgreementStatus.Pending) &&
                    r2.LegalEntity.AgreementStatus == Account.Api.Types.EmployerAgreementStatus.Pending));
        }
    }

    public class GetLegalEntityQueryTestsFixture : FluentTestFixture
    {
        public GetLegalEntityQueryHandler Handler { get; set; }
        public Mock<EmployerAccountDbContext> Db { get; set; }
        public IConfigurationProvider ConfigurationProvider { get; set; }
        public Domain.Models.Account.Account Account { get; private set; }
        public LegalEntity LegalEntity { get; set; }
        public DbSetStub<LegalEntity> LegalEntitiesDbSet { get; set; }
        public List<LegalEntity> LegalEntities { get; set; }


        public GetLegalEntityQueryTestsFixture()
        {
            LegalEntities = new List<LegalEntity>();
            LegalEntitiesDbSet = new DbSetStub<LegalEntity>(LegalEntities);
            Db = new Mock<EmployerAccountDbContext>();

            ConfigurationProvider = new MapperConfiguration(c =>
            {
                c.AddProfile<AgreementMappings>();
                c.AddProfile<LegalEntityMappings>();
            });

            Db.Setup(d => d.LegalEntities).Returns(LegalEntitiesDbSet);

            Handler = new GetLegalEntityQueryHandler(Db.Object, ConfigurationProvider);

            SetAccount()
                .SetLegalEntity()
                .AddLegalEntityAgreement(1, EmployerAgreementStatus.Removed)
                .AddLegalEntityAgreement(1, EmployerAgreementStatus.Signed)
                .AddLegalEntityAgreement(2, EmployerAgreementStatus.Pending);
        }

        public Task<GetLegalEntityResponse> Handle()
        {
            return Handler.Handle(new GetLegalEntityQuery
            {
                AccountId = Account.Id,
                AccountHashedId = Account.HashedId,
                LegalEntityId = LegalEntity.Id
            });
        }

        public GetLegalEntityQueryTestsFixture SetAccount()
        {
            Account = new Domain.Models.Account.Account
            {
                Id = 111111,
                HashedId = "ABC123"
            };

            return this;
        }

        private GetLegalEntityQueryTestsFixture SetLegalEntity()
        {
            LegalEntity = new LegalEntity
            {
                Id = 222222,
                Name = "Acme"
            };

            LegalEntities.Add(LegalEntity);

            return this;
        }

        private GetLegalEntityQueryTestsFixture AddLegalEntityAgreement(int versionNumber, EmployerAgreementStatus status)
        {
            var newAgreement = new EmployerAgreement
            {
                Template = new AgreementTemplate
                {
                    VersionNumber = versionNumber
                },
                StatusId = status
            };

            var newAccountLegalEntity = new AccountLegalEntity
            {
                Account = Account,
                LegalEntity = LegalEntity
            };

            newAccountLegalEntity.Agreements.Add(newAgreement);

            LegalEntity.AccountLegalEntities.Add(newAccountLegalEntity);

            return this;
        }
    }
}