CREATE VIEW [dbo].[GetTeamMembers]
	AS 
	
select a.Id as 'AccountId', u.Id ,'' as Name,  u.Email,CONVERT(varchar(64), u.PireanKey) as 'UserRef', r.Name as 'Role', 2 as 'Status' from [User] u
                            left join [Membership] m on m.UserId = u.Id
                            left join [Role] r on r.Id = m.RoleId
                            left join [Account] a on a.Id = m.AccountId

Union all
SELECT 
	i.AccountId,
	i.Id,
	i.Name,
	i.Email,
	'' as 'UserRef',
	r.Name AS 'Role',
	i.Status
FROM [dbo].[Invitation] i
	JOIN [dbo].[Account] a
		ON a.Id = i.AccountId
	JOIN [dbo].[Role] r
		ON r.Id = i.RoleId
