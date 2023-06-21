namespace SFA.DAS.EAS.Support.Infrastructure.Services.Contracts;

public interface IPayRefHashingService
{
    string HashValue(string id);
    string DecodeValueToString(string id);
}
