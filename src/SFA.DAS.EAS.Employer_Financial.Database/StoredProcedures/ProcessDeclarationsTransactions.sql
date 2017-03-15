CREATE PROCEDURE [employer_financial].[ProcessDeclarationsTransactions]
AS


--Add the topup from the declaration
INSERT INTO [employer_financial].LevyDeclarationTopup
	select mainUpdate.* from
	(
select 
		x.AccountId,
		GetDate() as DateAdded,
		x.SubmissionId,
		x.SubmissionDate,
		((y.LevyDueYTD - isnull(LAG(y.LevyDueYTD) OVER(Partition by y.empref order by y.SubmissionDate asc, y.submissionId),0)) * isnull(y.EnglishFraction,0)) * isnull(y.TopUpPercentage,0) as Amount
	FROM 
		[employer_financial].[GetLevyDeclarations] x
	
	left join 
		[employer_financial].[GetLevyDeclarations] y on y.lastsubmission = 1 and y.id = x.id
	where
		y.LevyDueYTD is not null
	) mainUpdate
	inner join (
		select submissionId from [employer_financial].LevyDeclaration
	EXCEPT
		select SubmissionId from [employer_financial].LevyDeclarationTopup
	) dervx on dervx.submissionId = mainUpdate.SubmissionId


-- Create Declarations


INSERT INTO [employer_financial].TransactionLine
select mainUpdate.* from
	(
	select 
			x.AccountId,
			y.CreatedDate as DateCreated,
			x.SubmissionId as SubmissionId,
			x.SubmissionDate as TransactionDate,
			1 as TransactionType,
			(y.LevyDueYTD - isnull(LAG(y.LevyDueYTD) OVER(Partition by y.empref order by y.SubmissionDate asc, y.submissionId),0)) as LevyDeclared,
			((y.LevyDueYTD - isnull(LAG(y.LevyDueYTD) OVER(Partition by y.empref order by y.SubmissionDate asc, y.submissionId),0)) * ISNULL(y.EnglishFraction,0)) + ldt.amount as Amount,
			x.EmpRef as EmpRef,
			null as PeriodEnd,
			null as UkPrn
		FROM 
			[employer_financial].[GetLevyDeclarations] x
		inner join
			[employer_financial].[LevyDeclarationTopup] ldt on ldt.submissionId = x.submissionId
		left join 
			[employer_financial].[GetLevyDeclarations] y on y.lastsubmission = 1 and y.id = x.id
		where
			y.LevyDueYTD is not null
	) mainUpdate
	inner join (
		select submissionId from [employer_financial].LevyDeclaration
	EXCEPT
		select SubmissionId from [employer_financial].TransactionLine where TransactionType = 1
	) dervx on dervx.submissionId = mainUpdate.SubmissionId
GO