using SFA.DAS.EmployerFinance.Queries.GetHMRCLevyDeclaration;

namespace SFA.DAS.EmployerFinance.UnitTests.ObjectMothers
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