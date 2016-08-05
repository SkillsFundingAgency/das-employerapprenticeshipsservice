

CREATE PROCEDURE [dbo].[CreateNotification]
	@UserId BIGINT, 
	@DateTime DATETIME ,
	@ForceFormat BIT = 0,
	@TemplateId VARCHAR(20),
	@Data NVARCHAR(MAX),
	@MessageFormat TINYINT,
	@Id BIGINT OUTPUT
as
BEGIN
	INSERT INTO [dbo].[Notification]
		(UserId, DateTime, ForecFormat, TemplateId,Data,MessageFormat)
	VALUES
		(@UserId,@DateTime, @ForceFormat, @TemplateId, @Data, @MessageFormat)

	SELECT @Id = SCOPE_IDENTITY()
END