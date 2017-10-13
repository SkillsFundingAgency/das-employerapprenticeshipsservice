using System;
using SFA.DAS.EAS.Application.Validation;

namespace SFA.DAS.EAS.Web.ViewModels.ManageApprenticeships
{
    public sealed class ValidateWhenToApplyChangeResult
    {
        public ValidationResult ValidationResult { get; set; }
        public DateTime DateOfChange { get; internal set; }
    }
}