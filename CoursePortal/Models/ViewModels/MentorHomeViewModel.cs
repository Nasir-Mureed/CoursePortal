namespace CoursePortal.Models.ViewModels;

public class MentorHomeViewModel
{
    public IEnumerable<Course> Courses { get; set; } = [];
    public IEnumerable<Student> Students { get; set; } = [];
    public IEnumerable<EnrollmentDetails> Enrollments { get; set; } = [];
}
