

CREATE PROCEDURE account.GetAccountInformation_ByDateRange
	@fromDate DATETIME,
	@toDate DATETIME,
	@offset int,
	@pageSize int
AS


Select 
	acc.HashedId as DasAccountId,
	acc.Name as DasAccountName,
	acc.CreatedDate as DateRegistered,
	le.Name as OrganisationName,
	le.RegisteredAddress as OrganisationRegisteredAddress,
	le.[Status] as OrganisationStatus,
	le.DateOfIncorporation as OrgansiationCreatedDate,
	le.Code as OrganisationNumber,
	t.Email as OwnerEmail,
	CASE le.Source 
		when 1 then 'Companies House' 
		when 2 then 'Charities' 
		when 3 then 'Public Bodies' 
		else 'Other' end 
	as OrganisationSource
from account.account acc
inner join account.AccountEmployerAgreement aea on aea.AccountId = acc.Id
inner join account.EmployerAgreement ea on ea.Id = aea.EmployerAgreementId
inner join account.LegalEntity le on le.Id = ea.LegalEntityId
OUTER APPLY
(
	SELECT TOP 1 u.email
	FROM account.Membership m
	inner join account.[User] u on u.Id = m.UserId
	where m.RoleId = 1 and m.AccountId = acc.Id
	ORDER BY m.CreatedDate desc
) t
WHERE acc.CreatedDate BETWEEN @fromDate and @toDate
Order by acc.id
OFFSET @offset ROWS FETCH NEXT @pageSize ROWS ONLY