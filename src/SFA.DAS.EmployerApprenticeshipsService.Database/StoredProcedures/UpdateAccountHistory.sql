CREATE PROCEDURE [account].[UpdateAccountHistory]
	@AccountId BIGINT,
	@PayeRef VARCHAR(20),
	@RemovedDate DATETIME
AS
	UPDATE [Account].[AccountHistory] SET RemovedDate = @RemovedDate 
	WHERE AccountId = @AccountId and PayeRef = @PayeRef

