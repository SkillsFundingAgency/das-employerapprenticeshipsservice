Create Procedure [account].[GetNumberOfInvitations_ByUserId]
	@id uniqueidentifier
AS
SELECT count(1)
  FROM [account].[Invitation] i
  inner join [account].[User] u on u.Email = i.Email
  Where ExpiryDate > GETDATE()
  And i.Status = 1
  and u.PireanKey = @id