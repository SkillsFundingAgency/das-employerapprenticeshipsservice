CREATE PROCEDURE [employer_account].[CreateAccountHistory]
	@AccountId BIGINT,
	@PayeRef VARCHAR(20),
	@AddedDate DATETIME
AS
	INSERT INTO [employer_account].[AccountHistory] (AccountId, PayeRef, AddedDate)
	VALUES (@AccountId, @PayeRef, @AddedDate)

