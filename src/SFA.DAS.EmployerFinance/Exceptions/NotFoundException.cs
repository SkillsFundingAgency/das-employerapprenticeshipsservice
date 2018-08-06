using System;

namespace SFA.DAS.EmployerFinance.Exceptions
{
    [Obsolete("Return null and use HttpNotFoundForNullModelAttribute instead.")]
    public class NotFoundException : Exception
    {
        public string ErrorMessage { get; set; }

        public NotFoundException(string errorMessage)
        {
            ErrorMessage = errorMessage;
        }   
    }
}