using System;
using apenew.Models;
using Microsoft.EntityFrameworkCore;

namespace apenew.Services
{
    public class AssessmentService
    {
        private readonly ApplicationDbContext _context;

        public AssessmentService(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<List<Assessment>> GetAssessmentsByEmailAsync(string email)
        {
            return await _context.Assessments
                .Where(a => a.Email == email)
                .ToListAsync();
        }

        public async Task<List<Assessment>> GetSubmittedAssessmentsAsync()
        {
            return await _context.Assessments
                .Where(a => a.Status != "Draft")
                .ToListAsync();
        }

        public async Task<Assessment> GetAssessmentByIdAsync(string id)
        {
            return await _context.Assessments
                .Include(a => a.Capabilities)
                .Include(a => a.Pins) 
                .FirstOrDefaultAsync(a => a.Id == id);
        }

        public async Task CreateAssessmentAsync(Assessment assessment, string assessmentType)
        {
            assessment.CreateAt = DateTime.UtcNow;
            assessment.Status = "Draft";
            _context.Assessments.Add(assessment);
            await _context.SaveChangesAsync();

            var capabilities = assessmentType == "Product"
                ? PredefinedCapabilities.ProductCapabilities
                : PredefinedCapabilities.PlatformCapabilities;

            foreach (var cap in capabilities)
            {
                _context.Capabilities.Add(new Capability
                {
                    AssessmentId = assessment.Id,
                    Control = cap.Control,
                    SubControlID = cap.SubControlID,
                    CapabilityName = cap.CapabilityName,
                    Evidence = null
                });
            }
            await _context.SaveChangesAsync();
        }

        public async Task UpdateAssessmentAsync(Assessment assessment)
        {
            _context.Update(assessment);
            await _context.SaveChangesAsync();
        }

        public async Task SubmitAssessmentAsync(string id, string email)
        {
            var assessment = await GetAssessmentByIdAsync(id);
            if (assessment.Capabilities.Any(c => c.Checked == null || string.IsNullOrEmpty(c.Evidence)))
            {
                throw new Exception("all capabilities must be checked with evidence");
            }
            if (assessment.AssessmentType == "Product" && string.IsNullOrEmpty(assessment.SolutionDesignImagePath))
            {
                throw new Exception("solution Design Evidence is required for Product assessments");
            }
            assessment.Status = "Submitted";
            assessment.SubmittedAt = DateTime.UtcNow;
            assessment.SubmittedBy = email;
            await _context.SaveChangesAsync();
        }

        public async Task ReviewAssessmentAsync(string id, string reviewerEmail, string justification, bool accept)
        {
            var assessment = await GetAssessmentByIdAsync(id);
            assessment.Status = accept ? "Accepted" : "Rejected";
            assessment.ReviewedAt = DateTime.UtcNow;
            assessment.ReviewedBy = reviewerEmail;
            assessment.Justification = justification;
            await _context.SaveChangesAsync();
        }
    }

    public static class PredefinedCapabilities
    {
        public static readonly List<Capability> ProductCapabilities = new List<Capability>
        {
            new Capability { Control = "SD2.6", SubControlID = "SD2.6.1", CapabilityName = "Code Review & PR Scanning" },
            new Capability { Control = "PA2.3", SubControlID = "PA2.3.1", CapabilityName = "Performance Assessment" },
            new Capability { Control = "SD1.3", SubControlID = "SD1.3.1", CapabilityName = "Secure Design" },
            new Capability { Control = "IM1.1", SubControlID = "IM1.1.1", CapabilityName = "Incident Management" },
            new Capability { Control = "AS1.2", SubControlID = "AS1.2.1", CapabilityName = "Application Security" }
        };

        public static readonly List<Capability> PlatformCapabilities = new List<Capability>
        {
            new Capability { Control = "PA1.6", SubControlID = "PA1.6.1", CapabilityName = "CSPM" },
            new Capability { Control = "SC2.2", SubControlID = "SC2.2.1", CapabilityName = "SAST" },
            new Capability { Control = "SM2.4", SubControlID = "SM2.4.1", CapabilityName = "Data Encryption at Rest" },
            new Capability { Control = "SD2.7", SubControlID = "SD2.7.1", CapabilityName = "Logging" },
            new Capability { Control = "SY1.2", SubControlID = "SY1.2.1", CapabilityName = "Data Encryption in Transit" }
        };
    }
}

