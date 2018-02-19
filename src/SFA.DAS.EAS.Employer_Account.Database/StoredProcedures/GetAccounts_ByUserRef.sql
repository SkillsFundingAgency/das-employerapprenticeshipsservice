CREATE PROCEDURE [employer_account].[GetAccounts_ByUserRef]
	@userRef UNIQUEIDENTIFIER
AS
SELECT a.Id, a.Name, m.RoleId, a.HashedId, a.PublicHashedId
FROM [employer_account].[User] u 
INNER JOIN [employer_account].[Membership] m ON m.UserId = u.Id
INNER JOIN [employer_account].[Account]  a ON m.AccountId = a.Id
WHERE u.UserRef = @userRef