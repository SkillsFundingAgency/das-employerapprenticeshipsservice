CREATE PROCEDURE [employer_account].[AddPayeToAccount]
	@accountId BIGINT,
	@employerRef  NVARCHAR(16),
	@accessToken VARCHAR(30),
	@refreshToken VARCHAR(30),
	@addedDate DATETIME,
	@employerRefName VARCHAR(500) NULL,
	@aorn VARCHAR(50) NULL
AS
BEGIN
	DECLARE @employerAgreementId BIGINT

	
	IF EXISTS(select 1 from [employer_account].[Paye] where Ref = @employerRef)
	BEGIN
		EXEC [employer_account].[UpdatePaye] @employerRef,@accessToken, @refreshToken, @employerRefName, @aorn
	END
	ELSE
	BEGIN
		EXEC [employer_account].[CreatePaye] @employerRef,@accessToken, @refreshToken, @employerRefName, @aorn
	END

	EXEC [employer_account].[CreateAccountHistory] @accountId,@employerRef,@addedDate

END



