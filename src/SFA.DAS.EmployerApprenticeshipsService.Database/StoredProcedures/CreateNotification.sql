

CREATE PROCEDURE [account].[CreateNotification]
	@UserId BIGINT, 
	@DateTime DATETIME ,
	@ForceFormat BIT = 0,
	@TemplateId VARCHAR(20),
	@Data NVARCHAR(MAX),
	@MessageFormat TINYINT,
	@Id BIGINT OUTPUT
as
BEGIN
	INSERT INTO [account].[Notification]
		(UserId, DateTime, ForecFormat, TemplateId,Data,MessageFormat)
	VALUES
		(@UserId,@DateTime, @ForceFormat, @TemplateId, @Data, @MessageFormat)

	SELECT @Id = SCOPE_IDENTITY()
END