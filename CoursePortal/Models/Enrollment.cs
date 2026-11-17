namespace CoursePortal.Models;

public class Enrollment
{
    public int EnrollmentId { get; set; }

    public int StudentId { get; set; }
    public int CourseId { get; set; }

    public DateTime EnrollmentDate { get; set; }
    public String Status { get; set; }
    public DateTime? CompletionDate { get; set; }
    public String Grade { get; set; }
}