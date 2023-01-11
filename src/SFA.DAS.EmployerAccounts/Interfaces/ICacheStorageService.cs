namespace SFA.DAS.EmployerAccounts.Interfaces;

public interface ICacheStorageService
{
    Task Save<T>(string key, T item, int expirationInMinutes);
    Task Delete(string key);
    bool TryGet(string key, out string value);
}