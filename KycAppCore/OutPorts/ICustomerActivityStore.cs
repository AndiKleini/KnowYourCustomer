using KycAppCore.Events;

namespace KycAppCore.OutPorts;

public interface ICustomerActivityStore
{
    public IEnumerable<CustomerActivityEventBase> GetEventsFor(int customerId);
}