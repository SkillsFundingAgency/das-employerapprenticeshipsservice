
IF NOT EXISTS (SELECT * FROM sys.schemas WHERE name = 'DataGen')
BEGIN
	EXEC('CREATE SCHEMA DataGen')
END
GO

CREATE OR ALTER FUNCTION DataGen.CalendarPeriodMonth (@date datetime)  
RETURNS int 
AS  
BEGIN  
  declare @month int = DATEPART(month,@date)
  RETURN(@month);  
END; 
GO

CREATE OR ALTER FUNCTION DataGen.CalendarPeriodYear (@date datetime)  
RETURNS int
AS  
BEGIN  
  declare @year int = DATEPART(year,@date)
  return @year
END; 
GO

CREATE OR ALTER FUNCTION DataGen.CollectionPeriodMonth (@date datetime)  
RETURNS int 
AS  
BEGIN  
  declare @collectionPeriodMonth int = (select dbo.CalendarPeriodMonth(@date))
  return @collectionPeriodMonth
END; 
GO

CREATE OR ALTER FUNCTION DataGen.CollectionPeriodYear (@date datetime)  
RETURNS int
AS  
BEGIN  
  declare @collectionPeriodYear int = (select dbo.CalendarPeriodYear(@date))
  return @collectionPeriodYear
END; 
GO

--todo R13, R14
CREATE OR ALTER FUNCTION DataGen.PeriodEndMonth (@date datetime)  
RETURNS VARCHAR(5)
AS  
BEGIN  
  declare @month int = DATEPART(month,@date)

  SET @month = @month - 7
  IF @month < 1
	SET @month = @month + 12

  declare @periodEndMonth VARCHAR(5) = CONVERT(varchar(5), @month)
  RETURN @periodEndMonth; 
END; 
GO

--todo R13, R14
CREATE OR ALTER FUNCTION DataGen.PeriodEndYear (@date datetime)  
RETURNS VARCHAR(5)
AS  
BEGIN  
  declare @month int = DATEPART(month,@date)
  declare @year int = DATEPART(year,@date)

  if @month < 8
	SET @year = @year - 1
	
  DECLARE @periodEndYear VARCHAR(4) = (SELECT RIGHT(CONVERT(VARCHAR(5), @year, 1), 2)) + (SELECT RIGHT(CONVERT(VARCHAR(4), @year+1, 1), 2))
  return @periodEndYear
END; 
GO

CREATE OR ALTER FUNCTION DataGen.PeriodEnd (@date datetime)  
RETURNS VARCHAR(8)
AS  
BEGIN  
  declare @periodEnd varchar(8) = (SELECT dbo.PeriodEndYear(@date) + '-R' + right('0' + dbo.PeriodEndMonth(@date),2))
  return @periodEnd
END; 
GO
