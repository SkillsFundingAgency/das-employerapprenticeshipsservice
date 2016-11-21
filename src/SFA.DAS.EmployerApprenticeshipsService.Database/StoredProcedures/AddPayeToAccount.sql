CREATE PROCEDURE [account].[AddPayeToAccount]
	@accountId BIGINT,
	@employerRef  NVARCHAR(16),
	@accessToken VARCHAR(30),
	@refreshToken VARCHAR(30),
	@addedDate DATETIME
AS
BEGIN
	DECLARE @employerAgreementId BIGINT

	
	IF EXISTS(select 1 from [account].[Paye] where ref = @employerRef)
	BEGIN
		EXEC [account].[UpdatePaye] @employerRef,@accessToken, @refreshToken
	END
	ELSE
	BEGIN
		EXEC [account].[CreatePaye] @employerRef,@accessToken, @refreshToken
	END

	EXEC [account].[CreateAccountHistory] @accountId,@employerRef,@addedDate

END



