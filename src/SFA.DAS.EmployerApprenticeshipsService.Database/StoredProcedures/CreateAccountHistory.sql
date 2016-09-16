CREATE PROCEDURE [account].[CreateAccountHistory]
	@AccountId BIGINT,
	@PayeRef VARCHAR(20),
	@AddedDate DATETIME
AS
	INSERT INTO [account].[AccountHistory] (AccountId, PayeRef, AddedDate)
	VALUES (@AccountId, @PayeRef, @AddedDate)

