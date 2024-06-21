Feature: LoyaltyProfiler
Creates loayality profiles for customers

Link to a feature: [Calculator]($projectname$/Features/LoyaltyProfiler.feature)

@mytag
Scenario: Layoality Profile evaluates to zero when registration happended within the last 365 days
	Given the customer signed up 1 days ago
	When the loyalty profile is evaluated
	Then the value for the loyalty points is 0

@mytag
Scenario: Layoality Profile evaluates to five when registration happended longer than 365 days ago
	Given the customer signed up 500 days ago
	When the loyalty profile is evaluated
	Then the value for the loyalty points is 5