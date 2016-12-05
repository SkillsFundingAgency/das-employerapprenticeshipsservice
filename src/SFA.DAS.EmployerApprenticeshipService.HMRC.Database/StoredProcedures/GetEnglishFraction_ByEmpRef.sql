CREATE PROCEDURE [levy].[GetEnglishFraction_ByEmpRef]
	@empref varchar(50)
as
select 
	* 
from levy.EnglishFraction
where EmpRef = @empref
