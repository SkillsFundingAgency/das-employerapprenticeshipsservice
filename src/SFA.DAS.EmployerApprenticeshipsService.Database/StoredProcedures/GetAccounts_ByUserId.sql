CREATE PROCEDURE [dbo].[GetAccounts_ByUserId]
	@id UNIQUEIDENTIFIER
	
AS
select 
	a.Name, m.RoleId 
from 
	[dbo].[User] u 
inner join 
	[dbo].[Membership] m on m.UserId = u.Id
inner join
	[dbo].[Account]  a on m.AccountId = a.Id
where 
u.PireanKey = @id
