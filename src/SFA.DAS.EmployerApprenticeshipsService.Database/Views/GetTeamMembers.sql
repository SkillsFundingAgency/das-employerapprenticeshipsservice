CREATE VIEW [dbo].[GetTeamMembers]
	AS 
	
select 1 AS IsUser,
	a.Id as 'AccountId', 
	u.Id ,'' as Name,  
	u.Email,
	CONVERT(varchar(64), u.PireanKey) as 'UserRef', 
	m.RoleId as 'Role', 
	2 as 'Status',
	NULL AS ExpiryDate
from [User] u
                            left join [Membership] m on m.UserId = u.Id
                            left join [Account] a on a.Id = m.AccountId

Union all
SELECT 
	0,
	i.AccountId,
	i.Id,
	i.Name,
	i.Email,
	NULL,
	i.RoleId,
	i.Status,
	i.ExpiryDate
FROM [dbo].[Invitation] i
	JOIN [dbo].[Account] a
		ON a.Id = i.AccountId
WHERE i.Status NOT IN (2, 4)
