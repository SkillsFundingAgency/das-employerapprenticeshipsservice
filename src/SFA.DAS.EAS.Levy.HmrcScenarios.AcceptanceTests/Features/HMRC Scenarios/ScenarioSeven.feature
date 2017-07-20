Feature: Scenario Seven - "No Payment for Period" & Ceased PAYE Scheme

Scenario: Balance should remain if no payment occurs
	Given I have an account
	And I add a PAYE Scheme 123/ABC with name Test Corp to the account
	When I get the following declarations from HMRC
		| Id | SubmissionId | PAYEScheme | LevyDueYTD | PayrollPeriod | PayrollMonth | SubmissionTime | CreatedDate | LevyAllowanceForFullYear | NoPaymentForPeriod | DateCeased |
		| 1  | 1            | 123/ABC    | 1000       | 17-18         | 1            | 2017-04-15     | 2017-04-23  | 15000                    |                    |            |
		| 2  | 2            | 123/ABC    | 2000       | 17-18         | 2            | 2017-05-15     | 2017-05-23  | 15000                    |     true           |            |		
	Then the balance on 06/2017 should be 1100 on the screen
	

Scenario: Balance should not be affected by past non payment months
	Given I have an account
	And I add a PAYE Scheme 123/ABC with name Test Corp to the account
	When I get the following declarations from HMRC
		| Id | SubmissionId | PAYEScheme | LevyDueYTD | PayrollPeriod | PayrollMonth | SubmissionTime | CreatedDate | LevyAllowanceForFullYear | NoPaymentForPeriod | DateCeased |
		| 1  | 1            | 123/ABC    | 1000       | 17-18         | 1            | 2017-04-15     | 2017-04-23  | 15000                    |                    |            |
		| 2  | 2            | 123/ABC    | 0          | 17-18         | 2            | 2017-05-15     | 2017-05-23  | 15000                    | true               |            |
		| 3  | 3            | 123/ABC    | 2000       | 17-18         | 3            | 2017-06-15     | 2017-06-23  | 15000                    |                    |            |
	Then the balance on 06/2017 should be 2200 on the screen