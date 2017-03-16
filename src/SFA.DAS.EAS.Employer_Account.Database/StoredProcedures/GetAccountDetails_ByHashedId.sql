CREATE PROCEDURE employer_account.GetAccountDetails_ByHashedId
	@HashedId VARCHAR(100)
AS
Select 
	acc.Id AS AccountId,
	acc.HashedId,
	acc.Name,
	acc.CreatedDate,
	t.Email as OwnerEmail,
	ach.PayeRef as PayeSchemeId,
	ea.LegalEntityId
from employer_account.account acc
inner join employer_account.AccountEmployerAgreement aea on aea.AccountId = acc.Id
inner join employer_account.EmployerAgreement ea on ea.Id = aea.EmployerAgreementId
inner join employer_account.AccountHistory ach on ach.AccountId = acc.Id
OUTER APPLY
(
	SELECT TOP 1 u.email
	FROM employer_account.Membership m
	inner join employer_account.[User] u on u.Id = m.UserId
	where m.RoleId = 1 and m.AccountId = acc.Id
	ORDER BY m.CreatedDate desc
) t
WHERE acc.HashedId = @HashedId