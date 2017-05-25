Feature: RemoveLegalEntity
	In order to remove connected organisations
	As a account owner
	I want to be able to remove legal entities from my account

@mytag
Scenario Outline: Remove legal entity when multiple exist on the account
	Given I am an account "<account_role>"
	When I have more than one legal entity with a "pending" status
	Then I "<result>" remove a legal entity
	Examples:
	| account_role | result   |
	| Owner        | can     |
	| Transactor   | cannot	|
	| Viewer       | cannot	|

Scenario: Unable to remove only legal entity on account
	Given I am an account "Owner"
	When There is only one legal entity on the account
	Then I "cannot" remove a legal entity

Scenario: Remove legal entity status
	Given I am an account "Owner"
	When I have more than one legal entity with a "signed" status
	Then I "cannot" remove a legal entity