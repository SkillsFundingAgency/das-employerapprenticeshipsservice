SET NOCOUNT ON;

PRINT 'Update V3 Template';

BEGIN TRAN;

update employer_account.EmployerAgreementTemplate
SET AgreementType = 2 where PartialViewName = '_Agreement_V3'

COMMIT TRAN;