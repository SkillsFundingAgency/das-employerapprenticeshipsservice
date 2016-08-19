using System.Threading.Tasks;

namespace SFA.DAS.EmployerApprenticeshipsService.Application.Validation
{
    public interface IValidator<T>
    {
        ValidationResult Validate(T item);

        Task<ValidationResult> ValidateAsync(T item);
    }
}