IF EXISTS (
	SELECT 1
	FROM INFORMATION_SCHEMA.TABLES
	WHERE TABLE_SCHEMA = 'employer_account'
	AND TABLE_NAME = 'TransferConnectionInvitationV1'
)
BEGIN
	DECLARE @transferConnectionInvitationV1Id BIGINT = (SELECT MIN(Id) FROM [employer_account].[TransferConnectionInvitationV1])
	DECLARE @senderUserId BIGINT
	DECLARE @senderAccountId BIGINT
	DECLARE @receiverAccountId BIGINT
	DECLARE @status INT
	DECLARE @approverUserId BIGINT
	DECLARE @rejectorUserId BIGINT
	DECLARE @createdDate DATETIME
	DECLARE @modifiedDate DATETIME
	DECLARE @transferConnectionInvitationId BIGINT

	WHILE @transferConnectionInvitationV1Id IS NOT NULL
	BEGIN
		SELECT
			@senderUserId = SenderUserId,
			@senderAccountId = SenderAccountId,
			@receiverAccountId = ReceiverAccountId,
			@status = Status,
			@approverUserId = ApproverUserId,
			@rejectorUserId = RejectorUserId,
			@createdDate = CreatedDate,
			@modifiedDate = ModifiedDate
		FROM [employer_account].[TransferConnectionInvitationV1]
		WHERE Id = @transferConnectionInvitationV1Id

		INSERT INTO [employer_account].[TransferConnectionInvitation] (SenderAccountId, ReceiverAccountId, Status, DeletedBySender, DeletedByReceiver, CreatedDate)
		VALUES (@senderAccountId, @receiverAccountId, @status, 0, 0, @createdDate)

		SET @transferConnectionInvitationId = SCOPE_IDENTITY()

		INSERT INTO [employer_account].[TransferConnectionInvitationChange] (TransferConnectionInvitationId, SenderAccountId, ReceiverAccountId, Status, DeletedBySender, DeletedByReceiver, UserId, CreatedDate)
		VALUES (@transferConnectionInvitationId, @senderAccountId, @receiverAccountId, 1, 0, 0, @senderUserId, @createdDate)

		IF (@approverUserId IS NOT NULL)
		BEGIN
			INSERT INTO [employer_account].[TransferConnectionInvitationChange] (TransferConnectionInvitationId, Status, UserId, CreatedDate)
			VALUES (@transferConnectionInvitationId, 2, @approverUserId, @modifiedDate)
		END

		IF (@rejectorUserId IS NOT NULL)
		BEGIN
			INSERT INTO [employer_account].[TransferConnectionInvitationChange] (TransferConnectionInvitationId, Status, UserId, CreatedDate)
			VALUES (@transferConnectionInvitationId, 3, @rejectorUserId, @modifiedDate)
		END

		SET @transferConnectionInvitationV1Id = (SELECT MIN(Id) FROM [employer_account].[TransferConnectionInvitationV1] WHERE Id > @transferConnectionInvitationV1Id)
	END

	DROP TABLE [employer_account].[TransferConnectionInvitationV1]
END