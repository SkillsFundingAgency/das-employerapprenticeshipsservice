using SFA.DAS.Validation;

namespace SFA.DAS.EmployerAccounts.Validation;

public interface IValidator<in T>
{
    Task<ValidationResult> ValidateAsync(T query);
}