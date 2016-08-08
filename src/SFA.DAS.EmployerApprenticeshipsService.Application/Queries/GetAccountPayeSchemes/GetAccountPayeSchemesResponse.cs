using System.Collections.Generic;
using SFA.DAS.EmployerApprenticeshipsService.Domain;

namespace SFA.DAS.EmployerApprenticeshipsService.Application.Queries.GetAccountPayeSchemes
{
    public class GetAccountPayeSchemesResponse
    {
        public List<PayeView> PayeSchemes { get; set; }
    }
}