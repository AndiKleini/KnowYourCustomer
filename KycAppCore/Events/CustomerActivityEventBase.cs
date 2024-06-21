namespace KycAppCore.Events;

public record CustomerActivityEventBase(int CustomerId, DateTime ActivityTimeStamp);
