namespace CoursePortal.Models.Interfaces;

public interface IEnrollmentRepository
{
    Task<IEnumerable<EnrollmentDetails>> GetAllEnrollments();
    Task<IEnumerable<EnrollmentDetails>> GetByStudentId(int studentId);
    Task<Enrollment?> GetEnrollmentById(int id);
    Task<bool> IsEnrolled(int studentId, int courseId);
    Task AddEnrollment(Enrollment enrollment);
    Task UpdateEnrollment(Enrollment enrollment);
    Task DeleteEnrollment(int id);
}
