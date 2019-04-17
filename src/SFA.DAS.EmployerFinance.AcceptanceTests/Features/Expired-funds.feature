Feature: Expired-funds

Scenario: Expired funds
	Given We have an account with a paye scheme
	And the account has transactions
		| TransactionType | Amount | DateCreated |
		| Declaration     | 1000   | 2018-04-23  |
		| Declaration     | 1000   | 2018-05-23  |
		| Declaration     | 1000   | 2018-06-23  |
		| Declaration     | 1000   | 2018-07-23  |
		| Declaration     | 1000   | 2018-08-23  |
		| Declaration     | 1000   | 2018-09-23  |
		| Declaration     | 1000   | 2018-10-23  |
		| Declaration     | 1000   | 2018-11-23  |
		| Declaration     | 1000   | 2018-12-23  |
		| Declaration     | 1000   | 2019-01-23  |
		| Declaration     | 1000   | 2019-02-23  |
		| Declaration     | 1000   | 2019-03-23  |
		| Declaration     | 1000   | 2019-04-23  |
	When the expire funds process runs on 2019-04-28 with a 12 month expiry period
	Then we should see a level 1 screen with a balance of 12000 on the 04/2019
	And we should see a level 1 screen with expired levy of -1000 on the 04/2019

Scenario: Expired funds after levy adjustments, payments and transfers
    Given We have an account with a paye scheme
	And we have period ends from 1718-R09 to 1920-R04
    And the account has transactions
		| TransactionType | Amount | DateCreated | PeriodEnd |
		| Declaration     | 1000   | 2018-04-23  |           |
		| Declaration     | -200   | 2018-05-23  |           |
		| Declaration     | 1000   | 2018-06-23  |           |
		| Declaration     | 1000   | 2018-07-23  |           |
		| Declaration     | 1000   | 2018-08-23  |           |
		| Declaration     | 1000   | 2018-09-23  |           |
		| Declaration     | -4500  | 2018-10-23  |           |
		| Declaration     | 1000   | 2018-11-23  |           |
		| Declaration     | 1000   | 2018-12-23  |           |
		| Declaration     | 1000   | 2019-01-23  |           |
		| Declaration     | 1000   | 2019-02-23  |           |
		| Declaration     | -400   | 2019-03-23  |           |
		| Declaration     | 1000   | 2019-04-23  |           |
		| Payment         | -10    | 2018-05-05  | 1718-R09  |
		| Payment         | 10     | 2018-06-05  | 1718-R10  |
		| Payment         | -10    | 2018-07-05  | 1718-R11  |
		| Payment         | -10    | 2018-08-05  | 1718-R12  |
		| Payment         | -10    | 2018-09-05  | 1819-R01  |
		| Payment         | -10    | 2018-10-05  | 1819-R02  |
		| Payment         | -10    | 2018-11-05  | 1819-R03  |
		| Payment         | -10    | 2018-12-05  | 1819-R04  |
		| Payment         | -10    | 2019-01-05  | 1819-R05  |
		| Payment         | -10    | 2019-02-05  | 1819-R06  |
		| Payment         | -10    | 2019-03-05  | 1819-R07  |
		| Payment         | -10    | 2019-04-05  | 1819-R08  |
		| Transfer        | -5     | 2019-01-05  | 1819-R05  |
		| Transfer        | -5     | 2019-02-05  | 1819-R06  |
		| Transfer        | 5      | 2019-03-05  | 1819-R07  |
		| Transfer        | -5     | 2019-04-05  | 1819-R08  |
	When the expire funds process runs on 2019-04-28 with a 12 month expiry period
	Then we should see a level 1 screen with a balance of 4600 on the 04/2019
	And we should see a level 1 screen with a total levy of 1000 on the 04/2019
	And we should see a level 1 screen with a total payment of -10 on the 04/2019
	And we should see a level 1 screen with a total transfer of -5 on the 04/2019
	And we should see a level 1 screen with expired levy of -190 on the 04/2019

Scenario: No expired funds after levy adjustments, payments, transfers and one month of previously expired funds
	Given We have an account with a paye scheme
	And we have period ends from 1718-R09 to 1920-R04
	And the account has transactions
		| TransactionType | Amount | DateCreated | PeriodEnd  |
		| Declaration     | 1000   | 2018-04-23  |            |
		| Declaration     | -200   | 2018-05-23  |            |
		| Declaration     | 1000   | 2018-06-23  |            |
		| Declaration     | 1000   | 2018-07-23  |            |
		| Declaration     | 1000   | 2018-08-23  |            |
		| Declaration     | 1000   | 2018-09-23  |            |
		| Declaration     | -4500  | 2018-10-23  |            |
		| Declaration     | 1000   | 2018-11-23  |            |
		| Declaration     | 1000   | 2018-12-23  |            |
		| Declaration     | 1000   | 2019-01-23  |            |
		| Declaration     | 1000   | 2019-02-23  |            |
		| Declaration     | -400   | 2019-03-23  |            |
		| Declaration     | 1000   | 2019-04-23  |            |
		| Declaration     | 1000   | 2019-05-23  |            |
		| Payment         | -10    | 2018-05-05  | 1718-R09   |
		| Payment	      | 10     | 2018-06-05  | 1718-R10   |
		| Payment         | -10    | 2018-07-05  | 1718-R11   |
		| Payment         | -10    | 2018-08-05  | 1718-R12   |
		| Payment         | -10    | 2018-09-05  | 1819-R01   |
		| Payment         | -10    | 2018-10-05  | 1819-R02   |
		| Payment         | -10    | 2018-11-05  | 1819-R03   |
		| Payment         | -10    | 2018-12-05  | 1819-R04   |
		| Payment         | -10    | 2019-01-05  | 1819-R05   |
		| Payment         | -10    | 2019-02-05  | 1819-R06   |
		| Payment         | -10    | 2019-03-05  | 1819-R07   |
		| Payment         | -10    | 2019-04-05  | 1819-R08   |
		| Payment         | -10    | 2019-05-05  | 1819-R09   |
		| Transfer        | -5     | 2019-01-05  | 1819-R05   |
		| Transfer        | -5     | 2019-02-05  | 1819-R06   |
		| Transfer        | 5      | 2019-03-05  | 1819-R07   |
		| Transfer        | -5     | 2019-04-05  | 1819-R08   |
		| Transfer        | -5     | 2019-05-05  | 1819-R09   |
		| ExpiredFund     |-190    | 2019-04-28  |            |
	When the expire funds process runs on 2019-05-28 with a 12 month expiry period
	Then we should see a level 1 screen with a balance of 5585 on the 05/2019
	And we should see a level 1 screen with a total levy of 1000 on the 05/2019
	And we should see a level 1 screen with a total payment of -10 on the 05/2019
	And we should see a level 1 screen with a total transfer of -5 on the 05/2019
	And we should see a level 1 screen with expired levy of 0 on the 05/2019

Scenario: Expired funds after levy adjustments, payments, transfers and one month of previously expired funds
	Given We have an account with a paye scheme
	And we have period ends from 1718-R09 to 1920-R04
	And the account has transactions
		| TransactionType | Amount | DateCreated | PeriodEnd |
		| Declaration     | 1000   | 2018-04-23  |           |
		| Declaration     | -200   | 2018-05-23  |           |
		| Declaration     | 1000   | 2018-06-23  |           |
		| Declaration     | 1000   | 2018-07-23  |           |
		| Declaration     | 1000   | 2018-08-23  |           |
		| Declaration     | 1000   | 2018-09-23  |           |
		| Declaration     | -4500  | 2018-10-23  |           |
		| Declaration     | 1000   | 2018-11-23  |           |
		| Declaration     | 1000   | 2018-12-23  |           |
		| Declaration     | 1000   | 2019-01-23  |           |
		| Declaration     | 1000   | 2019-02-23  |           |
		| Declaration     | -400   | 2019-03-23  |           |
		| Declaration     | 1000   | 2019-04-23  |           |
		| Declaration     | 1000   | 2019-05-23  |           |
		| Declaration     | 1000   | 2019-06-23  |           |
		| Declaration     | 1000   | 2019-07-23  |           |
		| Declaration     | 1000   | 2019-08-23  |           |
		| Declaration     | 1000   | 2019-09-23  |           |
		| Declaration     | 1000   | 2019-10-23  |           |
		| Declaration     | 1000   | 2019-11-23  |           |
		| Payment         | -10    | 2018-05-05  | 1718-R09  |
		| Payment         | 10     | 2018-06-05  | 1718-R10  |
		| Payment         | -10    | 2018-07-05  | 1718-R11  |
		| Payment         | -10    | 2018-08-05  | 1718-R12  |
		| Payment         | -10    | 2018-09-05  | 1819-R01  |
		| Payment         | -10    | 2018-10-05  | 1819-R02  |
		| Payment         | -10    | 2018-11-05  | 1819-R03  |
		| Payment         | -10    | 2018-12-05  | 1819-R04  |
		| Payment         | -10    | 2019-01-05  | 1819-R05  |
		| Payment         | -10    | 2019-02-05  | 1819-R06  |
		| Payment         | -10    | 2019-03-05  | 1819-R07  |
		| Payment         | -10    | 2019-04-05  | 1819-R08  |
		| Payment         | -10    | 2019-05-05  | 1819-R09  |
		| Payment         | -10    | 2019-06-05  | 1819-R10  |
		| Payment         | -10    | 2019-07-05  | 1819-R11  |
		| Payment         | -10    | 2019-08-05  | 1819-R12  |
		| Payment         | -10    | 2019-09-05  | 1920-R01  |
		| Payment         | -10    | 2019-10-05  | 1920-R02  |
		| Payment         | -10    | 2019-11-05  | 1920-R03  |
		| Transfer        | -5     | 2019-01-05  | 1819-R05  |
		| Transfer        | -5     | 2019-02-05  | 1819-R06  |
		| Transfer        | 5      | 2019-03-05  | 1819-R07  |
		| Transfer        | -5     | 2019-04-05  | 1819-R08  |
		| Transfer        | -5     | 2019-05-05  | 1819-R09  |
		| Transfer        | -5     | 2019-06-05  | 1819-R10  |
		| Transfer        | -5     | 2019-07-05  | 1819-R11  |
		| Transfer        | 5      | 2019-08-05  | 1819-R12  |
		| Transfer        | -5     | 2019-09-05  | 1920-R01  |
		| Transfer        | -5     | 2019-10-05  | 1920-R02  |
		| Transfer        | -5     | 2019-11-05  | 1920-R03  |
		| ExpiredFund     | -190   | 2019-04-28  |           |
	When the expire funds process runs on 2019-11-28 with a 12 month expiry period
	Then we should see a level 1 screen with a balance of 10600 on the 11/2019
	And we should see a level 1 screen with a total levy of 1000 on the 11/2019
	And we should see a level 1 screen with a total payment of -10 on the 11/2019
	And we should see a level 1 screen with a total transfer of -5 on the 11/2019
	And we should see a level 1 screen with expired levy of -905 on the 11/2019

Scenario: Expired funds after levy adjustments, payments, transfers and multiple months of previously expired funds
	Given We have an account with a paye scheme
	And we have period ends from 1718-R09 to 1920-R04
	And the account has transactions
		| TransactionType | Amount | DateCreated | PeriodEnd |
		| Declaration     | 1000   | 2018-04-23  |           |
		| Declaration     | -200   | 2018-05-23  |           |
		| Declaration     | 1000   | 2018-06-23  |           |
		| Declaration     | 1000   | 2018-07-23  |           |
		| Declaration     | 1000   | 2018-08-23  |           |
		| Declaration     | 1000   | 2018-09-23  |           |
		| Declaration     | -4500  | 2018-10-23  |           |
		| Declaration     | 1000   | 2018-11-23  |           |
		| Declaration     | 1000   | 2018-12-23  |           |
		| Declaration     | 1000   | 2019-01-23  |           |
		| Declaration     | 1000   | 2019-02-23  |           |
		| Declaration     | -400   | 2019-03-23  |           |
		| Declaration     | 1000   | 2019-04-23  |           |
		| Declaration     | 1000   | 2019-05-23  |           |
		| Declaration     | 1000   | 2019-06-23  |           |
		| Declaration     | 1000   | 2019-07-23  |           |
		| Declaration     | 1000   | 2019-08-23  |           |
		| Declaration     | 1000   | 2019-09-23  |           |
		| Declaration     | 1000   | 2019-10-23  |           |
		| Declaration     | 1000   | 2019-11-23  |           |
		| Declaration     | 1000   | 2019-12-23  |           |
		| Payment         | -10    | 2018-05-05  | 1718-R09  |
		| Payment         | 10     | 2018-06-05  | 1718-R10  |
		| Payment         | -10    | 2018-07-05  | 1718-R11  |
		| Payment         | -10    | 2018-08-05  | 1718-R12  |
		| Payment         | -10    | 2018-09-05  | 1819-R01  |
		| Payment         | -10    | 2018-10-05  | 1819-R02  |
		| Payment         | -10    | 2018-11-05  | 1819-R03  |
		| Payment         | -10    | 2018-12-05  | 1819-R04  |
		| Payment         | -10    | 2019-01-05  | 1819-R05  |
		| Payment         | -10    | 2019-02-05  | 1819-R06  |
		| Payment         | -10    | 2019-03-05  | 1819-R07  |
		| Payment         | -10    | 2019-04-05  | 1819-R08  |
		| Payment         | -10    | 2019-05-05  | 1819-R09  |
		| Payment         | -10    | 2019-06-05  | 1819-R10  |
		| Payment         | -10    | 2019-07-05  | 1819-R11  |
		| Payment         | -10    | 2019-08-05  | 1819-R12  |
		| Payment         | -10    | 2019-09-05  | 1920-R01  |
		| Payment         | -10    | 2019-10-05  | 1920-R02  |
		| Payment         | -10    | 2019-11-05  | 1920-R03  |
		| Payment         | -10    | 2019-12-05  | 1920-R04  |
		| Transfer        | -5     | 2019-01-05  | 1819-R05  |
		| Transfer        | -5     | 2019-02-05  | 1819-R06  |
		| Transfer        | 5      | 2019-03-05  | 1819-R07  |
		| Transfer        | -5     | 2019-04-05  | 1819-R08  |
		| Transfer        | -5     | 2019-05-05  | 1819-R09  |
		| Transfer        | -5     | 2019-06-05  | 1819-R10  |
		| Transfer        | -5     | 2019-07-05  | 1819-R11  |
		| Transfer        | 5      | 2019-08-05  | 1819-R12  |
		| Transfer        | -5     | 2019-09-05  | 1920-R01  |
		| Transfer        | -5     | 2019-10-05  | 1920-R02  |
		| Transfer        | -5     | 2019-11-05  | 1920-R03  |
		| Transfer        | -5     | 2019-12-05  | 1920-R04  |
		| ExpiredFund     | -190   | 2019-04-28  |           |
		| ExpiredFund     | -905   | 2019-11-28  |           |
	When the expire funds process runs on 2019-12-28 with a 12 month expiry period
	Then we should see a level 1 screen with a balance of 10600 on the 12/2019
	And we should see a level 1 screen with a total levy of 1000 on the 12/2019
	And we should see a level 1 screen with a total payment of -10 on the 12/2019
	And we should see a level 1 screen with a total transfer of -5 on the 12/2019
	And we should see a level 1 screen with expired levy of -985 on the 12/2019