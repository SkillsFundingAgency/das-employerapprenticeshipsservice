CREATE PROCEDURE [account].[ReleaseEmployerAgreementTemplate]
	@templateId INT
AS
BEGIN
	SET NOCOUNT ON;

	DECLARE @lastTemplateId BIGINT;
	DECLARE @expiryDays INT;

	--get the current template id
	SELECT TOP 1 @lastTemplateId = Id 
	FROM [account].[EmployerAgreementTemplate]
	WHERE ReleasedDate IS NOT NULL
	ORDER BY ReleasedDate DESC; 
	
	SELECT @expiryDays = ExpiryDays
	FROM [account].[EmployerAgreementTemplate]
	WHERE Id = @lastTemplateId; 

	--set expirydate for agreements using previous template
	UPDATE [account].[EmployerAgreement]
	SET ExpiredDate = GETDATE()+@expiryDays
	WHERE TemplateId = @lastTemplateId;

	--insert new record for each legalentity with new template
	INSERT INTO [account].[EmployerAgreement](LegalEntityId, TemplateId, StatusId)
	SELECT LegalEntityId, @templateId, 1 
	FROM [account].[EmployerAgreement]
	WHERE TemplateId = @lastTemplateId;

	--insert into AccountEmployerAgreement with new Ids
	INSERT INTO [account].[AccountEmployerAgreement]
	SELECT aea.AccountId, t.Id
	FROM [account].[AccountEmployerAgreement] aea
		JOIN [account].[EmployerAgreement] ea
			ON ea.Id = aea.EmployerAgreementId
		JOIN 
		(
			SELECT *
			FROM [account].[EmployerAgreement]
			WHERE TemplateId = @templateId
		) t
		ON t.LegalEntityId = ea.LegalEntityId
	WHERE ea.TemplateId = @lastTemplateId;

	UPDATE [account].[EmployerAgreementTemplate]
	SET [ReleasedDate] = GETDATE()
	WHERE Id = @templateId;
END
