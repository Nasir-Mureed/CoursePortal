namespace CoursePortal.Models;

public class Instructor
{
    public int InstructorId { get; set; }

    public String FirstName { get; set; }
    public String LastName { get; set; }
    public String Email { get; set; }
    public String PhoneNumber { get; set; }
    public String Specialization { get; set; }
    public String Bio { get; set; }
    public bool IsActive { get; set; }
}