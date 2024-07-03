namespace KycAppCore.Events;

public record PurchaseEvent(int CustomerId, DateTime ActivityDate, int Amount) : 
    CustomerActivityEventBase(CustomerId, ActivityDate) { }