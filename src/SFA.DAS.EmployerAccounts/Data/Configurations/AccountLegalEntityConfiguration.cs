using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SFA.DAS.EmployerAccounts.Models.Account;

namespace SFA.DAS.EmployerAccounts.Data.Configurations;

public class AccountLegalEntityConfiguration : IEntityTypeConfiguration<AccountLegalEntity>
{
    public void Configure(EntityTypeBuilder<AccountLegalEntity> builder)
    {
        builder.ToTable("AccountLegalEntity");
        
        //builder.HasMany(x => x.Agreements)
        //    .WithOne(x=> x.AccountLegalEntity)
        //    .HasForeignKey(k => k.AccountLegalEntityId);

        builder.HasOne(x => x.PendingAgreement)
            .WithMany()
            .HasForeignKey(y => y.PendingAgreementId);


        builder.HasOne(x => x.SignedAgreement)
            .WithMany()
            .HasForeignKey(y => y.SignedAgreementId);

    }
}