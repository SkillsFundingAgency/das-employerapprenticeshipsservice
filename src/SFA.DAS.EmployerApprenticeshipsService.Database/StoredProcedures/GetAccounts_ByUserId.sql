CREATE PROCEDURE [account].[GetAccounts_ByUserId]
	@userId UNIQUEIDENTIFIER
	
AS
select 
	a.Id, a.Name, m.RoleId 
from 
	[account].[User] u 
inner join 
	[account].[Membership] m on m.UserId = u.Id
inner join
	[account].[Account]  a on m.AccountId = a.Id
where 
u.PireanKey = @userId
