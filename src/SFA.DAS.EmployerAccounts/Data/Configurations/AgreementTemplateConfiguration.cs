using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SFA.DAS.EmployerAccounts.Models.Account;

namespace SFA.DAS.EmployerAccounts.Data.Configurations;

public class AgreementTemplateConfiguration : IEntityTypeConfiguration<AgreementTemplate>
{
    public void Configure(EntityTypeBuilder<AgreementTemplate> builder)
    {
        builder
            .ToTable("EmployerAgreementTemplate")
            .HasMany(t => t.Agreements);
    }
}