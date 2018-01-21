CREATE PROCEDURE [employer_account].[CreateTransferConnection]
	@senderUserId BIGINT,
	@senderAccountId BIGINT,
	@receiverAccountId BIGINT,
	@status TINYINT,
	@createdDate DATETIME,
	@transferConnectionId BIGINT OUTPUT
AS
BEGIN
	SET NOCOUNT ON

	INSERT INTO employer_account.TransferConnection 
		(SenderUserId, SenderAccountId, ReceiverAccountId, Status, CreatedDate) 
	VALUES 
		(@senderUserId, @senderAccountId, @receiverAccountId, @status, @createdDate)

	SELECT @transferConnectionId = SCOPE_IDENTITY();
END
GO