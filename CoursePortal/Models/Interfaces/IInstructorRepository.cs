namespace CoursePortal.Models.Interfaces;

public interface IInstructorRepository
{
    Task<IEnumerable<Instructor>> GetAllInstructors();
    Task<Instructor?> GetInstructorById(int id);
    Task AddInstructor(Instructor instructor);
    Task UpdateInstructor(Instructor instructor);
    Task DeleteInstructor(int id);
}
