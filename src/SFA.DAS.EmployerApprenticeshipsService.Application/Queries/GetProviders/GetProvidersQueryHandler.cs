﻿using System;
using System.Threading.Tasks;
using MediatR;
using SFA.DAS.EAS.Domain.Data;
using SFA.DAS.EAS.Domain.Data.Repositories;

namespace SFA.DAS.EAS.Application.Queries.GetProviders
{
    public class GetProvidersQueryHandler : IAsyncRequestHandler<GetProvidersQueryRequest, GetProvidersQueryResponse>
    {
        private readonly IProviderRepository _providerRepository;

        public GetProvidersQueryHandler(IProviderRepository providerRepository)
        {
            if (providerRepository == null)
                throw new ArgumentNullException(nameof(providerRepository));
            _providerRepository = providerRepository;
        }

        public async Task<GetProvidersQueryResponse> Handle(GetProvidersQueryRequest message)
        {
            var providers = await _providerRepository.GetAllProviders();

            return new GetProvidersQueryResponse
            {
                Providers = providers.Data
            };
        }
    }
}