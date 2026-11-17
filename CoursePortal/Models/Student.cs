namespace CoursePortal.Models;

public class Student
{
    public int StudentId { get; set; }

    // Links this student to ASP.NET Identity
    public string UserId { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;

    public string FirstName { get; set; } = string.Empty;
    public string LastName { get; set; } = string.Empty;
    public DateTime DateOfBirth { get; set; }
    public string Gender { get; set; } = string.Empty;
    public string PhoneNumber { get; set; } = string.Empty;
    public string Address { get; set; } = string.Empty;
    public string ProfilePicture { get; set; } = string.Empty;
    public DateTime RegistrationDate { get; set; }
}
