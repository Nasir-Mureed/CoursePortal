namespace CoursePortal.Models.Interfaces;

public interface ICourseRepository
{
    Task<IEnumerable<Course>> GetAllCourses();
    Task<IEnumerable<Course>> GetActiveCourses();
    Task<Course?> GetCourseById(int id);
    Task AddCourse(Course course);
    Task UpdateCourse(Course course);
    Task DeleteCourse(int id);
}
