CREATE PROCEDURE [account].[AddPayeToAccountForNewLegalEntity]
	@accountId BIGINT,
	@companyNumber NVARCHAR(50),
	@companyName NVARCHAR(255),
	@CompanyAddress NVARCHAR(255),
	@CompanyDateOfIncorporation DATETIME,
	@employerRef  NVARCHAR(16),
	@accessToken VARCHAR(30),
	@refreshToken VARCHAR(30),
	@addedDate DATETIME
AS
BEGIN
	DECLARE @legalEntityId BIGINT
	DECLARE @employerAgreementId BIGINT

	EXEC [account].[CreateLegalEntity] @companyNumber,@companyName,@CompanyAddress,@CompanyDateOfIncorporation,@legalEntityId OUTPUT

	IF EXISTS(select 1 from [account].[Paye] where ref = @employerRef)
	BEGIN
		EXEC [account].[UpdatePaye] @legalEntityId,@employerRef,@accessToken, @refreshToken
	END
	ELSE
	BEGIN
		EXEC [account].[CreatePaye] @legalEntityId,@employerRef,@accessToken, @refreshToken
	END

	EXEC [account].[CreateAccountHistory] @accountId,@employerRef,@addedDate

	EXEC [account].[CreateEmployerAgreement] @legalEntityId, @employerAgreementId OUTPUT

	EXEC [account].[CreateAccountEmployerAgreement] @accountId, @employerAgreementId
	
END



