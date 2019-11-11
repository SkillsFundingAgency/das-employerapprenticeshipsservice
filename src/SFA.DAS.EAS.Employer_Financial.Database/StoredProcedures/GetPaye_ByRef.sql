CREATE PROCEDURE [employer_financial].[GetPaye_ByRef]
	@Ref NVARCHAR(16)
AS

Select TOP 1
	paye.EmpRef as EmpRef,
    paye.Name as RefName
from 
	[employer_financial].AccountPaye paye
WHERE
	paye.EmpRef = @Ref
