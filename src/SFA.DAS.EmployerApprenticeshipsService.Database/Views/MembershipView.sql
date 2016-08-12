CREATE VIEW [dbo].[MembershipView]
AS
SELECT m.*, 
	CONVERT(varchar(64), u.PireanKey) AS UserRef, 
	u.Email,
	u.FirstName,
	u.LastName, 
	a.Name AS AccountName, 
	r.Name AS RoleName
FROM [dbo].[Membership] m
	JOIN [dbo].[User] u
		ON u.Id = m.UserId
	JOIN [dbo].[Account] a
		ON a.Id = m.AccountId
	JOIN [dbo].[Role] r
		ON r.Id = m.RoleId