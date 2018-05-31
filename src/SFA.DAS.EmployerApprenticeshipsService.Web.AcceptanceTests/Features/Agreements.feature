Feature: Agreements
	In order to allow spending on my account
	I want to be able to sign an ESFA agreement against a legal entity connected to my account

@mytag
Scenario: Owner Signs Agreements
	Given I am an account "Owner"
	When  I sign Agreement
	Then Agreement is signed


Scenario Outline: Sign Agreements
	Given I am an account "<accountRole>"
	When  I sign Agreement
	Then Agreement is pending
Examples: 
| accountRole |
| Transactor  | 
| Viewer      | 