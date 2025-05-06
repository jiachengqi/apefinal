using apenew.Models;

namespace apenew.Helper;

public static class CapabilityLoader
{
    public static void AddCapability(
        string subControlId,
        string subcontrolDescription,
        string cluster,
        string capability,
        string scope)
    {
        var newCapability = new Capability
        {
            SubControlID          = subControlId,
            SubcontrolDescription = subcontrolDescription,
            Cluster               = cluster,
            CapabilityName        = capability,
            Scope                 = scope
        };

        if (scope.Equals("Product",  StringComparison.OrdinalIgnoreCase))  PredefinedCapabilities.ProductCapabilities.Add(newCapability);
        if (scope.Equals("Service",  StringComparison.OrdinalIgnoreCase))  PredefinedCapabilities.PlatformCapabilities.Add(newCapability);
        if (scope.Equals("Both",     StringComparison.OrdinalIgnoreCase)) {
            PredefinedCapabilities.ProductCapabilities.Add(newCapability);
            PredefinedCapabilities.PlatformCapabilities.Add(newCapability);
        }
    }

    public static void ProcessCapabilityData(IEnumerable<Dictionary<string, string>> dataRows)
    {
        foreach (var row in dataRows)
        {
            AddCapability(
                row["SubcontrolId"],
                row["SubcontrolDescription"],
                row["Cluster"],
                row["Capability"],
                row["Scope"]
            );
        }
    }
    
    public static class PredefinedCapabilities
    {
        public static readonly List<Capability> ProductCapabilities = new List<Capability>();
        public static readonly List<Capability> PlatformCapabilities = new List<Capability>();
    }
}
    
    