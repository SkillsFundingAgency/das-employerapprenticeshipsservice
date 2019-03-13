CREATE VIEW [employer_account].[MembershipView]
AS
SELECT        
m.AccountId, 
m.UserId, 
m.Role, 
m.CreatedDate, 
m.ShowWizard, 
CONVERT(varchar(64), 
u.UserRef) AS UserRef, 
u.Email, 
u.FirstName, 
u.LastName, 
a.Name AS AccountName, 
a.HashedId AS HashedAccountId
FROM employer_account.Membership AS m 
INNER JOIN employer_account.[User] AS u 
ON u.Id = m.UserId 
INNER JOIN employer_account.Account AS a 
ON a.Id = m.AccountId