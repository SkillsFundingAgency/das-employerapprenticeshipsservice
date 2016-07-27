

CREATE PROCEDURE [dbo].[CreateNotification]
	@UserId INT, 
	@DateTime DATETIME ,
	@ForceFormat BIT = 0,
	@TemplateId VARCHAR(20),
	@Data NVARCHAR(MAX),
	@MessageFormat TINYINT,
	@Id INT OUTPUT
as
BEGIN
	INSERT INTO [dbo].[Notification]
		(UserId, DateTime, ForecFormat, TemplateId,Data,MessageFormat)
	VALUES
		(@UserId,@DateTime, @ForceFormat, @TemplateId, @Data, @MessageFormat)

	SELECT @Id = SCOPE_IDENTITY()
END