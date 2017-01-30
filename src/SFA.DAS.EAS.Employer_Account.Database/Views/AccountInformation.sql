CREATE VIEW [employer_account].[AccountInformation]
AS 
Select 
	acc.HashedId as DasAccountId,
	acc.Name as DasAccountName,
	acc.CreatedDate as DateRegistered,
	le.Id as OrganisationId,
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
	as OrganisationSource,
	py.Name as PayeSchemeName,
	ach.RemovedDate as AccountRemovedDate,
	acc.CreatedDate as AccountCreatedDate,
	acc.Id as AccountId
from employer_account.account acc
inner join employer_account.AccountEmployerAgreement aea on aea.AccountId = acc.Id
inner join employer_account.EmployerAgreement ea on ea.Id = aea.EmployerAgreementId
inner join employer_account.LegalEntity le on le.Id = ea.LegalEntityId
inner join employer_account.AccountHistory ach on ach.AccountId = acc.Id
inner join employer_account.Paye py on py.Ref = ach.PayeRef
OUTER APPLY
(
	SELECT TOP 1 u.email
	FROM employer_account.Membership m
	inner join employer_account.[User] u on u.Id = m.UserId
	where m.RoleId = 1 and m.AccountId = acc.Id
	ORDER BY m.CreatedDate desc
) t
