CREATE VIEW [dbo].[GetInvitations]
AS 
SELECT i.Id,
	a.Name AS AccountName,
	i.Name,
	i.Email,
	i.ExpiryDate,
	i.Status,
	u.Id AS InternalUserId,
	u.PireanKey AS ExternalUserId 
FROM [dbo].[Invitation] i
	JOIN [dbo].[Account] a
		ON a.Id = i.AccountId
	LEFT OUTER JOIN [dbo].[User] u
		ON u.Email = i.Email