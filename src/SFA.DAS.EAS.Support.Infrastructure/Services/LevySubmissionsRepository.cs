using HMRC.ESFA.Levy.Api.Types;
using HMRC.ESFA.Levy.Api.Types.Exceptions;
using SFA.DAS.EAS.Support.Infrastructure.Services.Contracts;

namespace SFA.DAS.EAS.Support.Infrastructure.Services;

public sealed class LevySubmissionsRepository : ILevySubmissionsRepository
{
    private readonly ILogger<LevySubmissionsRepository> _logger;
    private readonly ILevyTokenHttpClientFactory _levyTokenHttpClientFactory;

    public LevySubmissionsRepository(ILogger<LevySubmissionsRepository> logger, ILevyTokenHttpClientFactory levyTokenHttpClientFactory)
    {
        _logger = logger;
        _levyTokenHttpClientFactory = levyTokenHttpClientFactory;
    }


    public async Task<LevyDeclarations> Get(string payeScheme)
    {
        _logger.LogDebug("IApprenticeshipLevyApiClient.GetEmployerLevyDeclarations(\"{PayeScheme}\");", payeScheme);

        try
        {
            var levyApiClient = await _levyTokenHttpClientFactory.GetLevyHttpClient();

            var response = await levyApiClient.GetEmployerLevyDeclarations(payeScheme);

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

    private static List<Declaration> GetFilteredDeclarations(List<Declaration> resultDeclarations)
    {
        return resultDeclarations?.Where(x => x.SubmissionTime >= new DateTime(2017, 4, 1)).ToList();
    }
}
