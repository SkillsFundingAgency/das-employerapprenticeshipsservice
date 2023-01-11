using SFA.DAS.EmployerAccounts.Models.Account;

namespace SFA.DAS.EmployerAccounts.Commands.OrganisationAndPayeRefData;

public sealed  class CookieBasedOrganisationAndPayeDataSaver : AsyncRequestHandler<SaveOrganisationAndPayeData>
{
    private const string CookieName = "sfa-das-employerapprenticeshipsservice-employeraccount";

    private readonly ICookieStorageService<EmployerAccountData> _cookieRepository;
    private const int CookieExpiryInDays = 365;

    public CookieBasedOrganisationAndPayeDataSaver(ICookieStorageService<EmployerAccountData> cookieRepository)
    {
        _cookieRepository = cookieRepository ?? throw new ArgumentNullException(nameof(cookieRepository));
    }

    protected override Task HandleCore(SaveOrganisationAndPayeData message)
    {
        var existingCookie = _cookieRepository.Get(CookieName);

        if (existingCookie == null)
        {
            createNewCookieWithData(message.OrganisationData, message.PayeRefData);
        }
        else
        {
            updateExistingCookieWithNewData(existingCookie, message.OrganisationData, message.PayeRefData);
        }

        return Task.CompletedTask;
    }

    private void updateExistingCookieWithNewData(EmployerAccountData existingCookie,
        EmployerAccountOrganisationData organisationData, EmployerAccountPayeRefData payeRefData)
    {
        existingCookie.EmployerAccountOrganisationData = organisationData;
        existingCookie.EmployerAccountPayeRefData = payeRefData;

        _cookieRepository
            .Update(
                CookieName,
                existingCookie);
    }

    private void createNewCookieWithData(EmployerAccountOrganisationData organisationData,
        EmployerAccountPayeRefData payeRefData)
    {

        _cookieRepository
            .Create(
                new EmployerAccountData
                {
                    EmployerAccountOrganisationData = organisationData,
                    EmployerAccountPayeRefData = payeRefData
                },
                CookieName,
                CookieExpiryInDays);
    }
}