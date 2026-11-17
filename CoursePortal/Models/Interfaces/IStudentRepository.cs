namespace CoursePortal.Models.Interfaces;

public interface IStudentRepository
{
    Task<IEnumerable<Student>> GetAllStudents();
    Task<Student?> GetStudentById(int id);
    Task<Student?> GetStudentByUserId(string userId);
    Task AddStudent(Student student);
    Task UpdateStudent(Student student);
    Task DeleteStudent(int id);
}
