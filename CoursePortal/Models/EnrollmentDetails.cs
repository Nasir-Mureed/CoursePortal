namespace CoursePortal.Models;

// Enrollment row with course/student names for lists
public class EnrollmentDetails
{
    public int EnrollmentId { get; set; }
    public int StudentId { get; set; }
    public string StudentName { get; set; } = string.Empty;
    public int CourseId { get; set; }
    public string CourseName { get; set; } = string.Empty;
    public string CourseCode { get; set; } = string.Empty;
    public string Category { get; set; } = string.Empty;
    public DateTime EnrollmentDate { get; set; }
    public string Status { get; set; } = string.Empty;
    public string Grade { get; set; } = string.Empty;
}
