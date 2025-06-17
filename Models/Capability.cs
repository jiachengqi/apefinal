using System;
namespace apenew.Models
{
    public class Capability
    {
        public string Id { get; set; } = Guid.NewGuid().ToString();
        public string AssessmentId { get; set; }

        public string? SubControlID          { get; set; }  
        public string? SubcontrolDescription { get; set; }

        public string? CapabilityName { get; set; }          
        public string? Cluster        { get; set; }         

        public string? Checked  { get; set; }           
        public string? Evidence { get; set; }
        
        public string? ReviewerComment { get; set; } 
        public string? Scope                    { get; set; }

        public Assessment? Assessment { get; set; }
    }
}