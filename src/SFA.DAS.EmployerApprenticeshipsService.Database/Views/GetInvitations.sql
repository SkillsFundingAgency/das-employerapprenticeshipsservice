CREATE VIEW [account].[GetInvitations]
AS 
SELECT i.Id,
	i.AccountId,
	a.Name AS AccountName,
	i.Name,
	i.Email,
	i.ExpiryDate,
	CASE WHEN i.Status = 1 AND i.ExpiryDate < GETDATE() THEN 3 ELSE i.Status END AS Status,
	i.RoleId,
	r.Name AS RoleName,
	u.Id AS InternalUserId,
	u.PireanKey AS ExternalUserId	 
FROM [account].[Invitation] i
	JOIN [account].[Account] a
		ON a.Id = i.AccountId
	JOIN [account].[Role] r
		ON r.Id = i.RoleId
	LEFT OUTER JOIN [account].[User] u
		ON u.Email = i.Email