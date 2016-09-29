using System.Collections.Generic;
using SFA.DAS.EmployerApprenticeshipsService.Domain.Entities.Account;

namespace SFA.DAS.EmployerApprenticeshipsService.Web.Models
{
    public class BeginNewPayeScheme
    {
        public string HashedId { get; set; }
        public bool ValidationFailed { get; set; }
    }
}