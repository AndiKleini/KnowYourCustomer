# Know your customer

The solution is a training and experimental area for TDD. 

## Scenario 1 Extending the calculation of loyalty points

Following feature is requested:

**As a marketing manager I want to make loyalty of purchasing customers visible, so that proper classifications for bonus promotions can be made.**

Actually customers who are recently purchasing a lot should get better bonus promotions.

For the first iteration the simple formula $P = P + 2M$ calculating the value of the promotion points should do the trick. P is the variable of the loyalty points and M is the overall amount the customer spent within the last 30 days.

Development, business and quality assuarance are discussion the feature and following scenario is created:

Scenario: Layoality Profile points are evaluating to 5 + X * 2 when registration 
happended longer than 365 days and purchases in the hight of X were made within the last 30 days
Given the customer signed up <DaysPassedSinceSignUp> days ago
And the customer spent more than <MoneySpent> between <FromDaysAgo> and <ToDaysAgo> days ago
When the loyalty profile is evaluated
Then the value for the loyalty points is <ExpectedLoyaltyPoints>
Examples:
| DaysPassedSinceSignUp | MoneySpent | FromDaysAgo | ToDaysAgo | ExpectedLoyaltyPoints |
| 500                   | 5000       | 30          | 0         | 105                   |

### Steps

1.) After introducing the feature we add the spec to the feature file and create method. We also have to create 
record PurchaseEvent under usage of the IDE without leaving the test method.
```csharp
[Given("the customer spent more than (.*) between (.*) and (.*) days ago")]
public void TheCustomerSpentMoreThanMoneySpentBetweenFromDaysAgoAndToDaysAgo(
    int moneySpent, int fromDaysAgo, int toDaysAgo)
{
    var activityStoreTestAdapter = scenarioContext.Get<ActivityStoreTestAdapter>(ActivityStoreTestAdapterKey);
    activityStoreTestAdapter.Register(
        new PurchaseEvent(1, DateTime.Now.AddDays(toDaysAgo - 1), moneySpent));
}
```
Let the tests run and show that the new one is red.

2.) Implement the logic and get it green (shortest way)

3.) Refactor to parallel execution

4.) Add additional examples with a price that is not divisible by 1 euro and rerun (this will not break)

5.) Point out the problem with the Sum by substituting it with a first ?????? (maybe it is too much)

6.) Create Unit Test for the summation of events and implement refund event
    or maybe we implement dedicated IEnumarable extension method for summing up with more 
    restrictions -> we will write this only as additional information in the spec 
    fraud case when customer buys a lot of small things instead of a big one
    we can only count the max amount per day

```csharp
public static int SumUpPoints(this IEnumerable<PurchaseEvent> events)
    {
        return events.Sum(s => s.Amount);
    }
```



