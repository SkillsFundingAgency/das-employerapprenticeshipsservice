using System.IO;

namespace SFA.DAS.EmployerAccounts.Interfaces;

public interface IPdfService
{
    Task<MemoryStream> SubstituteValuesForPdf(string fileName, Dictionary<string, string> valuesToSubstitute);

    Task<MemoryStream> SubstituteValuesForPdf(string fileName);
}