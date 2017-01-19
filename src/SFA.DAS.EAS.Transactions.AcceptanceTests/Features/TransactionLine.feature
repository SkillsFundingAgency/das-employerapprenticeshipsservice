Feature: TransactionLine
	In order to show details of my balance
	I want view transactions in and out for my account


Scenario T01: Transaction History levy declarations
	Given I have an account
	When I have the following submissions
		| Paye_scheme | LevyDueYtd | Payroll_Year | Payroll_Month | English_Fraction |
		| 223/ABC     | 1000       | 16-17        | 11            | 1                |
		| 223/ABC     | 1100       | 16-17        | 12            | 1                |
	Then the balance should be 1210 on the screen

Scenario T02: Transaction History levy declarations with multiple schemes
	Given I have an account
	When I have the following submissions
		| Paye_scheme | LevyDueYtd | Payroll_Year | Payroll_Month | English_Fraction |
		| 123/ABC     | 1000       | 16-17        | 11            | 1                |
		| 456/ABC     | 1000       | 16-17        | 11            | 1                |
		| 123/ABC     | 1100       | 16-17        | 12            | 1                |
	Then the balance should be 2310 on the screen

Scenario T04: Transaction History levy declarations over Payroll_year
	Given I have an account
	When I have the following submissions
		| Paye_scheme | LevyDueYtd | Payroll_Year | Payroll_Month | English_Fraction |
		| 323/ABC     | 1000       | 16-17        | 12            | 1                |
		| 323/ABC     | 100        | 17-18        | 01            | 1                |
	Then the balance should be 1210 on the screen

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
	
Scenario T03: Transaction History levy declarations late account registration in payroll year
	Given I have an account
	When I have the following submissions
		| Paye_scheme | LevyDueYtd | Payroll_Year | Payroll_Month | English_Fraction |
		| 423/ABC     | 1000       | 16-17        | 01            | 1                |
		| 423/ABC     | 2000       | 16-17        | 02            | 1                |
		| 423/ABC     | 3000       | 16-17        | 03            | 1                |
	And I register on DAS in month 03
	Then the balance should be 3300 on the screen

Scenario T05: Transaction History levy declarations late account registration in payroll year
	Given I have an account
	When I have the following submissions
		| Paye_scheme | LevyDueYtd | Payroll_Year | Payroll_Month | English_Fraction |
		| 425/ABC     | 1000       | 16-17        | 01            | 1                |
		| 424/ABC     | 100        | 16-17        | 01            | 1                |
		| 425/ABC     | 2000       | 16-17        | 02            | 1                |
		| 424/ABC     | 200        | 16-17        | 02            | 1                |	
	And I register on DAS in month 02
	Then the balance should be 2420 on the screen

Scenario T06: Transaction History levy declarations next year registration 
	Given I have an account
	When I have the following submissions
		| Paye_scheme | LevyDueYtd | Payroll_Year | Payroll_Month | English_Fraction |
		| 323/ABC     | 1000       | 16-17        | 12            | 1                |
		| 323/ABC     | 100        | 17-18        | 01            | 1                |
	And I register on DAS in year 16-17 month 12
	Then the balance should be 1210 on the screen

Scenario T07: Transaction History levy declarations next year registration multiple PAYE schemes
	Given I have an account
	When I have the following submissions
		| Paye_scheme | LevyDueYtd | Payroll_Year | Payroll_Month | English_Fraction |
		| 327/ABC     | 1000       | 16-17        | 12            | 1                |
		| 427/ABC     | 1000       | 16-17        | 12            | 1                |
		| 327/ABC     | 100        | 17-18        | 01            | 1                |
		| 427/ABC     | 100        | 17-18        | 01            | 1                |	
	And I register on DAS in year 16-17 month 01
	Then the balance should be 2200 on the screen
	
Scenario T08: Single PAYE scheme removed
	Given I have an account
	When I have the following submissions
		| Paye_scheme | LevyDueYtd | Payroll_Year | Payroll_Month | English_Fraction |
		| 328/ABC     | 1000       | 16-17        | 01            | 1                |
		| 328/ABC     | 1100       | 16-17        | 02            | 1                |
		| 328/ABC     | 1200       | 16-17        | 03            | 1                |	
	And I register on DAS in year 16-17 month 01
	And I remove the PAYE scheme in month 04
	Then the balance should be 1330 on the screen
