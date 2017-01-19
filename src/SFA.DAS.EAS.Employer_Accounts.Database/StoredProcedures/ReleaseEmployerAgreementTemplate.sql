CREATE PROCEDURE [employer_account].[ReleaseEmployerAgreementTemplate]
	@templateId INT
AS
BEGIN
	SET NOCOUNT ON;

	DECLARE @lastTemplateId BIGINT;
	DECLARE @expiryDays INT;

	--get the current template id
	SELECT TOP 1 @lastTemplateId = Id 
	FROM [employer_account].[EmployerAgreementTemplate]
	WHERE ReleasedDate IS NOT NULL
	ORDER BY ReleasedDate DESC; 
	
	SELECT @expiryDays = ExpiryDays
	FROM [employer_account].[EmployerAgreementTemplate]
	WHERE Id = @lastTemplateId; 

	--set expirydate for agreements using previous template
	UPDATE [employer_account].[EmployerAgreement]
	SET ExpiredDate = GETDATE()+@expiryDays
	WHERE TemplateId = @lastTemplateId;

	--insert new record for each legalentity with new template
	INSERT INTO [employer_account].[EmployerAgreement](LegalEntityId, TemplateId, StatusId)
	SELECT LegalEntityId, @templateId, 1 
	FROM [employer_account].[EmployerAgreement]
	WHERE TemplateId = @lastTemplateId;

	--insert into AccountEmployerAgreement with new Ids
	INSERT INTO [employer_account].[AccountEmployerAgreement]
	SELECT aea.AccountId, t.Id
	FROM [employer_account].[AccountEmployerAgreement] aea
		JOIN [employer_account].[EmployerAgreement] ea
			ON ea.Id = aea.EmployerAgreementId
		JOIN 
		(
			SELECT *
			FROM [employer_account].[EmployerAgreement]
			WHERE TemplateId = @templateId
		) t
		ON t.LegalEntityId = ea.LegalEntityId
	WHERE ea.TemplateId = @lastTemplateId;

	UPDATE [employer_account].[EmployerAgreementTemplate]
	SET [ReleasedDate] = GETDATE()
	WHERE Id = @templateId;
END
