using apenew.Models;

namespace apenew.Helper;

public static class CapabilityLoader
{
    public static void AddCapability(
        string control,
        string subControlId,
        string subcontrolDescription,
        string type,
        string field,
        string domain,
        string capability,
        string danskeBankImplementation,
        string scope)
    {
        var newCapability = new Capability
        {
            Control = control,
            SubControlID = subControlId,
            SubcontrolDescription = subcontrolDescription,
            Field = field,
            Domain = domain,
            CapabilityName = capability,
            DanskeBankImplementation = danskeBankImplementation,
            Scope = scope
        };
        
        if (scope.Equals("Product", StringComparison.OrdinalIgnoreCase))
        {
            PredefinedCapabilities.ProductCapabilities.Add(newCapability);
        }
        else if (scope.Equals("Service", StringComparison.OrdinalIgnoreCase))
        {
            PredefinedCapabilities.PlatformCapabilities.Add(newCapability);
        }
        else if (scope.Equals("Both", StringComparison.OrdinalIgnoreCase))
        {
            PredefinedCapabilities.ProductCapabilities.Add(newCapability);
            PredefinedCapabilities.PlatformCapabilities.Add(newCapability);
        }
        else
        {
            throw new ArgumentException($"Scope value '{scope}' is not recognized.");
        }
    }

    public static void ProcessCapabilityData(IEnumerable<Dictionary<string, string>> dataRows)
    {
        foreach (var row in dataRows)
        {
            AddCapability(
                row["Control"],
                row["SubcontrolId"],
                row["SubcontrolDescription"],
                row["Type"],
                row["Field"],
                row["Domain"],
                row["Capability"],
                row["DanskeBankImplementation"],
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
    
    