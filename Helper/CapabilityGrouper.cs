using apenew.Models;

namespace apenew.Helper;

public class GroupedCapabilitiesByFieldName
{
    /// <summary>The field to which these capabilities belong.</summary>
    public string Field { get; set; }

    /// <summary>The list of capabilities grouped by name within this field.</summary>
    public List<GroupedCapabilitiesByName> Capabilities { get; set; }
}

/// <summary>
/// Represents a grouping of capabilities sharing the same CapabilityName.
/// </summary>
public class GroupedCapabilitiesByName
{
    /// <summary>The common capability name.</summary>
    public string CapabilityName { get; set; }

    /// <summary>All Capability objects that share the same CapabilityName.</summary>
    public List<Capability> Items { get; set; }
}

/// <summary>
/// Provides helper methods to group Capability collections.
/// </summary>
public static class CapabilityGrouper
{
    /// <summary>
    /// Groups a flat list of capabilities first by Field, then within each field by CapabilityName.
    /// </summary>
    public static List<GroupedCapabilitiesByFieldName> GroupByFieldThenName(IEnumerable<Capability> capabilities)
    {
        return capabilities
            .GroupBy(c => c.Field)
            .Select(fieldGroup => new GroupedCapabilitiesByFieldName
            {
                Field = fieldGroup.Key,
                Capabilities = fieldGroup
                    .GroupBy(c => c.CapabilityName)
                    .Select(capGroup => new GroupedCapabilitiesByName
                    {
                        CapabilityName = capGroup.Key,
                        Items = capGroup.ToList()
                    })
                    .ToList()
            })
            .ToList();
    }
}