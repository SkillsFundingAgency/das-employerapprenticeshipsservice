Feature: PayeSchemes
	In order to manage Paye Schemes for my account
	As a user
	I want to be add and remove Paye Schemes

Scenario: Owner adds paye scheme
	Given user Dave registered
	And user Dave created account A
	And user Dave has role Owner for account A
	When user Dave adds paye scheme "123/456" to account A
	Then The an active PAYE scheme "123/456" is added to account A
	And The user is redirected to the next steps view
	And A Paye Scheme Added event is raised

Scenario: Owner removes paye scheme
	Given user Dave registered
	And user Dave created account A
	And user Dave has role Owner for account A
	And user Dave adds paye scheme "123/456" to account A
	When user Dave removes paye scheme "123/456" from account A
	Then The an active PAYE scheme "123/456" is removed from account A
	And The user is redirected to the paye index view
	And A Paye Scheme Removed event is raised

#Scenario: Viewer removes paye scheme
	#Given user Dave has role Owner for account A 
	#And user Dave adds paye scheme "123/456" to account A
	#And user Bob has role Viewer for account A
	#When user Bob removes paye scheme "123/456" from account A
	#Then The an PAYE scheme "123/456" is not removed from account A
	#And The user is redirected to the paye index view
	#And A Paye Scheme Removed event is raised

#Scenario: Transactor removes paye scheme
#	Given user Dave registered
#	And user Dave created account A
#	And user Dave has role Transactor for account A
#	And user Dave adds paye scheme "123/456" to account A
#	When user Dave removes paye scheme "123/456" from account A
#	Then The an active PAYE scheme "123/456" is removed from account A
#	And The user is redirected to the paye index view
#	And A Paye Scheme Removed event is raised
#
#Scenario Outline: Remove PAYE scheme 
#Given I am an account "<account_role>"
#When I remove a scheme
#Then Scheme is "<scheme_status>"
#Examples:
#| account_role | scheme_status |
#	| Owner        | removed       |
#	| Viewer       | not_removed   |
#	| Transactor   | not_removed  |