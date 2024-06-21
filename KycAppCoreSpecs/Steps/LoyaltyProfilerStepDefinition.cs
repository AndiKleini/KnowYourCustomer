using FluentAssertions;
using KycAppCore;
using KycAppCore.Events;
using KycAppCore.OutPorts;
using KycAppCoreSpecs.TestAdapters;
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
        _scenarioContext.TryGetValue<IEnumerable<CustomerActivityEventBase>>(
            ActivityeventsKey, 
            out var registeredEvents);
        var loyaltyProfile = new LoyaltyProfile(new ActivityStreamTestAdapter(registeredEvents));
        loyaltyProfile.GenerateProfile(1);
        _scenarioContext.Add(EvaluationResultKey, loyaltyProfile.Points);
    }
    
    [Then("the value for the loyalty points is (.*)")]
    public void WhenTheLoyaltyProfileIsEvaluated(int expectedLoyaltyPoint)
    {
        _scenarioContext.TryGetValue<int>(EvaluationResultKey, out var result);
        result.Should().Be(expectedLoyaltyPoint);
    }

    [AfterScenario("Unregister activity events")]
    public void UnregisterActivityEvents()
    {
        _scenarioContext.Remove(ActivityeventsKey);
    }
    
    private void RecordActivityEvent(CustomerActivityEventBase activityEvent)
    {
        if (!_scenarioContext.ContainsKey(ActivityeventsKey))
        {
            _scenarioContext.Add(ActivityeventsKey, new List<CustomerActivityEventBase>());
        }
        var activityEvents = _scenarioContext.Get<IList<CustomerActivityEventBase>>(ActivityeventsKey);
        activityEvents.Add(activityEvent);
    }
}
