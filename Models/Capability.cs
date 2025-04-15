using System;
namespace apenew.Models
{
    public class Capability
    {
        public string Id { get; set; } = Guid.NewGuid().ToString();
        public string AssessmentId { get; set; }
        public string? Control { get; set; }
        public string? SubControlID { get; set; }
        public string? CapabilityName { get; set; }
        public string? Checked { get; set; }
        public string? Evidence { get; set; }

        public int? CapabilityId { get; set; }
        
        public string? SubcontrolDescription { get; set; }
        public string? Field { get; set; }
        public string? Domain { get; set; }
        public string? DanskeBankImplementation { get; set; }
        public string? Scope { get; set; }
        
        
        public Assessment? Assessment { get; set; }
    }
}