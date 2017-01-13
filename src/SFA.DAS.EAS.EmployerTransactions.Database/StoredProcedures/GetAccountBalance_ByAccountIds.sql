CREATE PROCEDURE [employer_transactions].[GetAccountBalance_ByAccountIds]
	@accountIds [employer_transactions].[AccountIds] Readonly
AS
	select 
		AccountId,
		Sum(Amount) Balance 
	from 
		[employer_transactions].TransactionLine 
	Group by AccountId

