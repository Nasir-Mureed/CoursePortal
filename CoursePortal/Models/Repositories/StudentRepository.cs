using CoursePortal.Models.Interfaces;
using Dapper;
using Microsoft.Data.Sqlite;

namespace CoursePortal.Models.Repositories;

public class StudentRepository : IStudentRepository
{
    private readonly string _connectionString;

    public StudentRepository(IConfiguration configuration)
    {
        _connectionString = configuration.GetConnectionString("DefaultConnection")!;
    }

    private SqliteConnection CreateConnection() => new(_connectionString);

    public async Task<IEnumerable<Student>> GetAllStudents()
    {
        await using var db = CreateConnection();
        return await db.QueryAsync<Student>("SELECT * FROM Students ORDER BY StudentId DESC;");
    }

    public async Task<Student?> GetStudentById(int id)
    {
        await using var db = CreateConnection();
        return await db.QueryFirstOrDefaultAsync<Student>(
            "SELECT * FROM Students WHERE StudentId = @id;", new { id });
    }

    public async Task<Student?> GetStudentByUserId(string userId)
    {
        await using var db = CreateConnection();
        return await db.QueryFirstOrDefaultAsync<Student>(
            "SELECT * FROM Students WHERE UserId = @userId;", new { userId });
    }

    public async Task AddStudent(Student student)
    {
        await using var db = CreateConnection();
        const string sql = """
            INSERT INTO Students
            (UserId, Email, FirstName, LastName, DateOfBirth, Gender, PhoneNumber, Address, ProfilePicture, RegistrationDate)
            VALUES
            (@UserId, @Email, @FirstName, @LastName, @DateOfBirth, @Gender, @PhoneNumber, @Address, @ProfilePicture, @RegistrationDate);
            """;
        await db.ExecuteAsync(sql, student);
    }

    public async Task UpdateStudent(Student student)
    {
        await using var db = CreateConnection();
        const string sql = """
            UPDATE Students SET
                Email = @Email,
                FirstName = @FirstName,
                LastName = @LastName,
                DateOfBirth = @DateOfBirth,
                Gender = @Gender,
                PhoneNumber = @PhoneNumber,
                Address = @Address,
                ProfilePicture = @ProfilePicture
            WHERE StudentId = @StudentId;
            """;
        await db.ExecuteAsync(sql, student);
    }

    public async Task DeleteStudent(int id)
    {
        await using var db = CreateConnection();
        await db.ExecuteAsync("DELETE FROM Students WHERE StudentId = @id;", new { id });
    }
}
