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
                    SubControlID = cap.SubControlID,
                    CapabilityName = cap.CapabilityName,
                    Evidence = null,
                    SubcontrolDescription = cap.SubcontrolDescription,
                    Cluster = cap.Cluster,
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
                    (c.Checked == null || string.IsNullOrEmpty(c.Evidence))))
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
            assessment.Status = accept ? "Validated" : "Rejected";
            assessment.ReviewedAt = DateTime.UtcNow;
            assessment.ReviewedBy = reviewerEmail;
            assessment.Justification = justification;
            await _context.SaveChangesAsync();
        }
        
        public void LoadCapabilities()
{
    CapabilityLoader.AddCapability("IR1.2.1", "Define a repeatable risk assessment methodology."                         , "Security Operations"        , "Threat Modeling"                       , "Product");
    CapabilityLoader.AddCapability("IR1.2.2", "Align risk assessments with regulatory and compliance requirements."      , "Governance & Compliance"    , "Automated Compliance"                  , "Both");
    CapabilityLoader.AddCapability("IR2.2.1", "Identify business‑critical assets and processes requiring protection."    , "Recovery & Resilience"      , "Business Continuity & Asset Management", "Product");
    CapabilityLoader.AddCapability("IR2.4.1", "Implement logging and monitoring to detect data alterations."             , "Security Operations"        , "SIEM"                                  , "Product");

    CapabilityLoader.AddCapability("SD1.1.2", "Security requirements should be defined and documented for all new system developments." , "Governance & Compliance" , "Assessment Process Engine" , "Both");
    CapabilityLoader.AddCapability("SD1.2.1", "Development and production environments should be strictly segregated."                   , "Platform Security"        , "Environment Segmentation"   , "Both");
    CapabilityLoader.AddCapability("SD1.3.1", "Conduct regular security reviews as part of software testing processes."                  , "Code Security"            , "4‑eyes principle"           , "Product");
    CapabilityLoader.AddCapability("SD1.3.2", "Use automated security testing tools in development and testing phases."                  , "Code Security"            , "SAST & DAST"                , "Product");

    CapabilityLoader.AddCapability("SD2.1.1", "Security requirements should be formally documented in system specifications."            , "Security Operations"      , "Threat Modeling"             , "Product");
    CapabilityLoader.AddCapability("SD2.4.1", "Standardized security hardening guidelines should be followed for system builds."        , "Platform Security"        , "Hardened Configs"            , "Both");
    CapabilityLoader.AddCapability("SD2.4.2", "Systems should be patched and updated before deployment."                                 , "Platform Security"        , "Patch Management"           , "Both");
    CapabilityLoader.AddCapability("SD2.5.1", "Perform penetration testing on all critical systems before deployment."                   , "Security Operations"      , "Pen Testing"                , "Both");
    CapabilityLoader.AddCapability("SD2.5.2", "Automated security scanning should be integrated into CI/CD pipelines."                  , "Code Security"            , "SAST & DAST"                , "Both");
    CapabilityLoader.AddCapability("SD2.6.1", "Require peer review of all critical code for security vulnerabilities."                  , "Code Security"            , "4‑eyes principle"           , "Both");
    CapabilityLoader.AddCapability("SD2.10",  "Ensure secure decommissioning of obsolete systems."                                      , "Governance & Compliance"  , "Decomissioning Plan"        , "Both");
    
    CapabilityLoader.AddCapability("BA1.1.1", "Implement role‑based access controls for all business applications."                     , "Identity & Access Management", "RBAC"                     , "Both");
    CapabilityLoader.AddCapability("BA1.1.2", "Regularly update business applications to mitigate vulnerabilities."                      , "Code Security"            , "Patch Management"           , "Product");
    CapabilityLoader.AddCapability("BA1.1.3", "Conduct regular application security assessments and penetration testing."               , "Security Operations"      , "Pen Testing"                , "Product");
    CapabilityLoader.AddCapability("BA1.2.2", "Implement web application firewalls (WAF) to detect and block malicious activities."     , "Network Security"         , "WAF"                        , "Product");
    CapabilityLoader.AddCapability("BA1.2.3", "Perform regular vulnerability assessments and security testing on web applications."     , "Code Security"            , "SAST & DAST"                , "Product");
    CapabilityLoader.AddCapability("BA1.3.1", "Implement strong input validation to prevent SQL injection, XSS, and other threats."     , "Code Security"            , "Input Validation"           , "Product");
    CapabilityLoader.AddCapability("BA1.3.2", "Ensure error handling mechanisms do not expose sensitive system details."                , "Code Security"            , "Error Handling & Error Masking", "Product");
    CapabilityLoader.AddCapability("BA2.3.1", "Implement strong authentication and access controls for databases."                      , "Identity & Access Management", "RBAC"                   , "Product");
    
    CapabilityLoader.AddCapability("SA1.1.1", "Implement least privilege principles for user access."                                   , "Identity & Access Management", "IAM"                    , "Product");
    CapabilityLoader.AddCapability("SA1.1.2", "Require multi‑factor authentication (MFA) for critical systems."                        , "Identity & Access Management", "MFA"                    , "Product");
    CapabilityLoader.AddCapability("SA1.1.3", "Implement access logging and monitoring."                                               , "Security Operations"      , "Logging & Monitoring"       , "Both");
    CapabilityLoader.AddCapability("SA1.3.2", "Use role‑based access control (RBAC) or attribute‑based access control (ABAC)."          , "Identity & Access Management", "RBAC"                   , "Both");
    
    CapabilityLoader.AddCapability("TS1.2.2", "Regularly update and patch systems to mitigate malware vulnerabilities."                 , "Security Operations"      , "Vulnerability Remediation"  , "Product");
    CapabilityLoader.AddCapability("TS1.4.1", "Implement single sign‑on (SSO) and centralized identity management."                     , "Identity & Access Management", "SSO, Federation"       , "Product");
    CapabilityLoader.AddCapability("TS1.4.2", "Regularly audit and review identity access rights."                                      , "Identity & Access Management", "IDAM Audit and Governance", "Product");
    CapabilityLoader.AddCapability("TS1.6.2", "Enforce encryption policies for data at rest and in transit."                            , "Data Security"            , "Encryption/Secrets Management", "Both");
    CapabilityLoader.AddCapability("TS1.7.1", "Implement access restrictions and tracking for sensitive digital assets."                , "Identity & Access Management", "IAM"                    , "Both");
    CapabilityLoader.AddCapability("TS2.1.1", "Implement strong encryption standards for data at rest and in transit."                  , "Data Security"            , "Encryption"                , "Both");
    CapabilityLoader.AddCapability("TS2.1.2", "Avoid using deprecated cryptographic algorithms."                                        , "Data Security"            , "Encryption"                , "Both");
    CapabilityLoader.AddCapability("TS2.2.1", "Store encryption keys securely using hardware security modules (HSMs)."                  , "Data Security"            , "HSMs/Secure Storage"       , "Service");
    CapabilityLoader.AddCapability("TS2.2.2", "Regularly rotate cryptographic keys and enforce strong key management policies."         , "Data Security"            , "Secrets Management"         , "Both");
    
    CapabilityLoader.AddCapability("TM1.1.1", "Conduct regular vulnerability scans and assessments of systems and applications."        , "Security Operations"      , "Vulnerability Scanning"     , "Both");
    CapabilityLoader.AddCapability("TM1.1.2", "Apply security patches and updates promptly to mitigate known vulnerabilities."           , "Platform Security"        , "Patch Management & Automation", "Both");
    CapabilityLoader.AddCapability("TM1.2.1", "Implement centralized logging and monitoring solutions."                                 , "Security Operations"      , "SIEM"                       , "Both");
    CapabilityLoader.AddCapability("TM1.2.2", "Retain logs for an appropriate period for forensic investigations."                      , "Security Operations"      , "Log Retention"              , "Both");
    
    CapabilityLoader.AddCapability("IM1.1.2", "Require labeling and tagging of classified information."                                 , "Data Security"            , "Data Labeling & Tagging"     , "Both");
    CapabilityLoader.AddCapability("IM1.1.3", "Apply access controls based on classification levels."                                   , "Identity & Access Management", "Identity & Access Management", "Both");
    CapabilityLoader.AddCapability("IM1.2.2", "Require consent for collecting and processing personal data."                            , "Data Security"            , "Consent Management"          , "Product");
    CapabilityLoader.AddCapability("IM1.2.3", "Implement data subject rights processes (e.g., access, deletion, correction)."          , "Data Security"            , "DSR"                        , "Product");
    CapabilityLoader.AddCapability("IM2.1.2", "Encrypt sensitive documents stored digitally."                                           , "Data Security"            , "Encryption"                , "Both");
    
    CapabilityLoader.AddCapability("SY1.1.1", "Implement security hardening guidelines for all system installations."                   , "Platform Security"        , "Config Management"          , "Service");
    CapabilityLoader.AddCapability("SY1.1.2", "Conduct security assessments before deploying new systems."                              , "Governance & Compliance"  , "Vulnerability Scanning"      , "Service");
    CapabilityLoader.AddCapability("SY1.2.1", "Apply standardized security configurations to all servers."                              , "Platform Security"        , "IaC Security & Automation"   , "Service");
    CapabilityLoader.AddCapability("SY1.2.2", "Regularly audit and update server configurations."                                       , "Platform Security"        , "Configuration Audit & Drift Detection", "Both");
    CapabilityLoader.AddCapability("SY1.3.1", "Apply security patches and updates to virtual machines (VMs)."                           , "Platform Security"        , "Patch Management Automation", "Both");
    CapabilityLoader.AddCapability("SY1.4.2", "Implement strong access controls for shared storage systems."                            , "Identity & Access Management", "Access Control & Storage Security", "Both");
    CapabilityLoader.AddCapability("SY2.2.1", "Implement real‑time system performance monitoring."                                      , "Platform Security"        , "Monitoring & Observability Tools", "Service");
    CapabilityLoader.AddCapability("SY2.3.1", "Implement automated backup processes for critical data."                                 , "Recovery & Resilience"    , "Backup & DR solutions"       , "Both");
    CapabilityLoader.AddCapability("SY2.3.2", "Encrypt backup data and store copies in a secure location."                              , "Recovery & Resilience"    , "Backup Encryption"           , "Both");
    CapabilityLoader.AddCapability("SY2.4.2", "Require security impact assessments before implementing system changes."                 , "Risk Management"          , "Posture Management Tools"    , "Both");
    
    CapabilityLoader.AddCapability("NC1.1.1", "Implement documented configuration standards for routers, firewalls, and switches."      , "Network Security"         , "Config Management"           , "Service");
    CapabilityLoader.AddCapability("NC1.1.2", "Apply the principle of least privilege for network device access."                       , "Identity & Access Management", "RBAC for Network Devices",  "Product");
    CapabilityLoader.AddCapability("NC1.5.1", "Implement deny‑by‑default rules and allow only necessary traffic."                       , "Network Security"         , "Firewall, ACL"               , "Both");
    
    CapabilityLoader.AddCapability("SC2.1.1", "Establish cloud security governance aligned with internal risk management frameworks."    , "Governance & Compliance"  , "CSPM"                        , "Service");
    CapabilityLoader.AddCapability("SC2.1.2", "Implement cloud security monitoring and access controls."                                , "Platform Security"        , "Cloud Workload Protection"    , "Service");
    CapabilityLoader.AddCapability("SC2.2.1", "Enforce encryption for data stored in cloud services."                                   , "Data Security"            , "Cloud Encryption & Key Management", "Both");
    CapabilityLoader.AddCapability("SC2.2.2", "Require multi‑factor authentication (MFA) for cloud service access."                     , "Identity & Access Management", "Cloud MFA"                , "Service");
    
    CapabilityLoader.AddCapability("BC1.1.3", "Define recovery objectives, including RTOs and RPOs."                                    , "Recovery & Resilience"    , "Disaster Recovery Orchestration", "Product");
    CapabilityLoader.AddCapability("BC1.2.1", "Conduct regular Business Impact Analyses (BIA) to assess risk exposure."                 , "Risk Management"          , "Business Impact Assessment" , "Both");
    CapabilityLoader.AddCapability("BC1.3.1", "Implement redundant systems for critical applications."                                  , "Recovery & Resilience"    , "Cloud HA & Failover"         , "Both");
    CapabilityLoader.AddCapability("BC1.3.2", "Use geographically distributed data centers for disaster recovery."                      , "Recovery & Resilience"    , "Disaster Recover as a Service", "Both");
    CapabilityLoader.AddCapability("BC2.1.1", "Develop business continuity plans (BCP) for critical business functions."                , "Recovery & Resilience"    , "Business Continuity Management", "Product");
    CapabilityLoader.AddCapability("BC2.1.2", "Conduct regular reviews and updates to business continuity plans."                       , "Governance & Compliance"  , "BCP Testing"                , "Product");
    CapabilityLoader.AddCapability("BC2.3.1", "Conduct regular BCP tests, including tabletop exercises and simulations."                , "Governance & Compliance"  , "Disaster Recovery Simulations", "Both");
    
    CapabilityLoader.AddCapability("AS1.1.2", "Regularly evaluate security controls for effectiveness and compliance."                  , "Governance & Compliance"  , "Compliance Automation"       , "Both");
    CapabilityLoader.AddCapability("AS1.2.1", "Perform regular penetration testing on critical systems and applications."               , "Security Operations"      , "Penetration Testing"         , "Both");
    CapabilityLoader.AddCapability("AS1.2.2", "Utilize automated security scanning tools to identify vulnerabilities."                  , "Security Operations"      , "Vulnerability Scanning"      , "Both");
}

    }
    
    
}

