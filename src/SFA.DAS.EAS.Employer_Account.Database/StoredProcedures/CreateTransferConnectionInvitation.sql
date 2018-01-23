CREATE PROCEDURE [employer_account].[CreateTransferConnectionInvitation]
	@senderUserId BIGINT,
	@senderAccountId BIGINT,
	@receiverAccountId BIGINT,
	@status TINYINT,
	@createdDate DATETIME,
	@transferConnectionInvitationId BIGINT OUTPUT
AS
BEGIN
	SET NOCOUNT ON

	INSERT INTO employer_account.TransferConnectionInvitation 
		(SenderUserId, SenderAccountId, ReceiverAccountId, Status, CreatedDate) 
	VALUES 
		(@senderUserId, @senderAccountId, @receiverAccountId, @status, @createdDate)

	SELECT @transferConnectionInvitationId = SCOPE_IDENTITY();
END
GO