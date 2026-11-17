namespace CoursePortal.Models;

public class Course
{
    public int CourseId { get; set; }

    public String CourseName { get; set; }
    public String CourseCode { get; set; }
    public String Description { get; set; }
    public int Credits { get; set; }
    public String Duration { get; set; }
    public String Category { get; set; }
    public bool IsActive { get; set; }
    public DateTime CreatedDate { get; set; }
}