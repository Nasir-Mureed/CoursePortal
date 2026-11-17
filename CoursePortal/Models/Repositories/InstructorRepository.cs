using CoursePortal.Models.Interfaces;
using Dapper;
using Microsoft.Data.Sqlite;

namespace CoursePortal.Models.Repositories;

public class InstructorRepository : IInstructorRepository
{
    private readonly string _connectionString;

    public InstructorRepository(IConfiguration configuration)
    {
        _connectionString = configuration.GetConnectionString("DefaultConnection")!;
    }

    private SqliteConnection CreateConnection() => new(_connectionString);

    public async Task<IEnumerable<Instructor>> GetAllInstructors()
    {
        await using var db = CreateConnection();
        return await db.QueryAsync<Instructor>("SELECT * FROM Instructors ORDER BY InstructorId;");
    }

    public async Task<Instructor?> GetInstructorById(int id)
    {
        await using var db = CreateConnection();
        return await db.QueryFirstOrDefaultAsync<Instructor>(
            "SELECT * FROM Instructors WHERE InstructorId = @id;", new { id });
    }

    public async Task AddInstructor(Instructor instructor)
    {
        await using var db = CreateConnection();
        const string sql = """
            INSERT INTO Instructors
            (FirstName, LastName, Email, PhoneNumber, Specialization, Bio, IsActive)
            VALUES
            (@FirstName, @LastName, @Email, @PhoneNumber, @Specialization, @Bio, @IsActive);
            """;
        await db.ExecuteAsync(sql, instructor);
    }

    public async Task UpdateInstructor(Instructor instructor)
    {
        await using var db = CreateConnection();
        const string sql = """
            UPDATE Instructors SET
                FirstName = @FirstName,
                LastName = @LastName,
                Email = @Email,
                PhoneNumber = @PhoneNumber,
                Specialization = @Specialization,
                Bio = @Bio,
                IsActive = @IsActive
            WHERE InstructorId = @InstructorId;
            """;
        await db.ExecuteAsync(sql, instructor);
    }

    public async Task DeleteInstructor(int id)
    {
        await using var db = CreateConnection();
        await db.ExecuteAsync("DELETE FROM Instructors WHERE InstructorId = @id;", new { id });
    }
}
