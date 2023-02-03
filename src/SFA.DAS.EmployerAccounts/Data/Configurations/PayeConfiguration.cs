using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SFA.DAS.EmployerAccounts.Models.PAYE;

namespace SFA.DAS.EmployerAccounts.Data.Configurations;

public class PayeConfiguration: IEntityTypeConfiguration<Paye>
{
    public void Configure(EntityTypeBuilder<Paye> builder)
    {
        builder.ToTable("Paye");
        builder.Ignore(a => a.AccountId);
    }
}