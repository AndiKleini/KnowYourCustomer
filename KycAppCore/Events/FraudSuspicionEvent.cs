namespace KycAppCore.Events;

public record FraudSuspicionEvent(int CustomerUnderTestId, DateTime AddDays, int FraudSuspicionId) : 
    CustomerActivityEventBase(CustomerUnderTestId, AddDays);