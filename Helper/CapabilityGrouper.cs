using apenew.Models;

namespace apenew.Helper;

public static class CapabilityGrouper
{
    public static List<GroupedCapabilitiesByCluster> GroupByClusterThenName(IEnumerable<Capability> caps)
        => caps.GroupBy(c => c.Cluster)
            .Select(clusterGroup => new GroupedCapabilitiesByCluster
            {
                Cluster = clusterGroup.Key,
                Capabilities = clusterGroup.GroupBy(c => c.CapabilityName)
                    .Select(capGroup => new GroupedCapabilitiesByName
                    {
                        CapabilityName = capGroup.Key,
                        Items = capGroup.ToList()
                    })
                    .ToList()
            }).ToList();
}

public class GroupedCapabilitiesByCluster
{
    public string Cluster { get; set; }
    public List<GroupedCapabilitiesByName> Capabilities { get; set; }
}

public class GroupedCapabilitiesByName
{
    public string CapabilityName { get; set; }
    public List<Capability> Items { get; set; }
    
    public string UiChecked  
    {
        get => Items.FirstOrDefault()?.Checked;
        set
        {
            foreach (var c in Items) c.Checked = value;
        }
    }

    public string UiEvidence 
    {
        get => Items.FirstOrDefault()?.Evidence;
        set
        {
            foreach (var c in Items) c.Evidence = value;
        }
    }
}
