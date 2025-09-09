using apenew.Models;

namespace apenew.Services;

public interface IAssessmentService
{
    Task<List<Assessment>> GetAllAssessmentsAsync();
    Task<List<Assessment>> GetSubmittedAssessmentsAsync();
    Task<Assessment> GetAssessmentByIdAsync(string id);
    Task<List<Assessment>> GetAssessmentsByEmailAsync(string email);
    Task UpdateAssessmentAsync(Assessment assessment);
    Task UplpadAssessmentCapabilitiesFromJson(string assessmentId, List<Capability> uploadedCapabilities);
    Task CreateAssessmentAsync(Assessment assessment);
    Task DeleteAssessmentAsync(string assessmentId);
    Task ReviewAssessmentAsync(string id, string reviewerEmail, string justification, bool accept);
    Task SubmitAssessmentAsync(string id, string email);
}