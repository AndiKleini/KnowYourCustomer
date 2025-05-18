namespace KycAppCore.Events;

public record FraudSuspicionResolvedEvent(int CustomerUnderTestId, DateTime activitTimeStamp, object FraudSuspicionId) : 
    CustomerActivityEventBase(CustomerUnderTestId, activitTimeStamp);