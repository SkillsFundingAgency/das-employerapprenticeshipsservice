using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SFA.DAS.EmployerAccounts.Models;

namespace SFA.DAS.EmployerAccounts.Data.Configurations;

public class HealthCheckConfiguration: IEntityTypeConfiguration<HealthCheck>
{
    public void Configure(EntityTypeBuilder<HealthCheck> builder)
    {
        builder.ToTable("HealthChecks", "dbo");
    }
}