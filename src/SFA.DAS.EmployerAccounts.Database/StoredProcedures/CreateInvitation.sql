CREATE PROCEDURE [employer_account].[CreateInvitation]
	@accountId int = 0,
	@name nvarchar(100),
	@email nvarchar(255),
	@expiryDate datetime,
	@statusId tinyint,
	@role tinyint,
	@invitationId bigint output
AS


INSERT INTO [employer_account].[Invitation] 
	([AccountId],[Name],[Email],[ExpiryDate],[Status],[Role]) 
VALUES 
	(@accountId, @name, @email, @expiryDate, @statusId, @role)


	SELECT @invitationId = SCOPE_IDENTITY();