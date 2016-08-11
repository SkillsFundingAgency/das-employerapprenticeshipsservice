CREATE PROCEDURE [dbo].[AddPayeToAccountForExistingLegalEntity]
	@accountId BIGINT,
	@legalEntityId BIGINT,
	@employerRef NVARCHAR(16),
	@accessToken VARCHAR(50),
	@refreshToken VARCHAR(50)
AS
BEGIN
	INSERT INTO [dbo].[Paye](Ref, AccountId, LegalEntityId, AccessToken, RefreshToken) VALUES (@employerRef, @accountId, @legalEntityId, @accessToken, @refreshToken);
END
