Feature: TransactionLine
	In order to show details of my balance
	I want view transactions in and out for my account


Scenario: Transaction History levy declarations
	Given I have an account
	When I have the following submissions
		| LevyDueYtd  | Payrol_Year | Payrol_Month |
		| 1000		  | 17			| 12		   |
		| 100		  | 18			| 01		   |
	Then the balance should be 1100 on the screen

Scenario: Transaction History levy declarations and Payments
	Given I have an account
	When I have the following submissions
         | LevyDueYtd | Payrol_Year | Payrol_Month |
         |      1000  |       17      |        1      |
         |      1100  |       17      |        2      |
	And I have the following payments
		| Payment_Amount | Payment_Type |
		| 100            | levy         |
		| 200            | cofund       |
	Then the balance should be 1000 on the screen