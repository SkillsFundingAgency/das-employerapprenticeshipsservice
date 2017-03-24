CREATE PROCEDURE [employer_financial].[GetAccountBalance_ByAccountIds]
	@accountIds [employer_financial].[AccountIds] Readonly
AS
	select 
		tl.AccountId,
		Sum(Amount) Balance 
	from 
		[employer_financial].TransactionLine tl
	inner join 
		@accountIds acc on acc.AccountId = tl.AccountId
	Group by tl.AccountId

