using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SFA.DAS.EmployerAccounts.Models.Account;

namespace SFA.DAS.EmployerAccounts.Data.Configurations;

public class EmployerAgreementConfiguration: IEntityTypeConfiguration<EmployerAgreement>
{
    public void Configure(EntityTypeBuilder<EmployerAgreement> builder)
    {
        builder.Property(a => a.AccountLegalEntity).IsRequired();
        builder.Property(a => a.Template).IsRequired();
        builder.Ignore(c => c.SignedByEmail);
    }
}