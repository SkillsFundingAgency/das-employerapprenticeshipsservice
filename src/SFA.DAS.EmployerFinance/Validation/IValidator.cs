namespace SFA.DAS.EmployerFinance.Validation
{
    public interface IValidator<T>
    {
        ValidationResult Validate(T item);
    }
}