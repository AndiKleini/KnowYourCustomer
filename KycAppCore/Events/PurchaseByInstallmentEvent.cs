using KycAppCore.Events;

namespace KycAppCore.Events;

public record PurchaseByInstallmentEvent(int CustomerId, DateTime ActivityDate, int Amount, int Commission) : 
    PurchaseEvent(CustomerId, ActivityDate, Amount);