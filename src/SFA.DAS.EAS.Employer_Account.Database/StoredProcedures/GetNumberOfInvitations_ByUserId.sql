Create Procedure [employer_account].[GetNumberOfInvitations_ByUserId]
	@id uniqueidentifier
AS
SELECT count(1)
  FROM [employer_account].[Invitation] i
  inner join [employer_account].[User] u on u.Email = i.Email
  Where ExpiryDate > GETDATE()
  And i.Status = 1
  and u.PireanKey = @id