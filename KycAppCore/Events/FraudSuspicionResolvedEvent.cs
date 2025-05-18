namespace KycAppCore.Events;

public record FraudSuspicionResolvedEvent(int CustomerUnderTestId, DateTime activitTimeStamp, int FraudSuspicionId) : 
    CustomerActivityEventBase(CustomerUnderTestId, activitTimeStamp);