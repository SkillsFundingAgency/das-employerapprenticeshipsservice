Feature: Agreements
	In order to allow spending on my account
	I want to be able to sign an ESFA agreement against a legal entity connected to my account

@mytag
Scenario: Owner Signs Agreement
	Given I am an account "Owner"
	When  I sign Agreement
	Then Agreement status is signed


Scenario Outline: Non Owner Signs Agreement
	Given I am an account "<accountRole>"
	When  I sign Agreement
	Then Agreement status is pending
	Examples: 
	| accountRole | status  |
	| Transactor  | Pending |
	| Viewer      | Pending |

