using SFA.DAS.EAS.Domain.Models.HmrcLevy;

namespace SFA.DAS.EAS.Application.Queries.GetHMRCLevyDeclaration
{
    public class GetHMRCLevyDeclarationResponse
    {
        public LevyDeclarations LevyDeclarations { get; set; }
        public string Empref { get; set; }
    }
}