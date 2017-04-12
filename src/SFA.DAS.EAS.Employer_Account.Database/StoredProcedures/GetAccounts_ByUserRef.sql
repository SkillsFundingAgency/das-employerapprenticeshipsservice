CREATE PROCEDURE [employer_account].[GetAccounts_ByUserRef]
	@userRef UNIQUEIDENTIFIER
	
AS
select 
	a.Id, a.Name, m.RoleId ,a.HashedId
from 
	[employer_account].[User] u 
inner join 
	[employer_account].[Membership] m on m.UserId = u.Id
inner join
	[employer_account].[Account]  a on m.AccountId = a.Id
where 
u.UserRef = @userRef
