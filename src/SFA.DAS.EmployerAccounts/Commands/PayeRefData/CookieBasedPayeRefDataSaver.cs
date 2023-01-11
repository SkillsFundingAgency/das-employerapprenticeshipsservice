using SFA.DAS.EmployerAccounts.Models.Account;

namespace SFA.DAS.EmployerAccounts.Commands.PayeRefData;

public sealed  class CookieBasedPayeRefDataSaver : AsyncRequestHandler<SavePayeRefData>
{
    private const string CookieName = "sfa-das-employerapprenticeshipsservice-employeraccount";

    private readonly ICookieStorageService<EmployerAccountData> _cookieRepository;
    private const int CookieExpiryInDays = 365;

    public CookieBasedPayeRefDataSaver(ICookieStorageService<EmployerAccountData> cookieRepository)
    {
        _cookieRepository = cookieRepository ?? throw new ArgumentNullException(nameof(cookieRepository));
    }

    protected override Task HandleCore(SavePayeRefData message)
    {
        var existingCookie = _cookieRepository.Get(CookieName);

        if (existingCookie == null)
        {
            createNewCookieWithData(message.PayeRefData);
        }
        else
        {
            updateExistingCookieWithNewData(existingCookie, message.PayeRefData);
        }

        return Task.CompletedTask;
    }

    private void updateExistingCookieWithNewData(EmployerAccountData existingCookie, EmployerAccountPayeRefData payeRefData)
    {
        existingCookie.EmployerAccountPayeRefData = payeRefData;

        _cookieRepository
            .Update(
                CookieName,
                existingCookie);
    }

    private void createNewCookieWithData(EmployerAccountPayeRefData payeRefData)
    {

        _cookieRepository
            .Create(
                new EmployerAccountData
                {
                    EmployerAccountOrganisationData = new EmployerAccountOrganisationData(),
                    EmployerAccountPayeRefData = payeRefData
                },
                CookieName,
                CookieExpiryInDays);
    }
}