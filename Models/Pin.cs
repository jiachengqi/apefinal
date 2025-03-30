namespace apenew.Models;

public class Pin
{
    public string Id { get; set; } = Guid.NewGuid().ToString();
    public string AssessmentId { get; set; }
    public string CapabilityId { get; set; }
    public double X { get; set; }
    public double Y { get; set; }
    public double Width { get; set; }
    public double Height { get; set; }
    public Assessment? Assessment { get; set; }
    public Capability? Capability { get; set; }
    public int DisplayNumber { get; set; }
}