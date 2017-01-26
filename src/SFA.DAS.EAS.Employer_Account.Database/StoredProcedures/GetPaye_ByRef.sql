CREATE PROCEDURE employer_account.GetPaye_ByRef
	@Ref NVARCHAR(16)
AS
Select 
	paye.Ref,
    paye.Name
from employer_account.Paye paye
WHERE paye.Ref = @Ref