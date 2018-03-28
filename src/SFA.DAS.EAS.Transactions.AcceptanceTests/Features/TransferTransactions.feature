Feature: TransferTransactions


Scenario: Transfer Transactions
Given I have an account
When I add transfer transactions
Then They should appear in the transaction summary page
