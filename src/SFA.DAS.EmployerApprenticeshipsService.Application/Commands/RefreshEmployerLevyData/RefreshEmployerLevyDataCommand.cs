using System.Collections.Generic;
using MediatR;
using SFA.DAS.EmployerApprenticeshipsService.Domain.Models.Levy;

namespace SFA.DAS.EmployerApprenticeshipsService.Application.Commands.RefreshEmployerLevyData
{
    public class RefreshEmployerLevyDataCommand : IAsyncRequest
    {
        public int employerId;
        public List<EmployerLevyData> EmployerLevyData { get; set; }
   
    }

    public class EmployerLevyData
    {
        public string EmpRef { get; set; }

        public DasDeclarations Declarations { get; set; }

        public DasEnglishFractions Fractions { get; set; }
    }
}
