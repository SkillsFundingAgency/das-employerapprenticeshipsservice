
-- ________                                                                         ____                                                                                  ___    __                    ___               
-- `MMMMMMMb.                                                                      6MMMMb/                                                   68b                          `MM    d'  @                   MM              
--  MM    `Mb                                                        /            8P    YM                                             /     Y89                           MM   d'                      MM               
--  MM     MM    ___  ____    ___ ___  __    __     ____  ___  __   /M           6M      Y   ____  ___  __     ____  ___  __    ___   /M     ___   _____  ___  __          MM  d'   ___  __     _____   MM____     ____  
--  MM     MM  6MMMMb `MM(    )M' `MM 6MMb  6MMb   6MMMMb `MM 6MMb /MMMMM        MM         6MMMMb `MM 6MMb   6MMMMb `MM 6MM  6MMMMb /MMMMM  `MM  6MMMMMb `MM 6MMb         MM d'    `MM 6MMb   6MMMMMb  MMMMMMb   6MMMMb\
--  MM    .M9 8M'  `Mb `Mb    d'   MM69 `MM69 `Mb 6M'  `Mb MMM9 `Mb MM           MM        6M'  `Mb MMM9 `Mb 6M'  `Mb MM69 " 8M'  `Mb MM      MM 6M'   `Mb MMM9 `Mb        MMd'      MMM9 `Mb 6M'   `Mb MM'  `Mb MM'    `
--  MMMMMMM9'     ,oMM  YM.  ,P    MM'   MM'   MM MM    MM MM'   MM MM           MM     ___MM    MM MM'   MM MM    MM MM'        ,oMM MM      MM MM     MM MM'   MM        MMYM.     MM'   MM MM     MM MM    MM YM.     
--  MM        ,6MM9'MM   MM  M     MM    MM    MM MMMMMMMM MM    MM MM           MM     `M'MMMMMMMM MM    MM MMMMMMMM MM     ,6MM9'MM MM      MM MM     MM MM    MM        MM YM.    MM    MM MM     MM MM    MM  YMMMMb 
--  MM        MM'   MM   `Mbd'     MM    MM    MM MM       MM    MM MM           YM      M MM       MM    MM MM       MM     MM'   MM MM      MM MM     MM MM    MM        MM  YM.   MM    MM MM     MM MM    MM      `Mb
--  MM        MM.  ,MM    YMP      MM    MM    MM YM    d9 MM    MM YM.  ,        8b    d9 YM    d9 MM    MM YM    d9 MM     MM.  ,MM YM.  ,  MM YM.   ,M9 MM    MM        MM   YM.  MM    MM YM.   ,M9 MM.  ,M9 L    ,MM
-- _MM_       `YMMM9'Yb.   M      _MM_  _MM_  _MM_ YMMMM9 _MM_  _MM_ YMMM9         YMMMM9   YMMMM9 _MM_  _MM_ YMMMM9 _MM_    `YMMM9'Yb.YMMM9 _MM_ YMMMMM9 _MM_  _MM_      _MM_   YM._MM_  _MM_ YMMMMM9 _MYMMMM9  MYMMMM9 
--                        d'                                                                                                                                                                                             
--                    (8),P                                                                                                                                                                                              
--                     YMM                                                                                                                                                                                               

	declare @accountId BIGINT                          = 1
    declare @accountName NVARCHAR(100)                 = 'Insert Name Here'
	declare @toDate DATETIME                           = GETDATE()
	declare @numberOfMonthsToCreate INT                = 25
	declare @defaultMonthlyTotalPayments DECIMAL(18,5) = 100
	declare @defaultPaymentsPerMonth int               = 3

--  _____  _____  _____  _____  _____  _____  _____  _____  _____  _____  _____  _____  _____  _____  _____  _____  _____  _____  _____  _____  _____  _____  _____ 
-- [_____][_____][_____][_____][_____][_____][_____][_____][_____][_____][_____][_____][_____][_____][_____][_____][_____][_____][_____][_____][_____][_____][_____]

declare @paymentsByMonth DataGen.PaymentGenerationSourceTable

insert @paymentsByMonth
select * from DataGen.GenerateSourceTable(@accountId, @accountName, @toDate, @numberOfMonthsToCreate, @defaultMonthlyTotalPayments, @defaultPaymentsPerMonth)

-- override defaults here...
-- e.g. to create refunds set the amount -ve
--UPDATE @paymentsByMonth SET amount = -500, paymentsToGenerate = 1 where monthBeforeToDate = -1
--UPDATE @paymentsByMonth SET amount = -500, paymentsToGenerate = 1 where monthBeforeToDate = -7

select * from @paymentsByMonth

exec DataGen.CreatePaymentsForMonths @accountId, @accountName, @paymentsByMonth
