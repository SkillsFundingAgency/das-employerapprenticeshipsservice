using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SFA.DAS.EAS.Infrastructure.Exceptions
{
    public class InvalidOrganisationTypeConversionException : Exception
    {
        public InvalidOrganisationTypeConversionException(string message) : base(message)
        {
            // just call base    
        }
    }
}
