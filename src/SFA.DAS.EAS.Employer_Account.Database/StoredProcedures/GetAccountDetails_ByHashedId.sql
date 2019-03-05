CREATE PROCEDURE employer_account.GetAccountDetails_ByHashedId
	@hashedAccountId NVARCHAR(100)
AS
	SET NOCOUNT ON

	SELECT
		a.Id AS AccountId,
		a.HashedId,
		a.PublicHashedId,
		a.Name,
		a.CreatedDate,
		u.Email AS OwnerEmail,
		ah.PayeRef AS PayeSchemeId,
		ale.LegalEntityId
	FROM employer_account.Account a
	INNER JOIN [employer_account].[AccountLegalEntity] ale ON ale.AccountId = a.Id
	INNER JOIN [employer_account].[EmployerAgreement] ea ON ea.AccountLegalEntityId = ale.Id
	INNER JOIN [employer_account].[AccountHistory] ah ON ah.AccountId = a.Id
	OUTER APPLY
	(
		SELECT TOP 1 u.Email
		FROM [employer_account].[Membership] m
		INNER JOIN [employer_account].[User] u ON u.Id = m.UserId
		WHERE m.[Role] = 1 and m.AccountId = a.Id
		ORDER BY m.CreatedDate DESC
	) u
	WHERE a.HashedId = @hashedAccountId AND
	ea.StatusId IN (1, 2) AND
	ale.Deleted IS NULL;