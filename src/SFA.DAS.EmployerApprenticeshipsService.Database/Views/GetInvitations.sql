CREATE VIEW [dbo].[GetInvitations]
AS 
SELECT i.Id,
	i.AccountId,
	a.Name AS AccountName,
	i.Name,
	i.Email,
	i.ExpiryDate,
	i.Status,
	i.RoleId,
	r.Name AS RoleName,
	u.Id AS InternalUserId,
	u.PireanKey AS ExternalUserId	 
FROM [dbo].[Invitation] i
	JOIN [dbo].[Account] a
		ON a.Id = i.AccountId
	JOIN [dbo].[Role] r
		ON r.Id = i.RoleId
	LEFT OUTER JOIN [dbo].[User] u
		ON u.Email = i.Email