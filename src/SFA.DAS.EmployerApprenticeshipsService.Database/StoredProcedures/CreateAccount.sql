

CREATE PROCEDURE [dbo].[CreateAccount]
(
	@userRef UNIQUEIDENTIFIER,
	@employerNumber NVARCHAR(50), 
	@employerName NVARCHAR(100), 
	@employerRef NVARCHAR(10),
	@accountId int OUTPUT
)
AS
BEGIN
	SET NOCOUNT ON;

	DECLARE @userId INT;

	INSERT INTO [dbo].[User](PireanKey, Email) VALUES (@userRef, 'test@test.org');
	SELECT @userId = SCOPE_IDENTITY();

	INSERT INTO [dbo].[Account](Name) VALUES (@employerName);
	SELECT @accountId = SCOPE_IDENTITY();

	INSERT INTO [dbo].[Paye](Ref, AccountId) VALUES (@employerRef, @accountId);
	INSERT INTO [dbo].[Membership](UserId, AccountId, RoleId) VALUES (@userId, @accountId, 1);
END