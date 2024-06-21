using FluentAssertions;
using KycAppCore.Events;
using KycAppCore.OutPorts;
using NUnit.Framework;

namespace KycAppCoreSpecs.Steps;

[Binding]
public sealed class LoyaltyProfilerStepDefinition
{
    private const string EvaluationResultKey = "EvaluationResultKey";
    private const string ActivityeventsKey = "ActivityEvents";
    private readonly ScenarioContext _scenarioContext;

    public LoyaltyProfilerStepDefinition(ScenarioContext scenarioContext)
    {
        _scenarioContext = scenarioContext;
    }
    
    [Given("the customer signed up (.*) days ago")]
    public void TheCustomerSignedUpDaysAgo(int daysSinceSignUp)
    {
        this.RecordActivityEvent(new SignUpActivityEvent(1, DateTime.Now.AddDays(-daysSinceSignUp)));
    }

    [When(@"the loyalty profile is evaluated")]
    public void WhenTheLoyaltyProfileIsEvaluated()
    {
        // TODO create mock adapter
        // ActivityStreamProvider.SetStream();
        _scenarioContext.Add(EvaluationResultKey, 0);
    }
    
    [Then("the value for the loyalty points is (.*)")]
    public void WhenTheLoyaltyProfileIsEvaluated(int expectedLoyaltyPoint)
    {
        _scenarioContext.TryGetValue<int>(out var result);
        result.Should().Be(expectedLoyaltyPoint);
    }
    
    private void RecordActivityEvent(SignUpActivityEvent signUpEvent)
    {
        if (!_scenarioContext.ContainsKey(ActivityeventsKey))
        {
            _scenarioContext.Add(ActivityeventsKey, new List<CustomerActivityEventBase>());
        }
        var activityEvents = _scenarioContext.Get<IList<CustomerActivityEventBase>>(ActivityeventsKey);
        activityEvents.Add(signUpEvent);
    }
}
