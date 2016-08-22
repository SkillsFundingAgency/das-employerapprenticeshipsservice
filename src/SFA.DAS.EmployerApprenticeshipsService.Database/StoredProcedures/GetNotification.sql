CREATE PROCEDURE [account].[GetNotification]
	@Id BIGINT
AS
	SELECT * FROM [account].[Notification]
	WHERE id = @Id
