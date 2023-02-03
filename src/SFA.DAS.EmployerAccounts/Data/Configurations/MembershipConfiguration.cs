using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace SFA.DAS.EmployerAccounts.Data.Configurations;

public class MembershipConfiguration: IEntityTypeConfiguration<Membership>
{
    public void Configure(EntityTypeBuilder<Membership> builder)
    {
        builder.ToTable("Membership");
        builder.HasKey(m => new { m.AccountId, m.UserId });
    }
}