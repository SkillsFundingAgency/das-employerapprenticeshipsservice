CREATE PROCEDURE [employer_account].[GetPaye_ByRef]
	@Ref NVARCHAR(16)
AS

Select TOP 1
	paye.Ref as EmpRef,
    paye.Name as RefName
from 
	employer_account.Paye paye
WHERE
	paye.Ref = @Ref
