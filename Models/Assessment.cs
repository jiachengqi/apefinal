using System;
namespace apenew.Models
{
    public class Assessment
    {
        public string Id { get; set; } = Guid.NewGuid().ToString();
        public string Email { get; set; }
        public DateTime CreateAt { get; set; }
        public DateTime? SubmittedAt { get; set; }
        public string? SubmittedBy { get; set; }
        public DateTime? ReviewedAt { get; set; }
        public DateTime? StartReviewAt { get; set; }
        public string? ReviewedBy { get; set; }
        public string Status { get; set; }
        public string AssessmentType { get; set; }
        public string ApplicationID { get; set; }
        public string? Justification { get; set; }
        public string? SolutionDesignImagePath { get; set; }
        public List<Pin> Pins { get; set; } = new List<Pin>();
        
        public List<Capability> Capabilities { get; set; } = new List<Capability>();
    }
}

