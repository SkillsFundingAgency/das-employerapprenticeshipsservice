using System.Linq;
using FluentAssertions;
using NUnit.Framework;
using SFA.DAS.EmployerAccounts.Api.Mappings;
using SFA.DAS.EmployerAccounts.Api.Types;
using SFA.DAS.EmployerAccounts.Models.Account;
using SFA.DAS.Testing.AutoFixture;
using AccountLegalEntity = SFA.DAS.EmployerAccounts.Models.Account.AccountLegalEntity;

namespace SFA.DAS.EmployerAccounts.Api.UnitTests.Mappings
{
    public class WhenMappingFromAccountLegalEntityToApiTypeLegalEntity
    {
        [Test, RecursiveMoqAutoData]
        public void Then_The_Fields_Are_Mapped_And_All_Agreements_Included(AccountLegalEntity source, EmployerAgreement agreement)
        {
            var actual = LegalEntityMapping.MapFromAccountLegalEntity(source, agreement, true);

            actual.Address.Should().Be(source.Address);
            actual.Code.Should().Be(source.LegalEntity.Code);
            actual.Name.Should().Be(source.Name);
            actual.DasAccountId.Should().Be(source.Account.HashedId);
            actual.AccountLegalEntityId.Should().Be(source.Id);
            actual.AccountLegalEntityPublicHashedId.Should().Be(source.PublicHashedId);
            actual.LegalEntityId.Should().Be(source.LegalEntityId);
            actual.DateOfInception.Should().Be(source.LegalEntity.DateOfIncorporation);
            actual.Code.Should().Be(source.LegalEntity.Code);
            actual.Sector.Should().Be(source.LegalEntity.Sector);
            actual.Status.Should().Be(source.LegalEntity.Status);
            actual.SourceNumeric.Should().Be((short)source.LegalEntity.Source);
            foreach (var actualAgreement in actual.Agreements)
            {
                var expectedAgreement = source.Agreements.Single(c => c.Id.Equals(actualAgreement.Id));
                actualAgreement.Id.Should().Be(expectedAgreement.Id);
                actualAgreement.AgreementType.Should().Be(expectedAgreement.Template.AgreementType);
                actualAgreement.TemplateVersionNumber.Should().Be(expectedAgreement.Template.VersionNumber);
                actualAgreement.Status.Should().Be((EmployerAgreementStatus)(int)expectedAgreement.StatusId);
                actualAgreement.SignedById.Should().Be(expectedAgreement.SignedById);
                actualAgreement.SignedByName.Should().Be(expectedAgreement.SignedByName);
                actualAgreement.SignedDate.Should().Be(expectedAgreement.SignedDate);
                actualAgreement.SignedByEmail.Should().Be(expectedAgreement.SignedByEmail);
            }

            actual.AgreementSignedByName.Should().Be(agreement.SignedByName);
            actual.AgreementSignedDate.Should().Be(agreement.SignedDate);
            actual.AgreementStatus.Should().Be((EmployerAgreementStatus)(int)agreement.StatusId);

        }

        [Test, RecursiveMoqAutoData]
        public void Then_If_No_Agreement_Then_Not_Mapped(AccountLegalEntity source)
        {
            var actual = LegalEntityMapping.MapFromAccountLegalEntity(source, null, true);

            actual.DasAccountId.Should().Be(source.Account.HashedId);
            actual.AccountLegalEntityId.Should().Be(source.Id);
            actual.AccountLegalEntityPublicHashedId.Should().Be(source.PublicHashedId);
            actual.AgreementSignedByName.Should().BeNullOrEmpty();
            actual.AgreementSignedDate.HasValue.Should().BeFalse();
        }

        [Test, RecursiveMoqAutoData]
        public void Then_The_Fields_Are_Mapped_And_Signed_And_Pending_Agreements_Included(AccountLegalEntity source,
            EmployerAgreement signedAgreement, 
            EmployerAgreement expiredAgreement, 
            EmployerAgreement removedAgreement, 
            EmployerAgreement supersededAgreement, 
            EmployerAgreement agreement)
        {
            expiredAgreement.StatusId = Models.EmployerAgreement.EmployerAgreementStatus.Expired;
            signedAgreement.StatusId = Models.EmployerAgreement.EmployerAgreementStatus.Signed;
            removedAgreement.StatusId = Models.EmployerAgreement.EmployerAgreementStatus.Removed;
            supersededAgreement.StatusId = Models.EmployerAgreement.EmployerAgreementStatus.Superseded;
            foreach (var employerAgreement in source.Agreements)
            {
                employerAgreement.StatusId = Models.EmployerAgreement.EmployerAgreementStatus.Pending;
            }
            source.Agreements.Add(expiredAgreement);
            source.Agreements.Add(signedAgreement);
            source.Agreements.Add(removedAgreement);
            source.Agreements.Add(supersededAgreement);
            
            var actual = LegalEntityMapping.MapFromAccountLegalEntity(source, agreement, false);

            actual.Agreements.Count.Should().Be(source.Agreements.Count + 1 - 4);
            actual.Agreements.TrueForAll(c =>
                    c.Status == EmployerAgreementStatus.Signed || c.Status == EmployerAgreementStatus.Pending).Should()
                .BeTrue();
        }
        
        [Test]
        public void Then_If_The_Source_Is_Null_Then_Null_Returned()
        {
            var actual = LegalEntityMapping.MapFromAccountLegalEntity(null, null, false);

            actual.Should().BeNull();
        }
    }
}