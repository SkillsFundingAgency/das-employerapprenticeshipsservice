	--  ,--.--------.              ,---.      .-._          ,-,--.     _,---.     ,----.                           _,---.      ,----.  .-._           ,----.                ,---.   ,--.--------.  .=-.-.  _,.---._    .-._                ,--.-.,-.  .-._          _,.---._                 ,-,--. 
	-- /==/,  -   , -\.-.,.---.  .--.'  \    /==/ \  .-._ ,-.'-  _\ .-`.' ,  \ ,-.--` , \  .-.,.---.           _.='.'-,  \  ,-.--` , \/==/ \  .-._ ,-.--` , \  .-.,.---.  .--.'  \ /==/,  -   , -\/==/_ /,-.' , -  `. /==/ \  .-._        /==/- |\  \/==/ \  .-._ ,-.' , -  `.    _..---.  ,-.'-  _\
	-- \==\.-.  - ,-./==/  `   \ \==\-/\ \   |==|, \/ /, /==/_ ,_.'/==/_  _.-'|==|-  _.-` /==/  `   \         /==.'-     / |==|-  _.-`|==|, \/ /, /==|-  _.-` /==/  `   \ \==\-/\ \\==\.-.  - ,-./==|, |/==/_,  ,  - \|==|, \/ /, /       |==|_ `/_ /|==|, \/ /, /==/_,  ,  - \ .' .'.-. \/==/_ ,_.'
	--  `--`\==\- \ |==|-, .=., |/==/-|_\ |  |==|-  \|  |\==\  \  /==/-  '..-.|==|   `.-.|==|-, .=., |       /==/ -   .-'  |==|   `.-.|==|-  \|  ||==|   `.-.|==|-, .=., |/==/-|_\ |`--`\==\- \  |==|  |==|   .=.     |==|-  \|  |        |==| ,   / |==|-  \|  |==|   .=.     /==/- '=' /\==\  \   
	--       \==\_ \|==|   '='  /\==\,   - \ |==| ,  | -| \==\ -\ |==|_ ,    /==/_ ,    /|==|   '='  /       |==|_   /_,-./==/_ ,    /|==| ,  | -/==/_ ,    /|==|   '='  /\==\,   - \    \==\_ \ |==|- |==|_ : ;=:  - |==| ,  | -|        |==|-  .|  |==| ,  | -|==|_ : ;=:  - |==|-,   '  \==\ -\  
	--       |==|- ||==|- ,   .' /==/ -   ,| |==| -   _ | _\==\ ,\|==|   .--'|==|    .-' |==|- ,   .'        |==|  , \_.' )==|    .-' |==| -   _ |==|    .-' |==|- ,   .' /==/ -   ,|    |==|- | |==| ,|==| , '='     |==| -   _ |        |==| _ , \ |==| -   _ |==| , '='     |==|  .=. \ _\==\ ,\ 
	--       |==|, ||==|_  . ,'./==/-  /\ - \|==|  /\ , |/==/\/ _ |==|-  |   |==|_  ,`-._|==|_  . ,'.        \==\-  ,    (|==|_  ,`-._|==|  /\ , |==|_  ,`-._|==|_  . ,'./==/-  /\ - \   |==|, | |==|- |\==\ -    ,_ /|==|  /\ , |        /==/  '\  ||==|  /\ , |\==\ -    ,_ //==/- '=' ,/==/\/ _ |
	--       /==/ -//==/  /\ ,  )==\ _.\=\.-'/==/, | |- |\==\ - , /==/   \   /==/ ,     //==/  /\ ,  )        /==/ _  ,  //==/ ,     //==/, | |- /==/ ,     //==/  /\ ,  )==\ _.\=\.-'   /==/ -/ /==/. / '.='. -   .' /==/, | |- |        \==\ /\=\.'/==/, | |- | '.='. -   .'|==|   -   /\==\ - , /
	--       `--`--``--`-`--`--' `--`        `--`./  `--` `--`---'`--`---'   `--`-----`` `--`-`--`--'         `--`------' `--`-----`` `--`./  `--`--`-----`` `--`-`--`--' `--`           `--`--` `--`-`    `--`--''   `--`./  `--`         `--`      `--`./  `--`   `--`--''  `-._`.___,'  `--`---' 

	DECLARE @senderAccountId BIGINT               = 0
	DECLARE @SenderAccountName NVARCHAR(100)      = 'Sender Name'
	--DECLARE @senderPayeScheme NVARCHAR(16)      = '123/SE12345'

    DECLARE @receiverAccountId BIGINT             = 1
	DECLARE @receiverAccountName NVARCHAR(100)    = 'Receiver Name'
	--DECLARE @receiverPayeScheme NVARCHAR(16)    = '123/RE12345'
	
    DECLARE @toDate DATETIME                      = GETDATE()
	declare @numberOfMonthsToCreate INT           = 25
	declare @defaultMonthlyTransfer DECIMAL(18,4) = 100

	declare @defaultNumberOfPaymentsPerMonth INT              = 1
	
	--  ,d88b.    ,  ,d88b.    ,  ,d88b.    ,  ,d88b.    ,  ,d88b.    ,  ,d88b.    ,  ,d88b.    ,  ,d88b.    ,  ,d88b.    ,  ,d88b.    ,  ,d88b.    ,  ,d88b.    ,  ,d88b.    ,  ,d88b.    ,  ,d88b.    ,  ,d88b.    ,  ,d88b.    ,  ,d88b.    ,  ,d88b.    ,  ,d88b.    ,  ,d88b.    ,  ,d88b.    ,  ,d88b.    ,  ,d88b.    ,
	-- '    `Y88P'  '    `Y88P'  '    `Y88P'  '    `Y88P'  '    `Y88P'  '    `Y88P'  '    `Y88P'  '    `Y88P'  '    `Y88P'  '    `Y88P'  '    `Y88P'  '    `Y88P'  '    `Y88P'  '    `Y88P'  '    `Y88P'  '    `Y88P'  '    `Y88P'  '    `Y88P'  '    `Y88P'  '    `Y88P'  '    `Y88P'  '    `Y88P'  '    `Y88P'  '    `Y88P' 

	DECLARE @paymentsByMonth TABLE (monthBeforeToDate INT, amount DECIMAL(18, 4), paymentsToGenerate INT, createMonth DATETIME)

-- generate defaults
insert into @paymentsByMonth
SELECT TOP (@numberOfMonthsToCreate)
			monthBeforeToDate = -@numberOfMonthsToCreate+ROW_NUMBER() OVER (ORDER BY [object_id]), 
			@defaultMonthlyTransfer,
			@defaultNumberOfPaymentsPerMonth,
			DATEADD(month,/*monthBeforeToDate*/ -@numberOfMonthsToCreate+ROW_NUMBER() OVER (ORDER BY [object_id]),@toDate)
FROM sys.all_objects
ORDER BY monthBeforeToDate;

-- override defaults here...
-- e.g. to create refunds set the amount -ve
--UPDATE @paymentsByMonth SET amount = -500, paymentsToGenerate = 1 where monthBeforeToDate = -1
--UPDATE @paymentsByMonth SET amount = -500, paymentsToGenerate = 1 where monthBeforeToDate = -7

select * from @paymentsByMonth

--todo: sproc that takes gen source table and generates payments

DECLARE @monthBeforeToDate INT = 1
DECLARE @createDate DATETIME
DECLARE @amount DECIMAL(18, 4)
DECLARE @paymentsToGenerate INT

WHILE (1 = 1) 
BEGIN  

  SELECT TOP 1 @monthBeforeToDate = monthBeforeToDate, @createDate = createMonth, @amount = amount, @paymentsToGenerate = paymentsToGenerate
  FROM @paymentsByMonth
  WHERE monthBeforeToDate < @monthBeforeToDate
  ORDER BY monthBeforeToDate DESC

  IF @@ROWCOUNT = 0 BREAK;

  exec DataGen.CreatePaymentAndTransferForMonth	@senderAccountId,   @senderAccountName,   -- @senderPayeScheme,
												@receiverAccountId, @receiverAccountName, -- @receiverPayeScheme,
												@createDate, @amount, @paymentsToGenerate
END
