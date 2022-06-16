using System;
using System.Linq.Expressions;
using StructureMap;

namespace SFA.DAS.EmployerFinance.Api.Client
{
    public class EmployerFinanceApiClientRegistry : Registry
    {
        public EmployerFinanceApiClientRegistry(Expression<Func<IContext, EmployerFinanceApiClientConfiguration>> getApiConfig)
        {
            For<EmployerFinanceApiClientConfiguration>().Use(getApiConfig);
            For<IEmployerFinanceApiClientConfiguration>().Use<EmployerFinanceApiClientConfiguration>();
            For<IEmployerFinanceApiClient>().Use<EmployerFinanceApiClient>();
        }
    }
}