DECLARE @sqlStatement NVARCHAR(4000) = '
	CREATE PROCEDURE #CreateLevy
		@AccountId BIGINT,
		@payeScheme NVARCHAR(50),
		@levyAmount DECIMAL
	AS
	BEGIN
		DECLARE @SubmissionDate DATETIME = GETDATE()

		EXEC [employer_financial].[CreateDeclaration] @levyAmount, @payeScheme, @SubmissionDate, 123, 123, @AccountId, @levyAmount, ''17-18'', 1, @SubmissionDate, NULL, NULL, NULL, 0, 0, 0
	END'

EXEC sp_executesql @sqlStatement

EXECUTE #CreateLevy 1, '222/AA00002', 500
EXECUTE #CreateLevy 2, '123/SFAT029', 1000000
EXECUTE #CreateLevy 3, '101/CUR00016', 0
EXECUTE [employer_financial].[ProcessDeclarationsTransactions] 1, '222/AA00002'
EXECUTE [employer_financial].[ProcessDeclarationsTransactions] 2, '123/SFAT029'
EXECUTE [employer_financial].[ProcessDeclarationsTransactions] 3, '101/CUR00016'

DROP PROCEDURE #CreateLevy