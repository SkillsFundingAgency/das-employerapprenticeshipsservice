CREATE PROCEDURE [employer_account].[CreateUserAccount]
(
	@userId BIGINT,
	@employerName NVARCHAR(100), 
	@apprenticeshipEmployerType TINYINT,
	@accountId BIGINT OUTPUT,
	@addedDate DATETIME
)
AS
BEGIN
	SET NOCOUNT ON;

	INSERT INTO [employer_account].[Account](Name, CreatedDate, ApprenticeshipEmployerType) VALUES (@employerName, @addedDate, @apprenticeshipEmployerType);
	SELECT @accountId = SCOPE_IDENTITY();

	INSERT INTO [employer_account].[Membership](UserId, AccountId, [Role]) VALUES (@userId, @accountId, 1);

	INSERT INTO [employer_account].[UserAccountSettings] (UserId, AccountId, ReceiveNotifications) VALUES (@userId, @accountId, 1)
END