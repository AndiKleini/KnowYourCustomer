using KycAppCore.Events;
using KycAppCore.OutPorts;

namespace KycAppCoreSpecs.TestAdapters;

public class ActivityStoreTestAdapter : ICustomerActivityStore
{
    private readonly List<CustomerActivityEventBase> activityEvents = new();

    public async Task<IEnumerable<CustomerActivityEventBase>> GetEventsFor(int customerId)
    {
        return await Task.FromResult(activityEvents);
    }

    public void Register(CustomerActivityEventBase activityEvent)
    {
        this.activityEvents.Add(activityEvent);
    }
}