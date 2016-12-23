Feature: TransactionLine
	In order to show details of my balance
	I want view transactions in and out for my account


Scenario: Transaction History levy declarations
	Given I have an account
	When I have the following submissions
		| Paye_scheme | LevyDueYtd | Payroll_Year | Payroll_Month | English_Fraction |
		| 223/ABC     | 1000       | 16-17        | 11            | 1                |
		| 223/ABC     | 1100       | 16-17        | 12            | 1                |
	Then the balance should be 1210 on the screen

Scenario: Transaction History levy declarations with multiple schemes
	Given I have an account
	When I have the following submissions
		| Paye_scheme | LevyDueYtd | Payroll_Year | Payroll_Month | English_Fraction |
		| 123/ABC     | 1000       | 16-17        | 11            | 1                |
		| 456/ABC     | 1000       | 16-17        | 11            | 1                |
		| 123/ABC     | 1100       | 16-17        | 12            | 1                |
	Then the balance should be 2310 on the screen

Scenario: Transaction History levy declarations over Payroll_year
	Given I have an account
	When I have the following submissions
		| Paye_scheme | LevyDueYtd | Payroll_Year | Payroll_Month | English_Fraction |
		| 323/ABC     | 1000       | 16-17        | 12            | 1                |
		| 323/ABC     | 100        | 17-18        | 01            | 1                |
	Then the balance should be 1100 on the screen

Scenario: Transaction History levy declarations and Payments
	Given I have an account
	When I have the following submissions
         | Paye_scheme | LevyDueYtd | Payroll_Year | Payroll_Month | English_Fraction |
         | 423/ABC     | 1000       | 17-18        | 01            | 1                |
         | 423/ABC     | 1100       | 17-18        | 02            | 1                |
	And I have the following payments
		| Payment_Amount | Payment_Type |
		| 100            | levy         |
		| 200            | cofund       |
	Then the balance should be 1110 on the screen
	
Scenario: Transaction History levy declarations late account registration in payroll year
	Given I have an account
	When I have the following submissions
		| Paye_scheme | LevyDueYtd | Payroll_Year | Payroll_Month | English_Fraction |
		| 423/ABC     | 1000       | 16-17        | 01            | 1                |
		| 423/ABC     | 2000       | 16-17        | 02            | 1                |
		| 423/ABC     | 3000       | 16-17        | 03            | 1                |
	And I register on DAS in month 03
	Then the balance should be 3300 on the screen

Scenario: Transaction History levy declarations late account registration in payroll year
	Given I have an account
	When I have the following submissions
		| Paye_scheme | LevyDueYtd | Payroll_Year | Payroll_Month | English_Fraction |
		| 425/ABC     | 1000       | 16-17        | 01            | 1                |
		| 424/ABC     | 100        | 16-17        | 01            | 1                |
		| 425/ABC     | 2000       | 16-17        | 02            | 1                |
		| 424/ABC     | 200        | 16-17        | 02            | 1                |	
	And I register on DAS in month 02
	Then the balance should be 2420 on the screen
