using KycAppCore.Events;

namespace KycAppCore.OutPorts;

public static class ActivityStreamProvider
{
    private static ICustomerActivityStream? activityStream;

    public static void SetStream(ICustomerActivityStream customerActivityStream) => activityStream = customerActivityStream;

    internal static ICustomerActivityStream GetStream() => activityStream;
}