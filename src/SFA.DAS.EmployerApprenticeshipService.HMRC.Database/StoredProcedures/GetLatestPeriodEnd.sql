CREATE PROCEDURE [levy].[GetLatestPeriodEnd]
	
AS

select top 1 
	* 
from 
	[levy].[PeriodEnd] 
order by 
	completiondatetime desc
