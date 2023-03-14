using System.Threading;
using SFA.DAS.EmployerAccounts.Models.Account;

namespace SFA.DAS.EmployerAccounts.Commands.OrganisationAndPayeRefData;

public sealed class CookieBasedOrganisationAndPayeDataSaver : IRequestHandler<SaveOrganisationAndPayeData>
{
    private const string CookieName = "sfa-das-employerapprenticeshipsservice-employeraccount";

    private readonly ICookieStorageService<EmployerAccountData> _cookieRepository;
    private const int CookieExpiryInDays = 365;

    public CookieBasedOrganisationAndPayeDataSaver(ICookieStorageService<EmployerAccountData> cookieRepository)
    {
        _cookieRepository = cookieRepository ?? throw new ArgumentNullException(nameof(cookieRepository));
    }

    public Task<Unit> Handle(SaveOrganisationAndPayeData message, CancellationToken cancellationToken)
    {
        var existingCookie = _cookieRepository.Get(CookieName);

        if (existingCookie == null)
        {
            CreateNewCookieWithData(message.OrganisationData, message.PayeRefData);
        }
        else
        {
            UpdateExistingCookieWithNewData(existingCookie, message.OrganisationData, message.PayeRefData);
        }

        return Task.FromResult(Unit.Value);
    }

    private void UpdateExistingCookieWithNewData(EmployerAccountData existingCookie, EmployerAccountOrganisationData organisationData, EmployerAccountPayeRefData payeRefData)
    {
        existingCookie.EmployerAccountOrganisationData = organisationData;
        existingCookie.EmployerAccountPayeRefData = payeRefData;

        _cookieRepository.Update(CookieName, existingCookie);
    }

    private void CreateNewCookieWithData(EmployerAccountOrganisationData organisationData, EmployerAccountPayeRefData payeRefData)
    {
        var employerAccountData = new EmployerAccountData
        {
            EmployerAccountOrganisationData = organisationData,
            EmployerAccountPayeRefData = payeRefData
        };

        _cookieRepository.Create(employerAccountData, CookieName, CookieExpiryInDays);
    }
}