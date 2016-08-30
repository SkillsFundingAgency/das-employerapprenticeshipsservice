using SFA.DAS.EmployerApprenticeshipsService.Application.Queries.GetHMRCLevyDeclaration;

namespace SFA.DAS.EmployerApprenticeshipsService.TestCommon.ObjectMothers
{
    public static class GetHMRCLevyDeclarationResponseObjectMother
    {
        public static GetHMRCLevyDeclarationResponse Create(string empref="123avc")
        {

            var declarationResponse = new GetHMRCLevyDeclarationResponse
            {
                Empref = empref,
                Fractions = EnglishFractionObjectMother.Create(empref),
                LevyDeclarations = DeclarationsObjectMother.Create(empref)
            };


            return declarationResponse;
        }
    }
}