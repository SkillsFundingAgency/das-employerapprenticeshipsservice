using RestSharp;

namespace SFA.DAS.EmployerAccounts.Interfaces;

public interface IRestClientFactory
{
    IRestClient Create(Uri baseUrl);
}