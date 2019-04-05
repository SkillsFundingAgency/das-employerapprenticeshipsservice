
IF object_id(N'DataGen.CalendarPeriodMonth', N'FN') IS NOT NULL
    DROP FUNCTION DataGen.CalendarPeriodMonth
GO

IF object_id(N'DataGen.CalendarPeriodYear', N'FN') IS NOT NULL
    DROP FUNCTION DataGen.CalendarPeriodYear
GO

IF object_id(N'DataGen.CollectionPeriodMonth', N'FN') IS NOT NULL
    DROP FUNCTION DataGen.CollectionPeriodMonth
GO

IF object_id(N'DataGen.CollectionPeriodYear', N'FN') IS NOT NULL
    DROP FUNCTION DataGen.CollectionPeriodYear
GO

IF object_id(N'DataGen.PeriodEndMonth', N'FN') IS NOT NULL
    DROP FUNCTION DataGen.PeriodEndMonth
GO

IF object_id(N'DataGen.PeriodEndYear', N'FN') IS NOT NULL
    DROP FUNCTION DataGen.PeriodEndYear
GO

IF object_id(N'DataGen.PeriodEnd', N'FN') IS NOT NULL
    DROP FUNCTION DataGen.PeriodEnd
GO

IF object_id(N'DataGen.CollectionPeriodId', N'FN') IS NOT NULL
    DROP FUNCTION DataGen.CollectionPeriodId
GO

IF EXISTS (SELECT * FROM sys.schemas WHERE name = 'DataGen')
BEGIN
	EXEC('DROP SCHEMA DataGen')
END
GO
