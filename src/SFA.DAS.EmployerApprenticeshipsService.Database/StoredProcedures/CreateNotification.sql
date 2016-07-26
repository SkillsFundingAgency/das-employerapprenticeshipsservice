

CREATE PROCEDURE [dbo].[CreateNotification]
	@UserId INT, 
	@DateTime DATETIME ,
	@ForecFormat BIT = 0,
	@TemplateId VARCHAR(20),
	@Data NVARCHAR(MAX),
	@MessageFormat TINYINT
as
BEGIN
	INSERT INTO [dbo].[Notification]
		(UserId, DateTime, ForecFormat, TemplateId,Data,MessageFormat)
	VALUES
		(@UserId,@DateTime, @ForecFormat, @TemplateId, @Data, @MessageFormat)

	SELECT Id = SCOPE_IDENTITY()
END