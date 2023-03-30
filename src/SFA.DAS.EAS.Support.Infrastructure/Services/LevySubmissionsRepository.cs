using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using HMRC.ESFA.Levy.Api.Client;
using HMRC.ESFA.Levy.Api.Types;
using HMRC.ESFA.Levy.Api.Types.Exceptions;
using Microsoft.Extensions.Logging;
using SFA.DAS.NLog.Logger;

namespace SFA.DAS.EAS.Support.Infrastructure.Services
{
    public sealed class LevySubmissionsRepository : ILevySubmissionsRepository
    {
        private IApprenticeshipLevyApiClient _levyApiClient;
        private readonly ILogger<LevySubmissionsRepository> _logger;
        private readonly ILevyTokenHttpClientFactory _levyTokenHttpClientFactory;

        public LevySubmissionsRepository(ILogger<LevySubmissionsRepository> logger, ILevyTokenHttpClientFactory levyTokenHttpClientFactory)
        {
            _logger = logger;
            _levyTokenHttpClientFactory = levyTokenHttpClientFactory;
        }


        public async Task<LevyDeclarations> Get(string payeScheme)
        {
            _logger.LogDebug($"IApprenticeshipLevyApiClient.GetEmployerLevyDeclarations(\"{payeScheme}\");");

            try
            {

                _levyApiClient = await _levyTokenHttpClientFactory.GetLevyHttpClient();

                var response = await _levyApiClient.GetEmployerLevyDeclarations(payeScheme);

                if (response != null)
                {
                    var filteredDeclarations = GetFilteredDeclarations(response.Declarations);
                    response.Declarations = filteredDeclarations.OrderByDescending(x => x.SubmissionTime).ThenByDescending(x => x.Id).ToList();
                }

                return response;
            }
            catch (ApiHttpException ex)
            {
                var properties = new Dictionary<string, object>
                {
                    {"RequestCtx.StatusCode", ex.HttpCode}
                };
                _logger.LogError(ex, "Issue retrieving levy declarations", properties);
                throw;
            }
        }

        private List<Declaration> GetFilteredDeclarations(List<Declaration> resultDeclarations)
        {
            return resultDeclarations?.Where(x => x.SubmissionTime >= new DateTime(2017, 4, 1)).ToList();
        }
    }
}
