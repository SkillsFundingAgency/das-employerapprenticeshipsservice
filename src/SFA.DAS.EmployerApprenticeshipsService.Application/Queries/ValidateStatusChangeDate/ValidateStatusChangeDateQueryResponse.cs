using System;
using SFA.DAS.EAS.Application.Validation;

namespace SFA.DAS.EAS.Application.Queries.ValidateStatusChangeDate
{
    public sealed class ValidateStatusChangeDateQueryResponse
    {
        public ValidationResult ValidationResult { get; set; }
        public DateTime ValidatedChangeOfDate { get; internal set; }
    }
}
