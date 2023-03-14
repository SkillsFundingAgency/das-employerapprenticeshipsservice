using System.Threading;
using SFA.DAS.EmployerAccounts.Models.Account;

namespace SFA.DAS.EmployerAccounts.Commands.OrganisationData;

public sealed  class CookieBasedOrganisationDataSaver : IRequestHandler<SaveOrganisationData>
{
    private const string CookieName = "sfa-das-employerapprenticeshipsservice-employeraccount";

    private readonly ICookieStorageService<EmployerAccountData> _cookieRepository;
    private const int CookieExpiryInDays = 365;

    public CookieBasedOrganisationDataSaver(ICookieStorageService<EmployerAccountData> cookieRepository)
    {
        _cookieRepository = cookieRepository ?? throw new ArgumentNullException(nameof(cookieRepository));
    }

    public Task<Unit> Handle(SaveOrganisationData message, CancellationToken cancellationToken)
    {
        var existingCookie = _cookieRepository.Get(CookieName);

        if (existingCookie == null)
        {
            createNewCookieWithData(message.OrganisationData);
        }
        else
        {
            updateExistingCookieWithNewData(existingCookie, message.OrganisationData);
        }

        return Task.FromResult(Unit.Value);
    }

    private void updateExistingCookieWithNewData(EmployerAccountData existingCookie, EmployerAccountOrganisationData organisationData)
    {
        existingCookie.EmployerAccountOrganisationData = organisationData;

        _cookieRepository
            .Update(
                CookieName,
                existingCookie);
    }

    private void createNewCookieWithData(EmployerAccountOrganisationData organisationData)
    {

        _cookieRepository
            .Create(
                new EmployerAccountData
                {
                    EmployerAccountOrganisationData = organisationData,
                    EmployerAccountPayeRefData = new EmployerAccountPayeRefData()
                },
                CookieName,
                CookieExpiryInDays);
    }
}