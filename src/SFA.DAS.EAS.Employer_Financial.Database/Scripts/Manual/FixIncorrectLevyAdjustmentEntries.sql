BEGIN TRY
BEGIN TRANSACTION
	
	-- UPDATE LEVY DECLARATION ADJUSTMENT AMOUNT VALUE
	UPDATE LevyDeclaration
	SET LevyDeclaration.EndOfYearAdjustmentAmount = levyAdjustment.AdjustmentAmount
	FROM [employer_financial].[LevyDeclaration]  LevyDeclaration
	INNER JOIN 
	(SELECT
		tl.submissionId as submissionId	
	    ,(ISNULL(ld2.LevyDueYTD, 0) - ld.LevyDueYTD) as AdjustmentAmount
	FROM [employer_financial].[LevyDeclaration] ld
		INNER JOIN [employer_financial].[TransactionLine] tl ON ld.SubmissionId = tl.SubmissionId
		CROSS APPLY(
			SELECT TOP(1) * FROM [employer_financial].[LevyDeclaration] 
			WHERE AccountId = ld.AccountId 
			AND empRef = ld.empRef
			AND SubmissionId <> ld.SubmissionId
			AND PayrollYear = ld.PayrollYear
			AND SubmissionDate <= ld.SubmissionDate
			ORDER BY SubmissionDate DESC
		) ld2
		WHERE ld.EndOfYearAdjustment = 1 
		AND tl.Amount = 0
		AND tl.LevyDeclared <> 0) levyAdjustment ON LevyDeclaration.submissionId = levyAdjustment.submissionId

	-- UPDATE LEVY TOPUP AMOUNT VALUE
	UPDATE LevyTopUp
	SET LevyTopUp.Amount = levyAdjustment.TopUpAmount
	FROM [employer_financial].[LevyDeclarationTopup]  LevyTopUp
	INNER JOIN 
	(SELECT
		tl.submissionId as submissionId	
	    ,(ld.LevyDueYTD - ISNULL(ld2.LevyDueYTD, 0)) * ISNULL(ld2.EnglishFraction, 1) * 0.1 as TopUpAmount  
	FROM [employer_financial].[LevyDeclaration] ld
		INNER JOIN [employer_financial].[TransactionLine] tl ON ld.SubmissionId = tl.SubmissionId
		CROSS APPLY(
			SELECT TOP(1) levy.*,  ldtu.EnglishFraction FROM [employer_financial].[LevyDeclaration] levy
			INNER JOIN [employer_financial].[GetLevyDeclarationAndTopUp] ldtu ON ldtu.SubmissionId = levy.SubmissionId
			WHERE levy.AccountId = ld.AccountId 
			AND levy.empRef = ld.empRef
			AND levy.SubmissionId <> ld.SubmissionId
			AND levy.PayrollYear = ld.PayrollYear
			AND levy.SubmissionDate <= ld.SubmissionDate
			ORDER BY levy.SubmissionDate DESC
		) ld2
		WHERE ld.EndOfYearAdjustment = 1 
		AND tl.Amount = 0
		AND tl.LevyDeclared <> 0) levyAdjustment ON LevyTopUp.submissionId = levyAdjustment.submissionId

	-- UPDATE TRANSACTION LINE AMOUNT AND Levy YTD
	UPDATE TransactionLine
	SET transactionLine.Amount = levyAdjustment.NewAmount
	    ,transactionLine.LevyDeclared = levyAdjustment.NewLevyYtd
	FROM [employer_financial].[TransactionLine] transactionLine
	INNER JOIN 
	(SELECT
		tl.submissionId as submissionId	
	    ,(ld.LevyDueYTD - ISNULL(ld2.LevyDueYTD, 0)) * ISNULL(ld2.EnglishFraction, 1) * 1.1 as NewAmount
		,(ld.LevyDueYTD - ISNULL(ld2.LevyDueYTD, 0)) as NewLevyYtd
	FROM [employer_financial].[LevyDeclaration] ld
		INNER JOIN [employer_financial].[TransactionLine] tl ON ld.SubmissionId = tl.SubmissionId
		CROSS APPLY(
			SELECT TOP(1) levy.*, ldtu.EnglishFraction FROM [employer_financial].[LevyDeclaration] levy
			INNER JOIN [employer_financial].[GetLevyDeclarationAndTopUp] ldtu ON ldtu.SubmissionId = levy.SubmissionId
			WHERE levy.AccountId = ld.AccountId 
			AND levy.empRef = ld.empRef
			AND levy.SubmissionId <> ld.SubmissionId
			AND levy.PayrollYear = ld.PayrollYear
			AND levy.SubmissionDate <= ld.SubmissionDate
			ORDER BY levy.SubmissionDate DESC
		) ld2
		WHERE ld.EndOfYearAdjustment = 1 
		AND tl.Amount = 0
		AND tl.LevyDeclared <> 0) levyAdjustment ON transactionLine.submissionId = levyAdjustment.submissionId

	UPDATE TransactionLine
	SET transactionLine.EnglishFraction = ldtu.EnglishFraction   
	FROM [employer_financial].[TransactionLine] transactionLine	
	INNER JOIN [employer_financial].[GetLevyDeclarationAndTopUp] ldtu ON ldtu.SubmissionId = transactionLine.SubmissionId
	WHERE transactionLine.EnglishFraction IS NULL
	AND ldtu.EndOfYearAdjustment = 1
	
END TRY
BEGIN CATCH
	DECLARE @ErrorMsg nvarchar(max)
	DECLARE @ErrorSeverity INT;  
	DECLARE @ErrorState INT;  

	SET @ErrorMsg = ERROR_NUMBER() + ERROR_LINE() + ERROR_MESSAGE()
	SET @ErrorSeverity = ERROR_SEVERITY()
	SET @ErrorState = ERROR_STATE()

	IF @@TRANCOUNT > 0
		ROLLBACK TRANSACTION 
	
	RAISERROR(@ErrorMsg, @ErrorSeverity, @ErrorState)
END CATCH

IF @@TRANCOUNT > 0
	COMMIT TRANSACTION