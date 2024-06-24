using KycAppCore.Events;

namespace KycAppCore.OutPorts;

public interface ICustomerActivityStore
{
    public Task<IEnumerable<CustomerActivityEventBase>> GetEventsFor(int customerId);
}