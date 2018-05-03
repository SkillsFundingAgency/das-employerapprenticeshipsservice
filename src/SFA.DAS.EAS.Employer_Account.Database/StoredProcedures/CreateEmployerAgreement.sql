CREATE PROCEDURE [employer_account].[CreateEmployerAgreement]
	@legalEntityId BIGINT,
	@accountId BIGINT,
	@templateId INT = NULL,
	@employerAgreementId BIGINT OUTPUT
AS
BEGIN	
	SET NOCOUNT ON

	IF @templateId IS NULL
	BEGIN
		SELECT TOP 1 @templateId = Id
		FROM [employer_account].[EmployerAgreementTemplate]
		ORDER BY VersionNumber DESC
	END

	INSERT INTO [employer_account].[EmployerAgreement] (LegalEntityId, AccountId, TemplateId, StatusId) 
	VALUES (@legalEntityId, @accountId, @templateId, 1)

	SELECT @employerAgreementId = SCOPE_IDENTITY()
END