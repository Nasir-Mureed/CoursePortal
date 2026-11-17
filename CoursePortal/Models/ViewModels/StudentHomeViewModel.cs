namespace CoursePortal.Models.ViewModels;

public class StudentHomeViewModel
{
    public Student? Student { get; set; }
    public IEnumerable<Course> AvailableCourses { get; set; } = [];
    public IEnumerable<EnrollmentDetails> MyEnrollments { get; set; } = [];
}
