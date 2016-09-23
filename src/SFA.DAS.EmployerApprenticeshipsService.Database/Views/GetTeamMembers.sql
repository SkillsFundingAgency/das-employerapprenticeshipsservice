CREATE VIEW [account].[GetTeamMembers]
	AS 

select 1 AS IsUser,
	a.Id as 'AccountId', 
	a.HashedId,
	u.Id ,CONCAT(u.FirstName, ' ', u.LastName) as Name,  
	u.Email,
	CONVERT(varchar(64), u.PireanKey) as 'UserRef', 
	m.RoleId as 'Role', 
	2 as 'Status',
	NULL AS ExpiryDate
from [account].[User] u
                            left join [account].[Membership] m on m.UserId = u.Id
                            left join [account].[Account] a on a.Id = m.AccountId

Union all
SELECT 
	0,
	i.AccountId,
	a.HashedId,
	i.Id,
	i.Name,
	i.Email,
	NULL,
	i.RoleId,
	CASE WHEN i.Status = 1 AND i.ExpiryDate < GETDATE() THEN 3 ELSE i.Status END AS Status,
	i.ExpiryDate
FROM [account].[Invitation] i
	JOIN [account].[Account] a
		ON a.Id = i.AccountId
WHERE i.Status NOT IN (2, 4)
