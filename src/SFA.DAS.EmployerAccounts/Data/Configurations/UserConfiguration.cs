using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace SFA.DAS.EmployerAccounts.Data.Configurations;

public class UserConfiguration : IEntityTypeConfiguration<User>
{
    public void Configure(EntityTypeBuilder<User> builder)
    {
        builder.ToTable("User");
        builder.Ignore(u => u.FullName);
        builder.Ignore(u => u.UserRef);
        builder.Property(u => u.Ref).HasColumnName(nameof(User.UserRef));
    }
}