using System;
using apenew.Helper;
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

            LoadCapabilities();

            var capabilities = assessmentType == "Product"
                ? CapabilityLoader.PredefinedCapabilities.ProductCapabilities
                : CapabilityLoader.PredefinedCapabilities.PlatformCapabilities;

            foreach (var cap in capabilities)
            {
                _context.Capabilities.Add(new Capability
                {
                    AssessmentId = assessment.Id,
                    Control = cap.Control,
                    SubControlID = cap.SubControlID,
                    CapabilityName = cap.CapabilityName,
                    Evidence = null,
                    SubcontrolDescription = cap.SubcontrolDescription,
                    Field = cap.Field,
                    Domain = cap.Domain,
                    DanskeBankImplementation = cap.DanskeBankImplementation,
                    Scope = cap.Scope
                });
            }
            await _context.SaveChangesAsync();
            
            CapabilityLoader.PredefinedCapabilities.ProductCapabilities.Clear();
            CapabilityLoader.PredefinedCapabilities.PlatformCapabilities.Clear();

        }

        public async Task UpdateAssessmentAsync(Assessment assessment)
        {
            _context.Update(assessment);
            await _context.SaveChangesAsync();
        }

        public async Task SubmitAssessmentAsync(string id, string email)
        {
            var assessment = await GetAssessmentByIdAsync(id);
            if (assessment.Capabilities.Any(c => 
                    (c.Checked == null || string.IsNullOrEmpty(c.Evidence)) &&
                    c.DanskeBankImplementation != "No" &&
                    c.DanskeBankImplementation != "Not available"))
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
        
        public void LoadCapabilities()
    {
        List<Dictionary<string, string>> capabilityData = new List<Dictionary<string, string>>
            {
                // Row 1
                // new Dictionary<string, string>
                // {
                //     { "Control", "IR1.2" },
                //     { "SubcontrolId", "IR1.2.1" },
                //     { "SubcontrolDescription", "Define a repeatable risk assessment methodology." },
                //     { "Type", "Process" },
                //     { "Field", "Security Operations" },
                //     { "Domain", "Risk Management" },
                //     { "Capability", "Threat Modeling" },
                //     { "DanskeBankImplementation", "" },
                //     { "Scope", "Product" }
                // },
                // Row 2
                new Dictionary<string, string>
                {
                    { "Control", "IR1.2" },
                    { "SubcontrolId", "IR1.2.2" },
                    { "SubcontrolDescription", "Align risk assessments with regulatory and compliance requirements." },
                    { "Type", "Process" },
                    { "Field", "Governance & Compliance" },
                    { "Domain", "Risk Management" },
                    { "Capability", "Automated Compliance" },
                    { "DanskeBankImplementation", "" },
                    { "Scope", "Both" }
                },
                // Row 3
                new Dictionary<string, string>
                {
                    { "Control", "IR2.2" },
                    { "SubcontrolId", "IR2.2.1" },
                    { "SubcontrolDescription", "Identify business-critical assets and processes requiring protection." },
                    { "Type", "Process" },
                    { "Field", "Code Security" },
                    { "Domain", "Risk Management" },
                    { "Capability", "Input Validation" },
                    { "DanskeBankImplementation", "" },
                    { "Scope", "Product" }
                },
                // Row 4
                new Dictionary<string, string>
                {
                    { "Control", "IR2.4" },
                    { "SubcontrolId", "IR2.4.1" },
                    { "SubcontrolDescription", "Implement logging and monitoring to detect data alterations." },
                    { "Type", "Process" },
                    { "Field", "Security Operations" },
                    { "Domain", "Defensive Ops" },
                    { "Capability", "SIEM" },
                    { "DanskeBankImplementation", "" },
                    { "Scope", "Product" }
                },
                // Row 5
                new Dictionary<string, string>
                {
                    { "Control", "SD1.1" },
                    { "SubcontrolId", "SD1.1.2" },
                    { "SubcontrolDescription", "Security requirements should be defined and documented for all new system developments." },
                    { "Type", "Process" },
                    { "Field", "Security Operations" },
                    { "Domain", "Application Development Security" },
                    { "Capability", "Pen Testing" },
                    { "DanskeBankImplementation", "" },
                    { "Scope", "Both" }
                },
                // Row 6
                new Dictionary<string, string>
                {
                    { "Control", "SD1.2" },
                    { "SubcontrolId", "SD1.2.1" },
                    { "SubcontrolDescription", "Development and production environments should be strictly segregated." },
                    { "Type", "Process" },
                    { "Field", "Design & Engineering" },
                    { "Domain", "Secure Infrastructure Integrations" },
                    { "Capability", "Environment Segmentation" },
                    { "DanskeBankImplementation", "" },
                    { "Scope", "Both" }
                },
                // Row 7
                new Dictionary<string, string>
                {
                    { "Control", "SD1.2" },
                    { "SubcontrolId", "SD1.2.2" },
                    { "SubcontrolDescription", "Access to development systems should be restricted based on job roles." },
                    { "Type", "Technology" },
                    { "Field", "Identity & Access Management" },
                    { "Domain", "Identity &  Access Management" },
                    { "Capability", "IAM - RBAC" },
                    { "DanskeBankImplementation", "" },
                    { "Scope", "Both" }
                },
                // Row 8
                new Dictionary<string, string>
                {
                    { "Control", "SD1.3" },
                    { "SubcontrolId", "SD1.3.1" },
                    { "SubcontrolDescription", "Conduct regular security reviews as part of software testing processes." },
                    { "Type", "Process" },
                    { "Field", "Code Security" },
                    { "Domain", "Application Development Security" },
                    { "Capability", "4-eyes principle" },
                    { "DanskeBankImplementation", "" },
                    { "Scope", "Product" }
                },
                // Row 9
                new Dictionary<string, string>
                {
                    { "Control", "SD1.3" },
                    { "SubcontrolId", "SD1.3.2" },
                    { "SubcontrolDescription", "Use automated security testing tools in development and testing phases." },
                    { "Type", "Technology" },
                    { "Field", "Code Security" },
                    { "Domain", "Application Developmdent Security" },
                    { "Capability", "SAST" },
                    { "DanskeBankImplementation", "" },
                    { "Scope", "Product" }
                },
                // Row 10
                new Dictionary<string, string>
                {
                    { "Control", "SD2.1" },
                    { "SubcontrolId", "SD2.1.1" },
                    { "SubcontrolDescription", "Security requirements should be formally documented in system specifications." },
                    { "Type", "Process" },
                    { "Field", "Security Operations" },
                    { "Domain", "Application Development Security" },
                    { "Capability", "Logging & Monitoring" },
                    { "DanskeBankImplementation", "" },
                    { "Scope", "Product" }
                },
                // // Row 11
                // new Dictionary<string, string>
                // {
                //     { "Control", "SD2.2" },
                //     { "SubcontrolId", "SD2.2.2" },
                //     { "SubcontrolDescription", "Implement defense-in-depth security controls in system designs." },
                //     { "Type", "Technology" },
                //     { "Field", "Design & Engineering" },
                //     { "Domain", "Secure Infrastructure Integrations" },
                //     { "Capability", "Multiple" },
                //     { "DanskeBankImplementation", "" },
                //     { "Scope", "Both" }
                // },
                // // Row 12
                // new Dictionary<string, string>
                // {
                //     { "Control", "SD2.4" },
                //     { "SubcontrolId", "SD2.4.1" },
                //     { "SubcontrolDescription", "Standardized security hardening guidelines should be followed for system builds." },
                //     { "Type", "Process" },
                //     { "Field", "Design & Engineering" },
                //     { "Domain", "Secure Infrastructure Integrations" },
                //     { "Capability", "Hardened Configs" },
                //     { "DanskeBankImplementation", "Terraform Config" },
                //     { "Scope", "Both" }
                // },
                // // Row 13
                // new Dictionary<string, string>
                // {
                //     { "Control", "SD2.4" },
                //     { "SubcontrolId", "SD2.4.2" },
                //     { "SubcontrolDescription", "Systems should be patched and updated before deployment." },
                //     { "Type", "Technology" },
                //     { "Field", "Design & Engineering" },
                //     { "Domain", "Secure Infrastructure Integrations" },
                //     { "Capability", "Patch Management" },
                //     { "DanskeBankImplementation", "" },
                //     { "Scope", "Both" }
                // },
                // // Row 14
                // new Dictionary<string, string>
                // {
                //     { "Control", "SD2.5" },
                //     { "SubcontrolId", "SD2.5.1" },
                //     { "SubcontrolDescription", "Perform penetration testing on all critical systems before deployment." },
                //     { "Type", "Process" },
                //     { "Field", "Operational Security" },
                //     { "Domain", "Proactive Security" },
                //     { "Capability", "Pen Testing" },
                //     { "DanskeBankImplementation", "Annual Pen Test Report or date for one is scheduled" },
                //     { "Scope", "Both" }
                // },
                // // Row 15
                // new Dictionary<string, string>
                // {
                //     { "Control", "SD2.5" },
                //     { "SubcontrolId", "SD2.5.2" },
                //     { "SubcontrolDescription", "Automated security scanning should be integrated into CI/CD pipelines." },
                //     { "Type", "Technology" },
                //     { "Field", "Design & Engineering" },
                //     { "Domain", "Application Development Security" },
                //     { "Capability", "SAST & DAST" },
                //     { "DanskeBankImplementation", "" },
                //     { "Scope", "Both" }
                // },
                // // Row 16
                // new Dictionary<string, string>
                // {
                //     { "Control", "SD2.6" },
                //     { "SubcontrolId", "SD2.6.1" },
                //     { "SubcontrolDescription", "Require peer review of all critical code for security vulnerabilities." },
                //     { "Type", "Process" },
                //     { "Field", "Design & Engineering" },
                //     { "Domain", "Secure Infrastructure Integrations" },
                //     { "Capability", "4-eyes principle" },
                //     { "DanskeBankImplementation", "" },
                //     { "Scope", "Both" }
                // },
                // // Row 17
                // new Dictionary<string, string>
                // {
                //     { "Control", "SD2.6" },
                //     { "SubcontrolId", "SD2.6.2" },
                //     { "SubcontrolDescription", "Use automated static analysis tools for detecting security issues in code." },
                //     { "Type", "Technology" },
                //     { "Field", "Design & Engineering" },
                //     { "Domain", "Application Development Security" },
                //     { "Capability", "SAST" },
                //     { "DanskeBankImplementation", "" },
                //     { "Scope", "Both" }
                // },
                // // Row 18
                // new Dictionary<string, string>
                // {
                //     { "Control", "SD2.10" },
                //     { "SubcontrolId", "SD2.10" },
                //     { "SubcontrolDescription", "Ensure secure decommissioning of obsolete systems." },
                //     { "Type", "Process" },
                //     { "Field", "Design & Engineering" },
                //     { "Domain", "Backup & Resilience" },
                //     { "Capability", "Decomissioning Plan" },
                //     { "DanskeBankImplementation", "Show plan for decommissioning" },
                //     { "Scope", "Both" }
                // },
                // // Row 19
                // new Dictionary<string, string>
                // {
                //     { "Control", "BA1.1" },
                //     { "SubcontrolId", "BA1.1.1" },
                //     { "SubcontrolDescription", "Implement role-based access controls for all business applications." },
                //     { "Type", "Technology" },
                //     { "Field", "Design & Engineering" },
                //     { "Domain", "Identity & Access Management" },
                //     { "Capability", "RBAC" },
                //     { "DanskeBankImplementation", "" },
                //     { "Scope", "Both" }
                // },
                // // Row 20
                // new Dictionary<string, string>
                // {
                //     { "Control", "BA1.1" },
                //     { "SubcontrolId", "BA1.1.2" },
                //     { "SubcontrolDescription", "Regularly update business applications to mitigate vulnerabilities." },
                //     { "Type", "Technology" },
                //     { "Field", "Design & Engineering" },
                //     { "Domain", "Application Development Security" },
                //     { "Capability", "Patch Management" },
                //     { "DanskeBankImplementation", "" },
                //     { "Scope", "Product" }
                // },
                // // Row 21
                // new Dictionary<string, string>
                // {
                //     { "Control", "BA1.1" },
                //     { "SubcontrolId", "BA1.1.3" },
                //     { "SubcontrolDescription", "Conduct regular application security assessments and penetration testing." },
                //     { "Type", "Technology" },
                //     { "Field", "Operational Security" },
                //     { "Domain", "Proactive Security" },
                //     { "Capability", "SAST, DAST, Pen Testing" },
                //     { "DanskeBankImplementation", "" },
                //     { "Scope", "Product" }
                // },
                // // Row 22
                // new Dictionary<string, string>
                // {
                //     { "Control", "BA1.2" },
                //     { "SubcontrolId", "BA1.2.2" },
                //     { "SubcontrolDescription", "Implement web application firewalls (WAF) to detect and block malicious activities." },
                //     { "Type", "Technology" },
                //     { "Field", "Design & Engineering" },
                //     { "Domain", "Infrastructure, Network, and Platform Security" },
                //     { "Capability", "WAF" },
                //     { "DanskeBankImplementation", "" },
                //     { "Scope", "Product" }
                // },
                // // Row 23
                // new Dictionary<string, string>
                // {
                //     { "Control", "BA1.2" },
                //     { "SubcontrolId", "BA1.2.3" },
                //     { "SubcontrolDescription", "Perform regular vulnerability assessments and security testing on web applications." },
                //     { "Type", "Technology" },
                //     { "Field", "Operational Security" },
                //     { "Domain", "Proactive Security" },
                //     { "Capability", "SAST, DAST, Pen Testing" },
                //     { "DanskeBankImplementation", "" },
                //     { "Scope", "Product" }
                // },
                // // Row 24
                // new Dictionary<string, string>
                // {
                //     { "Control", "BA1.3" },
                //     { "SubcontrolId", "BA1.3.1" },
                //     { "SubcontrolDescription", "Implement strong input validation to prevent SQL injection, XSS, and other threats." },
                //     { "Type", "Technology" },
                //     { "Field", "Design & Engineering" },
                //     { "Domain", "Application Development Security" },
                //     { "Capability", "Input Validation" },
                //     { "DanskeBankImplementation", "" },
                //     { "Scope", "Product" }
                // },
                // // Row 25
                // new Dictionary<string, string>
                // {
                //     { "Control", "BA1.3" },
                //     { "SubcontrolId", "BA1.3.2" },
                //     { "SubcontrolDescription", "Ensure error handling mechanisms do not expose sensitive system details." },
                //     { "Type", "Technology" },
                //     { "Field", "Design & Engineering" },
                //     { "Domain", "Application Development Security" },
                //     { "Capability", "Error Handling & Error Masking" },
                //     { "DanskeBankImplementation", "" },
                //     { "Scope", "Product" }
                // },
                // // Row 26
                // new Dictionary<string, string>
                // {
                //     { "Control", "BA2.3" },
                //     { "SubcontrolId", "BA2.3.1" },
                //     { "SubcontrolDescription", "Implement strong authentication and access controls for databases." },
                //     { "Type", "Technology" },
                //     { "Field", "Design & Engineering" },
                //     { "Domain", "Identity & Access Management" },
                //     { "Capability", "RBAC" },
                //     { "DanskeBankImplementation", "Reference FT access" },
                //     { "Scope", "Product" }
                // },
                // // Row 27
                // new Dictionary<string, string>
                // {
                //     { "Control", "SA1.1" },
                //     { "SubcontrolId", "SA1.1.1" },
                //     { "SubcontrolDescription", "Implement least privilege principles for user access." },
                //     { "Type", "Process" },
                //     { "Field", "Design & Engineering" },
                //     { "Domain", "Identity & Access Management" },
                //     { "Capability", "IAM" },
                //     { "DanskeBankImplementation", "" },
                //     { "Scope", "Product" }
                // },
                // // Row 28
                // new Dictionary<string, string>
                // {
                //     { "Control", "SA1.1" },
                //     { "SubcontrolId", "SA1.1.2" },
                //     { "SubcontrolDescription", "Require multi-factor authentication (MFA) for critical systems." },
                //     { "Type", "Technology" },
                //     { "Field", "Design & Engineering" },
                //     { "Domain", "Identity & Access Management" },
                //     { "Capability", "MFA" },
                //     { "DanskeBankImplementation", "" },
                //     { "Scope", "Product" }
                // },
                // // Row 29
                // new Dictionary<string, string>
                // {
                //     { "Control", "SA1.1" },
                //     { "SubcontrolId", "SA1.1.3" },
                //     { "SubcontrolDescription", "Implement access logging and monitoring." },
                //     { "Type", "Technology" },
                //     { "Field", "Operational Security" },
                //     { "Domain", "Defensive Ops" },
                //     { "Capability", "Logging & Monitoring" },
                //     { "DanskeBankImplementation", "" },
                //     { "Scope", "Both" }
                // },
                // // Row 30
                // new Dictionary<string, string>
                // {
                //     { "Control", "SA1.3" },
                //     { "SubcontrolId", "SA1.3.2" },
                //     { "SubcontrolDescription", "Use role-based access control (RBAC) or attribute-based access control (ABAC)." },
                //     { "Type", "Technology" },
                //     { "Field", "Design & Engineering" },
                //     { "Domain", "Identity & Access Management" },
                //     { "Capability", "RBAC" },
                //     { "DanskeBankImplementation", "" },
                //     { "Scope", "Both" }
                // },
                // // Row 31
                // new Dictionary<string, string>
                // {
                //     { "Control", "TS1.1" },
                //     { "SubcontrolId", "TS1.1.2" },
                //     { "SubcontrolDescription", "Implement layered security controls based on a defense-in-depth approach." },
                //     { "Type", "Technology" },
                //     { "Field", "Design & Engineering" },
                //     { "Domain", "Infrastructure, Network, and Platform Security" },
                //     { "Capability", "" },
                //     { "DanskeBankImplementation", "" },
                //     { "Scope", "Both" }
                // },
                // // Row 32
                // new Dictionary<string, string>
                // {
                //     { "Control", "TS1.2" },
                //     { "SubcontrolId", "TS1.2.2" },
                //     { "SubcontrolDescription", "Regularly update and patch systems to mitigate malware vulnerabilities." },
                //     { "Type", "Technology" },
                //     { "Field", "Operational Security" },
                //     { "Domain", "Defensive Ops" },
                //     { "Capability", "Vulnerability Remediation" },
                //     { "DanskeBankImplementation", "" },
                //     { "Scope", "Product" }
                // },
                // // Row 33
                // new Dictionary<string, string>
                // {
                //     { "Control", "TS1.4" },
                //     { "SubcontrolId", "TS1.4.1" },
                //     { "SubcontrolDescription", "Implement single sign-on (SSO) and centralized identity management." },
                //     { "Type", "Technology" },
                //     { "Field", "Design & Engineering" },
                //     { "Domain", "Identity & Access Management" },
                //     { "Capability", "SSO, Federation" },
                //     { "DanskeBankImplementation", "" },
                //     { "Scope", "Product" }
                // },
                // // Row 34
                // new Dictionary<string, string>
                // {
                //     { "Control", "TS1.4" },
                //     { "SubcontrolId", "TS1.4.2" },
                //     { "SubcontrolDescription", "Regularly audit and review identity access rights." },
                //     { "Type", "Process" },
                //     { "Field", "Operational Security" },
                //     { "Domain", "Defensive Ops" },
                //     { "Capability", "IDAM Audit and Governance" },
                //     { "DanskeBankImplementation", "" },
                //     { "Scope", "Product" }
                // },
                // // Row 35
                // new Dictionary<string, string>
                // {
                //     { "Control", "TS1.6" },
                //     { "SubcontrolId", "TS1.6.2" },
                //     { "SubcontrolDescription", "Enforce encryption policies for data at rest and in transit." },
                //     { "Type", "Technology" },
                //     { "Field", "Design & Engineering" },
                //     { "Domain", "Data Security" },
                //     { "Capability", "Encryption/Secrets Management" },
                //     { "DanskeBankImplementation", "" },
                //     { "Scope", "Both" }
                // },
                // // Row 36
                // new Dictionary<string, string>
                // {
                //     { "Control", "TS1.7" },
                //     { "SubcontrolId", "TS1.7.1" },
                //     { "SubcontrolDescription", "Implement access restrictions and tracking for sensitive digital assets." },
                //     { "Type", "Technology" },
                //     { "Field", "Design & Engineering" },
                //     { "Domain", "Data Security" },
                //     { "Capability", "IAM" },
                //     { "DanskeBankImplementation", "" },
                //     { "Scope", "Both" }
                // },
                // // Row 37
                // new Dictionary<string, string>
                // {
                //     { "Control", "TS2.1" },
                //     { "SubcontrolId", "TS2.1.1" },
                //     { "SubcontrolDescription", "Implement strong encryption standards for data at rest and in transit." },
                //     { "Type", "Technology" },
                //     { "Field", "Design & Engineering" },
                //     { "Domain", "Data Security" },
                //     { "Capability", "Encryption" },
                //     { "DanskeBankImplementation", "" },
                //     { "Scope", "Both" }
                // },
                // // Row 38
                // new Dictionary<string, string>
                // {
                //     { "Control", "TS2.1" },
                //     { "SubcontrolId", "TS2.1.2" },
                //     { "SubcontrolDescription", "Avoid using deprecated cryptographic algorithms." },
                //     { "Type", "Technology" },
                //     { "Field", "Design & Engineering" },
                //     { "Domain", "Data Security" },
                //     { "Capability", "Encryption" },
                //     { "DanskeBankImplementation", "" },
                //     { "Scope", "Both" }
                // },
                // // Row 39
                // new Dictionary<string, string>
                // {
                //     { "Control", "TS2.2" },
                //     { "SubcontrolId", "TS2.2.1" },
                //     { "SubcontrolDescription", "Store encryption keys securely using hardware security modules (HSMs)." },
                //     { "Type", "Technology" },
                //     { "Field", "Design & Engineering" },
                //     { "Domain", "Data Security" },
                //     { "Capability", "HSMs/Secure Storage" },
                //     { "DanskeBankImplementation", "" },
                //     { "Scope", "Service" }
                // },
                // // Row 40
                // new Dictionary<string, string>
                // {
                //     { "Control", "TS2.2" },
                //     { "SubcontrolId", "TS2.2.2" },
                //     { "SubcontrolDescription", "Regularly rotate cryptographic keys and enforce strong key management policies." },
                //     { "Type", "Process" },
                //     { "Field", "Strategy & Vision" },
                //     { "Domain", "Security Governance" },
                //     { "Capability", "Secrets Management" },
                //     { "DanskeBankImplementation", "" },
                //     { "Scope", "Both" }
                // }
            };
            
            CapabilityLoader.ProcessCapabilityData(capabilityData);
    }
    }
    
    
}

