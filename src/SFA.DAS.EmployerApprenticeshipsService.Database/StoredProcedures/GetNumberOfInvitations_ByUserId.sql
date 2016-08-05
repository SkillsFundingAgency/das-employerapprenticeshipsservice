Create Procedure GetNumberOfInvitations_ByUserId
	@id uniqueidentifier
AS
SELECT count(1)
  FROM [dbo].[Invitation] i
  inner join [dbo].[User] u on u.Email = i.Email
  Where ExpiryDate > GETDATE()
  And i.Status = 1
  and u.PireanKey = @id