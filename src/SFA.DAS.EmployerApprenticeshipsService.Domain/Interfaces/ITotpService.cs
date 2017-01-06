using System.Security.Cryptography.X509Certificates;

namespace SFA.DAS.EAS.Domain.Interfaces
{
    public interface ITotpService
    {
        string GetCode(string key, string timeValue = "");
    }
}