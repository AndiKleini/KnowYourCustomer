using FluentAssertions;
using NUnit.Framework;

namespace KycAppCoreSpecs.Steps;

[Binding]
public sealed class LoyaltyProfilerStepDefinition
{
    private const string DaysSinceSignupKey = "daysSinceSignUp";
    private const string EvaluationResultKey = "EvaluationResultKey";
    private readonly ScenarioContext _scenarioContext;

    public LoyaltyProfilerStepDefinition(ScenarioContext scenarioContext)
    {
        _scenarioContext = scenarioContext;
    }
    
    [Given("the customer signed up (.*) days ago")]
    public void GivenTheFirstNumberIs(int daysSinceSignUp)
    {
        _scenarioContext.Add(DaysSinceSignupKey, daysSinceSignUp);
    }
    
    [When(@"the loyalty profile is evaluated")]
    public void WhenTheLoyaltyProfileIsEvaluated()
    {
        _scenarioContext.Add(EvaluationResultKey, 0);
    }
    
    [Then("the value for the loyalty points is (.*)")]
    public void WhenTheLoyaltyProfileIsEvaluated(int expectedLoyaltyPoint)
    {
        _scenarioContext.TryGetValue<int>(out var result);
        result.Should().Be(expectedLoyaltyPoint);
    }
}