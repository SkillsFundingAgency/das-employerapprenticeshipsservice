CREATE PROCEDURE [levy].[GetAccountBalance_ByAccountIds]
	@accountIds [levy].[AccountIds] Readonly
AS
	select 
		AccountId,
		Sum(Amount) Balance 
	from 
		levy.TransactionLine 
	Group by AccountId

