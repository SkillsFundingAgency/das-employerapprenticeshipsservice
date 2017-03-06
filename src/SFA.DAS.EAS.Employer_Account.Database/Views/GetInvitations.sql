﻿CREATE VIEW [employer_account].[GetInvitations]
AS 
SELECT i.Id,
	i.AccountId,
	a.Name AS AccountName,
	i.Name,
	i.Email,
	i.ExpiryDate,
	CASE WHEN i.Status = 1 AND i.ExpiryDate < GETDATE() THEN 3 ELSE i.Status END AS Status,
	i.RoleId,
	u.Id AS InternalUserId,
	u.PireanKey AS ExternalUserId	 
FROM [employer_account].[Invitation] i
	JOIN [employer_account].[Account] a
		ON a.Id = i.AccountId
	LEFT OUTER JOIN [employer_account].[User] u
		ON u.Email = i.Email