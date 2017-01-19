CREATE PROCEDURE [employer_account].[GetNotification]
	@Id BIGINT
AS
	SELECT * FROM [employer_account].[Notification]
	WHERE id = @Id
