CREATE PROCEDURE [employer_account].[ValidateAccountMembership]
	@accountHashedId NVARCHAR(100),
	@userExternalId UNIQUEIDENTIFIER
AS
BEGIN
	DECLARE @true BIT = 1
	DECLARE @false BIT = 0

	SELECT 
		CASE
			WHEN EXISTS (
				SELECT 1
				FROM [employer_account].[Membership] m
				INNER JOIN [employer_account].[Account] a ON a.Id = m.AccountId
				INNER JOIN [employer_account].[User] u ON u.Id = m.UserId
				WHERE a.HashedId = @accountHashedId
				AND u.UserRef = @userExternalId
			)
			THEN @true
			ELSE @false
		END
END