namespace SFA.DAS.EmployerAccounts.Validation
{
    public interface IValidator<T>
    {
        ValidationResult Validate(T item);
    }
}