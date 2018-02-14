using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Globalization;
using MediatR;
using SFA.DAS.EAS.Application.Formatters.TransactionDowloads;
using SFA.DAS.EAS.Application.Helpers;
using SFA.DAS.EAS.Application.Messages;
using SFA.DAS.EAS.Domain;

namespace SFA.DAS.EAS.Application.Queries.GetTransactionsDownload
{
    public class GetTransactionsDownloadQuery : IAsyncRequest<GetTransactionsDownloadResponse>, IValidatableObject
    {
        [Required]
        [RegularExpression(Constants.HashedAccountIdRegex)]
        public string HashedAccountId { get; set; }

        [Required]
        public MonthYear StartDate { get; set; }

        [Required]
        public MonthYear EndDate { get; set; }

        [Required]
        public DownloadFormatType? DownloadFormat { get; set; }

        public IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
        {
            var now = DateTime.Now;
            var today = now.Date;
            
            if (StartDate.Month == null)
            {
                yield return new ValidationResult("Enter a start month", new[] { NameOf.Property(this, q => q.StartDate.Month) });
            }

            if (StartDate.Year == null)
            {
                yield return new ValidationResult("Enter a start year", new[] { NameOf.Property(this, q => q.StartDate.Year) });
            }
                
            if (StartDate.Month != null && StartDate.Year != null && !StartDate.IsValid())
            {
                yield return new ValidationResult("Enter a different start date", new[] { NameOf.Property(this, q => q.StartDate.Month) });
            }
                
            if (StartDate.Month != null && StartDate.Year != null && StartDate.IsValid() && StartDate > today)
            {
                yield return new ValidationResult($"The latest start date you can enter is {today:MM yyyy}", new[] { NameOf.Property(this, q => q.StartDate.Month) });
            }

            if (EndDate.Month == null)
            {
                yield return new ValidationResult("Enter an end month", new[] { NameOf.Property(this, q => q.EndDate.Month) });
            }

            if (EndDate.Year == null)
            {
                yield return new ValidationResult("Enter an end year", new[] { NameOf.Property(this, q => q.EndDate.Year) });
            }

            if (EndDate.Month != null && EndDate.Year != null && !EndDate.IsValid())
            {
                yield return new ValidationResult("Enter a different end date", new[] { NameOf.Property(this, q => q.EndDate.Month) });
            }

            if (EndDate.Month != null && EndDate.Year != null && EndDate.IsValid() && EndDate > today)
            {
                yield return new ValidationResult($"The latest end date you can enter is {today:MM yyyy}", new[] { NameOf.Property(this, q => q.EndDate.Month) });
            }

            var earliestDate = DateTime.ParseExact("19000101", "yyyyMMdd", CultureInfo.InvariantCulture);

            if (EndDate.Month != null && EndDate.Year != null && EndDate.IsValid() && EndDate < earliestDate)
            {
                yield return new ValidationResult($"The earliest end date you can enter is {earliestDate:MM yyyy}", new[] { NameOf.Property(this, q => q.EndDate.Month) });
            }

            if (StartDate.Month != null && StartDate.Year != null && StartDate.IsValid() && StartDate < earliestDate)
            {
                yield return new ValidationResult($"The earliest start date you can enter is {earliestDate:MM yyyy}", new[] { NameOf.Property(this, q => q.StartDate.Month) });
            }

        }
    }
}