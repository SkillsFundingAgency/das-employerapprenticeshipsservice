Feature: Expired-funds

Scenario: Levy declarations, no expired levy declarations, no adjustment levy declarations, no payments, no refund payments, no transfer payments
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

Scenario: Levy declarations, expired levy declarations, adjustment levy declarations, payments, refund payments, transfer payments
	Given We have an account with a paye scheme
	And the account has transactions
		| TransactionType | Amount | DateCreated |
		| Declaration     | 1000   | 2018-04-23  |
		| Payment         | -10    | 2018-05-05  |
		| Declaration     | -100   | 2018-05-23  |
		| Payment         | -10    | 2018-06-05  |
		| Declaration     | 1000   | 2018-06-23  |
		| Payment         | -10    | 2018-07-05  |
		| Transfer        | -2     | 2018-07-05  |
		| Declaration     | 1000   | 2018-07-23  |
		| Payment         | -10    | 2018-08-05  |
		| Transfer        | -2     | 2018-08-05  |
		| Declaration     | 1000   | 2018-08-23  |
		| Payment         | -10    | 2018-09-05  |
		| Transfer        | -2     | 2018-09-05  |
		| Declaration     | 1000   | 2018-09-23  |
		| Payment         | -10    | 2018-10-05  |
		| Transfer        | -2     | 2018-10-05  |
		| Declaration     | 1000   | 2018-10-23  |
		| Payment         | -10    | 2018-11-05  |
		| Transfer        | -2     | 2018-11-05  |
		| Declaration     | 1000   | 2018-11-23  |
		| Payment         | -10    | 2018-12-05  |
		| Transfer        | -2     | 2018-12-05  |
		| Declaration     | 1000   | 2018-12-23  |
		| Payment         | -10    | 2019-01-05  |
		| Transfer        | -2     | 2019-01-05  |
		| Declaration     | 1000   | 2019-01-23  |
		| Payment         | -10    | 2019-02-05  |
		| Transfer        | -2     | 2019-02-05  |
		| Declaration     | -100   | 2019-02-23  |
		| Payment         | -10    | 2019-03-05  |
		| Transfer        | -2     | 2019-03-05  |
		| Declaration     | 1000   | 2019-03-23  |
		| Payment         | 10     | 2019-04-05  |
		| Transfer        | -2     | 2019-04-05  |
		| ExpiredFund     | -880   | 2019-03-28  |
		| Declaration     | 1000   | 2019-04-23  |
	When the expire funds process runs on 2019-04-28 with a 12 month expiry period
	Then we should see a level 1 screen with a balance of 8900 on the 04/2019
	And we should see a level 1 screen with expired levy of -888 on the 04/2019