CREATE PROCEDURE [dbo].[GetNotification]
	@Id BIGINT
AS
	SELECT * FROM [dbo].[Notification]
	WHERE id = @Id
