using KycAppCore.Events;

namespace KycAppCore.OutPorts;

public interface ICustomerActivityStream
{
    public IObservable<CustomerActivityEventBase> Consume(int customerId);
}