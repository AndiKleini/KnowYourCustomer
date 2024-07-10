# Know your customer

The solution is a training and experimental area for TDD. 

## *Scenario 1:* Extending the calculation of loyalty points

Following feature is requested:

**As a marketing manager I want to make loyalty of purchasing customers visible, so that proper classifications for bonus promotions can be made.**

Actually customers who are recently purchasing a lot should get better bonus promotions.

For the first iteration the simple formula $P = P + 2M$ calculating the value of the promotion points should do the trick. $P$ is the variable of the loyalty points and $M$ is the overall amount the customer spent within the last 30 days.

Development, business and quality assuarance (the three amigos) are discussing the feature and as am outcome following scenario is created:

**Scenario:**\
Layoality Profile points are evaluating to 5 + X * 2 when registration happended longer than 365 days and purchases in the hight of X were made within the last 30 days.\

&nbsp;&nbsp;&nbsp;&nbsp;**Given** the customer signed up \<DaysPassedSinceSignUp\> days ago\
&nbsp;&nbsp;&nbsp;&nbsp;**And** the customer spent more than \<MoneySpent\> \<DaysAgo\> days ago\
&nbsp;&nbsp;&nbsp;&nbsp;**When** the loyalty profile is evaluated\
&nbsp;&nbsp;&nbsp;&nbsp;**Then** the value for the loyalty points is \<ExpectedLoyaltyPoints\>\

Examples:\
| DaysPassedSinceSignUp | MoneySpent | FromDaysAgo | DaysAgo | ExpectedLoyaltyPoints |\
| 500&emsp;&emsp;&emsp;&emsp;&emsp;&emsp;&emsp;&emsp;&emsp;&emsp;| 5000&emsp;&emsp;&emsp;&nbsp;&nbsp;| 30&emsp;&emsp;&emsp;&emsp;&emsp;&nbsp;&nbsp;| 0&emsp;&emsp;&emsp;&nbsp;&nbsp;         | 105 &emsp;&emsp;&emsp;&emsp;&emsp;&emsp;&emsp;&emsp;&nbsp;&nbsp;&nbsp;|

### *Step 1:* Add failing specflow test

1. After introducing the feature we add the spec to the feature file [LoyaltyProfiler.feature](KycAppCoreSpecs/Features/LoayalityProfiler.feature).
2. We can reuse already implemented bindings for most of the step definitions as those are already in place testing the previous feature of rewarding sign up.
3. We have to implement the step biding for the step **the customer spent more than \<MoneySpent\> \<DaysAgo\> days ago**. Therefore we create the proper method in [LoyaltyProfilerStepDefintion](KycAppCoreSpecs/Steps/LoayalityProfilerStepDefintion.cs).
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
> **_Note:_** The record (I prefer record here but feel free going with a class) PurchaseEvent didn't exist in the solution and was created during coding the test. Use means of your IDE to create the new type quickly and without leaving the specflow project KycAppCoreSpecs.

Let the tests run and show that the recently added one is red while the others are green.

### *Step 2:* Add business logic

1. Implement the logic of the feature. For finding the code for the business logic (imagine you are a newbee in this repo and you see it for the first time) look at the defintion for the **then**-step. This shows that the method **GenerateProfile** in the class [LoyaltyProfile](KycCoreApp/LoyaltyProfile.cs) is invoked. Below you can see one solution for adding the feature.
```csharp
public async Task GenerateProfile(int customerId)
    {
        DateTime? signUpDate = (await activityStore.GetEventsFor(customerId)).OfType<SignUpActivityEvent>()
            .FirstOrDefault(s => s.CustomerId == customerId)?.ActivityTimeStamp;
        if (signUpDate == null)
        {
            this.Error = ErrorCodes.UnknownCustomer;
        }
        else
        {
            this.Points += signUpDate <= DateTime.Now.AddYears(-1) ? PointsForSignupLongtimeAgo : 0;
        }

        // this block contains the new added code
        this.Points +=
            (await activityStore.GetEventsFor(customerId)).OfType<PurchaseEvent>()
            .Where(p => p.ActivityTimeStamp.Date > DateTime.Now.AddDays(-30)).
            Sum(p => p.Amount * 2 / 100);
    }
```
> **_Note:_** Do the minimal things that are necessary for getting the tests green.

Let the tests run again and show that all are green.

2. Refactor the code in the method **GenerateProfile** by calling **GetEventsFor** on **activityStore** only once (-> **_performance_**). Additionally substitute the expressions
```csharp
DateTime.Now.AddDays(-30) 
```
and
```csharp
DateTime.Now.AddDays(-1)
```
by properly named methods (-> **_clean code_**).
```csharp
 public async Task GenerateProfile(int customerId)
    {
        var customerActivityEventBases = 
            (await activityStore.GetEventsFor(customerId)).ToList();
        
        DateTime? signUpDate = customerActivityEventBases.OfType<SignUpActivityEvent>()
            .FirstOrDefault(s => s.CustomerId == customerId)?.ActivityTimeStamp;
        if (signUpDate == null)
        {
            this.Error = ErrorCodes.UnknownCustomer;
        }
        else
        {
            this.Points += signUpDate <= OneYearAgo() ? PointsForSignupLongtimeAgo : 0;
        }

        this.Points +=
            customerActivityEventBases.OfType<PurchaseEvent>()
            .Where(p => p.ActivityTimeStamp.Date > ThirtyDaysAgo()).
            Sum(p => p.Amount * 2 / 100);
    }
```
> **_Note:_** Let the tests run after each refactoring task in order to check whether something is broken or not.

### *Step 3:* Add a unit test for an edge case

Now we have to handle an edge case. The system accepts payments by installment as well, which is not covered by the current implementation. Instead of paying the whole amount the customer pays initially a commission and afterwards step by step the rest.\
From business point of view this corner case is not of any interest as with the already existing specs everything is clear. The money that was put on the table within the last 30 days has to be considered. Not more not less.\
We could now enhance our specs so that the edge case is covered as well, but as business told us this is not relevant for them, we are better of going with a unit test. Otherwise we would take the risk that specs are not considered as a useful documentation for business anylonger.\

1. Refactor a proper method for summation
2. Create unit tests for the new method
3. Implement the logic

## Takeaways ##
Let's summarize a few takeaways here.
* **Stay in the test code as much/long you can !**\
The test code is the first client code to your businesslogic ever and therefore it is a suitable place for design decisions regarding your APIs. During writing the tests you figure out what you really need and the risk of falling into the traps of speculative generality or YAGNI (You aint gonna need it) is lower. This approach may also have a huge impact on readability as well.\ Use
means of your IDE for easily creating methods, types and properties on demand (e.g.: generate method), create type ...), so that you are not distracted from the test code.
* **Switch between styles**\
There are different approaches of unit testing out in the field like London (mocking, white box, inside-out) or Detroit (Classic, black box, outside-in). Take a closer look at the philosophies behind those but see them as frameworks or tools. In the real world, by knowing the advantages and disadvantages, you can make use of all of them. It depends on the situation which style is currently more suitable and don't hesitate using both as well.
* **Talk the language of business**\
By creating scenarios in Gherkin adapt to language of your business. If you are already following a DDD approach this is easier. If not, you should al least avoid technical phrases or terms in your specs so that business feels comfortable with it. You should only change the specs when the business is changing. If you have to refactor your Gherkin test cases because of some technical changes in the background although the business did not change, your specs are not suitable for business documentation.
* **Tell me the problem not the solution**\
Some stakeholders tend to suggest or even order concrete solutions. During a valuable 3 amigos discussion make sure that the problem is understood and work together on a proper solution. As documentation for such a session you can use concrete scenarios.
* **Start with a failing test**\
Each change in your business logic starts with a failing test. Sometimes it is tempting to add quickly features or fixing bugs without writing a failing test first. If the tests remain green after the change you haven't covered this feature at all. Someone could remove it without breaking the tests.
* **Separate business logic from technical aspects**
Make use of architectural patterns (e.g.: ports and adapters) realizing a clear separation between business logic and technical topics. TDD works best when the code under test is not depending directly from infrastructure (e.g.: db access via stored procedure, http requests, file access ...) so that you can focus on implementing the business logic.


