CREATE PROCEDURE [dbo].[AddPayeToAccountForExistingLegalEntity]
	@accountId BIGINT,
	@legalEntityId BIGINT,
	@employerRef NVARCHAR(16)
AS
BEGIN
	INSERT INTO [dbo].[Paye](Ref, AccountId, LegalEntityId) VALUES (@employerRef, @accountId, @legalEntityId);
END
