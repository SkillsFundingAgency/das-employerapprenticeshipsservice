CREATE PROCEDURE [dbo].[AddPayeToAccountForNewLegalEntity]
	@accountId BIGINT,
	@companyNumber NVARCHAR(50),
	@companyName NVARCHAR(255),
	@CompanyAddress NVARCHAR(255),
	@CompanyDateOfIncorporation DATETIME,
	@employerRef  NVARCHAR(16),
	@accessToken VARCHAR(30),
	@refreshToken VARCHAR(30)
AS
BEGIN
	DECLARE @legalEntityId BIGINT

	EXEC [CreateLegalEntity] @companyNumber,@companyName,@CompanyAddress,@CompanyDateOfIncorporation,@legalEntityId OUTPUT

	EXEC [AddPayeToAccountForExistingLegalEntity] @accountId, @legalEntityId,@employerRef,@accessToken, @refreshToken

	EXEC [CreateEmployerAgreement] @legalEntityId


END



