DECLARE @sqlStatement NVARCHAR(4000) = '
	CREATE PROCEDURE #CreateLevy
		@AccountId BIGINT,
		@payeScheme NVARCHAR(50),
		@SubmissionId BIGINT,
		@levyAmount DECIMAL
	AS
	BEGIN
		DECLARE @SubmissionDate DATETIME = GETDATE()
		IF (NOT EXISTS (SELECT 1 FROM [employer_financial].[LevyDeclaration] WHERE SubmissionId = @SubmissionId))
		BEGIN 
			EXEC [employer_financial].[CreateDeclaration] @levyAmount, @payeScheme, @SubmissionDate, @SubmissionId, 123, @AccountId, @levyAmount, ''17-18'', 1, @SubmissionDate, NULL, NULL, NULL, 0, 0, 0
		END
	END'

EXEC sp_executesql @sqlStatement

EXECUTE #CreateLevy 1, '222/AA00002', 126, 500
EXECUTE #CreateLevy 2, '123/SFAT029', 127, 1000000
EXECUTE #CreateLevy 3, '101/CUR00016', 128, 0
EXECUTE [employer_financial].[ProcessDeclarationsTransactions] 1, '222/AA00002'
EXECUTE [employer_financial].[ProcessDeclarationsTransactions] 2, '123/SFAT029'
EXECUTE [employer_financial].[ProcessDeclarationsTransactions] 3, '101/CUR00016'

DROP PROCEDURE #CreateLevy