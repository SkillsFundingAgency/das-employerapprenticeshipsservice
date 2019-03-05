CREATE VIEW [employer_account].[GetTeamMembers]
	AS 

select 1 AS IsUser,
	a.Id as 'AccountId', 
	a.HashedId,
	u.Id ,CONCAT(u.FirstName, ' ', u.LastName) as Name,  
	u.Email,
	CONVERT(varchar(64), u.UserRef) as 'UserRef', 
	m.[Role] as 'Role', 
	2 as 'Status',
	NULL AS ExpiryDate
from [employer_account].[User] u
                            left join [employer_account].[Membership] m on m.UserId = u.Id
                            left join [employer_account].[Account] a on a.Id = m.AccountId

Union all
SELECT 
	0,
	i.AccountId,
	a.HashedId,
	i.Id,
	i.Name,
	i.Email,
	NULL,
	i.[Role],
	CASE WHEN i.Status = 1 AND i.ExpiryDate < GETDATE() THEN 3 ELSE i.Status END AS Status,
	i.ExpiryDate
FROM [employer_account].[Invitation] i
	JOIN [employer_account].[Account] a
		ON a.Id = i.AccountId
WHERE i.Status NOT IN (2, 4)
