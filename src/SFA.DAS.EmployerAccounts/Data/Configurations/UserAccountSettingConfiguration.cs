using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace SFA.DAS.EmployerAccounts.Data.Configurations;

public class UserAccountSettingConfiguration : IEntityTypeConfiguration<UserAccountSetting>
{
    public void Configure(EntityTypeBuilder<UserAccountSetting> builder)
    {
        builder.ToTable("UserAccountSettings");
        builder.Property(u => u.Account).IsRequired();
        builder.Property(u => u.User).IsRequired();
    }
}