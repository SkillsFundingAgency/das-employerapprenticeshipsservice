using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SFA.DAS.EmployerAccounts.Models.Account;

namespace SFA.DAS.EmployerAccounts.Data.Configurations;

public class AccountLegalEntityConfiguration : IEntityTypeConfiguration<AccountLegalEntity>
{
    public void Configure(EntityTypeBuilder<AccountLegalEntity> builder)
    {
        builder.ToTable("AccountLegalEntity");
        builder.HasMany(x => x.Agreements);

        builder.HasOne(x => x.PendingAgreement);
        builder.HasOne(x => x.SignedAgreement);

    }
}