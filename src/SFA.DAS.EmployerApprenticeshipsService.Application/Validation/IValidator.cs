using SFA.DAS.EmployerApprenticeshipsService.Application.Queries.GetLevyDeclaration;

namespace SFA.DAS.EmployerApprenticeshipsService.Application.Validation
{
    public interface IValidator<T>
    {
        ValidationResult Validate(T item);
    }
}