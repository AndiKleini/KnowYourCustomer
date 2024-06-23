namespace KycAppCore.Events;

public record PurchaseEvent(int CustomerId, DateTime ActivityTimeStamp, int Amount) : 
    CustomerActivityEventBase(CustomerId, ActivityTimeStamp);