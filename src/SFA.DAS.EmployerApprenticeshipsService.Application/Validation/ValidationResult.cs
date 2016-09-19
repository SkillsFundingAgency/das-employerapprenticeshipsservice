using System.Collections.Generic;
using System.Linq;

namespace SFA.DAS.EmployerApprenticeshipsService.Application.Validation
{
    public class ValidationResult
    {
        public ValidationResult()
        {
            ValidationDictionary = new Dictionary<string, string>();
        }

        public Dictionary<string, string> ValidationDictionary { get; set; }

        public void AddError(string propertyName, string validationError)
        {
            ValidationDictionary.Add(propertyName, validationError);
        }

        public bool IsValid()
        {
            if (ValidationDictionary == null)
            {
                return false;
            }

            return !ValidationDictionary.Any();
        }

        public bool IsUnauthorized { get; set; }
    }
}