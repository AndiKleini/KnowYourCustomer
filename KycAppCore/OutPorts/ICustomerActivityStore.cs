using KycAppCore.Events;

namespace KycAppCore.OutPorts;

public interface ICustomerActivityStore
{
    public IEnumerable<CustomerActivityEventBase> Consume(int customerId);
}