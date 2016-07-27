CREATE PROCEDURE [dbo].[GetNotification]
	@Id int
AS
	SELECT * FROM [dbo].[Notification]
	WHERE id = @Id
