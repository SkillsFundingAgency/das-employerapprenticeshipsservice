using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace SFA.DAS.EmployerAccounts.Data.Configurations;

public class UserAccountSettingConfiguration : IEntityTypeConfiguration<UserAccountSetting>
{
    public void Configure(EntityTypeBuilder<UserAccountSetting> builder)
    {
        builder.ToTable("UserAccountSettings");
    }
}