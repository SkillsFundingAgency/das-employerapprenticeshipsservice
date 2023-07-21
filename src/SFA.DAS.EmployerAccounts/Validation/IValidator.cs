namespace SFA.DAS.EmployerAccounts.Validation;

public interface IValidator<in T>
{
    ValidationResult Validate(T query);
    Task<ValidationResult> ValidateAsync(T query);
}