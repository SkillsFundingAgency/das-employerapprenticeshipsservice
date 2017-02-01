using SFA.DAS.EAS.Application.Queries.GetHMRCLevyDeclaration;

namespace SFA.DAS.EAS.TestCommon.ObjectMothers
{
    public static class GetHMRCLevyDeclarationResponseObjectMother
    {
        public static GetHMRCLevyDeclarationResponse Create(string empref="123avc")
        {

            var declarationResponse = new GetHMRCLevyDeclarationResponse
            {
                Empref = empref,
                LevyDeclarations = DeclarationsObjectMother.Create(empref)
            };


            return declarationResponse;
        }
    }
}