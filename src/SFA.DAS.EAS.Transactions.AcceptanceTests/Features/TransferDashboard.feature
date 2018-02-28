Feature: TransferDashboard
	In order to review transfer details of my account
	I want view a summary of my account's transfers


Scenario: Transfer Allowance
	Given I have an account
	When I have the following submissions
		| Paye_scheme | LevyDueYtd | Payroll_Year | Payroll_Month | English_Fraction | SubmissionDate | CreatedDate |
		| 223/ABC     | 1000       | 16-17        | 10            | 1                | 2017-02-18     | 2017-02-23  |
		| 223/ABC     | 2000       | 16-17        | 11            | 1                | 2017-03-18     | 2017-03-23  |
		| 223/ABC     | 3000       | 16-17        | 12            | 1                | 2017-04-18     | 2017-04-23  |
		| 223/ABC     | 1000       | 17-18        | 1             | 1                | 2017-05-18     | 2017-05-23  |
		| 223/ABC     | 2000       | 17-18        | 2             | 1                | 2017-06-18     | 2017-06-23  |
		| 223/ABC     | 3000       | 17-18        | 3             | 1                | 2017-07-18     | 2017-07-23  |
	And The transfer allowance ratio is 10 percent
	Then the transfer allowance should be 330 on the transfer dashboard screen