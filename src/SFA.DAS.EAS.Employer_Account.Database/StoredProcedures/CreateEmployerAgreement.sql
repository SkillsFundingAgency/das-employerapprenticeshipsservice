CREATE PROCEDURE [employer_account].[CreateEmployerAgreement]
	@accountLegalEntityId BIGINT,
	@templateId INT = NULL,
	@eoi BIT,
	@employerAgreementId BIGINT OUTPUT
AS
BEGIN	
	SET NOCOUNT ON

	IF @templateId IS NULL
	BEGIN
		SELECT TOP 1 @templateId = Id
		FROM [employer_account].[EmployerAgreementTemplate]
		WHERE AgreementType LIKE CASE @eoi WHEN 0 THEN 'Levy' ELSE 'NonLevy.EOI%' END
		ORDER BY VersionNumber DESC
	END

	INSERT INTO [employer_account].[EmployerAgreement] (AccountLegalEntityId, TemplateId, StatusId) 
	VALUES (@accountLegalEntityId, @templateId, 1)

	SELECT @employerAgreementId = SCOPE_IDENTITY()
END