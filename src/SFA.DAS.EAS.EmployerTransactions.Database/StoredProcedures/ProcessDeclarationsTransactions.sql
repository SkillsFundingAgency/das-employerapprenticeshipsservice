CREATE PROCEDURE [employer_transactions].[ProcessDeclarationsTransactions]
AS


--Add the topup from the declaration
INSERT INTO [employer_transactions].LevyDeclarationTopup
	select mainUpdate.* from
	(
select 
		x.AccountId,
		GetDate() as DateAdded,
		x.SubmissionId,
		x.SubmissionDate,
		((y.LevyDueYTD - isnull(LAG(y.LevyDueYTD) OVER(Partition by y.empref order by y.SubmissionDate asc, y.submissionId),0)) * y.EnglishFraction) * y.TopUpPercentage as Amount
	FROM 
		[employer_transactions].[GetLevyDeclarations] x
	
	left join 
		[employer_transactions].[GetLevyDeclarations] y on y.lastsubmission = 1 and y.id = x.id
	where
		y.LevyDueYTD is not null
	) mainUpdate
	inner join (
		select submissionId from [employer_transactions].LevyDeclaration
	EXCEPT
		select SubmissionId from [employer_transactions].LevyDeclarationTopup
	) dervx on dervx.submissionId = mainUpdate.SubmissionId


-- Create Declarations


INSERT INTO [employer_transactions].TransactionLine
select mainUpdate.* from
	(
	select 
			x.AccountId,
			GetDate() as DateCreated,
			x.SubmissionId as SubmissionId,
			x.SubmissionDate as TransactionDate,
			1 as TransactionType,
			(y.LevyDueYTD - isnull(LAG(y.LevyDueYTD) OVER(Partition by y.empref order by y.SubmissionDate asc, y.submissionId),0)) as LevyDeclared,
			((y.LevyDueYTD - isnull(LAG(y.LevyDueYTD) OVER(Partition by y.empref order by y.SubmissionDate asc, y.submissionId),0)) * y.EnglishFraction) + ldt.amount as Amount,
			x.EmpRef as EmpRef,
			null as PeriodEnd,
			null as UkPrn
		FROM 
			[employer_transactions].[GetLevyDeclarations] x
		inner join
			[employer_transactions].[LevyDeclarationTopup] ldt on ldt.submissionId = x.submissionId
		left join 
			[employer_transactions].[GetLevyDeclarations] y on y.lastsubmission = 1 and y.id = x.id
		where
			y.LevyDueYTD is not null
	) mainUpdate
	inner join (
		select submissionId from [employer_transactions].LevyDeclaration
	EXCEPT
		select SubmissionId from [employer_transactions].TransactionLine where TransactionType = 1
	) dervx on dervx.submissionId = mainUpdate.SubmissionId