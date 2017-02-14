using System;
using RestSharp;

namespace SFA.DAS.EAS.Infrastructure.Interfaces.REST
{
    public interface IRestClientFactory
    {
        IRestClient Create(Uri baseUrl);
    }
}
