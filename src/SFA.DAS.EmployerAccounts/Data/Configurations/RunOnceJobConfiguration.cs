using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SFA.DAS.EmployerAccounts.Models;

namespace SFA.DAS.EmployerAccounts.Data.Configurations;

public class RunOnceJobConfiguration: IEntityTypeConfiguration<RunOnceJob>
{
    public void Configure(EntityTypeBuilder<RunOnceJob> builder)
    {
        builder.ToTable("RunOnceJob", "dbo");
        builder.HasKey(j => j.Name);
    }
}