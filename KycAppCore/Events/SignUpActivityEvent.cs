namespace KycAppCore.Events;

public record SignUpActivityEvent(int CustomerId, DateTime ActivityTimeStamp)  
    : CustomerActivityEventBase(CustomerId, ActivityTimeStamp) { }