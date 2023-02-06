using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SFA.DAS.EmployerAccounts.Models.Account;

namespace SFA.DAS.EmployerAccounts.Data.Configurations;

public class EmployerAgreementConfiguration: IEntityTypeConfiguration<EmployerAgreement>
{
    public void Configure(EntityTypeBuilder<EmployerAgreement> builder)
    {
        builder.ToTable("EmployerAgreement");
        builder.Ignore(c => c.SignedByEmail);
    }
}