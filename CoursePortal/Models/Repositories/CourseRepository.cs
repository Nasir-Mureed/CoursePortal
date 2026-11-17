using CoursePortal.Models.Interfaces;
using Dapper;
using Microsoft.Data.Sqlite;

namespace CoursePortal.Models.Repositories;

public class CourseRepository : ICourseRepository
{
    private readonly string _connectionString;

    public CourseRepository(IConfiguration configuration)
    {
        _connectionString = configuration.GetConnectionString("DefaultConnection")!;
    }

    private SqliteConnection CreateConnection() => new(_connectionString);

    public async Task<IEnumerable<Course>> GetAllCourses()
    {
        await using var db = CreateConnection();
        return await db.QueryAsync<Course>("SELECT * FROM Courses ORDER BY CourseId DESC;");
    }

    public async Task<IEnumerable<Course>> GetActiveCourses()
    {
        await using var db = CreateConnection();
        return await db.QueryAsync<Course>(
            "SELECT * FROM Courses WHERE IsActive = 1 ORDER BY CourseId DESC;");
    }

    public async Task<Course?> GetCourseById(int id)
    {
        await using var db = CreateConnection();
        return await db.QueryFirstOrDefaultAsync<Course>(
            "SELECT * FROM Courses WHERE CourseId = @id;", new { id });
    }

    public async Task AddCourse(Course course)
    {
        await using var db = CreateConnection();
        const string sql = """
            INSERT INTO Courses
            (CourseName, CourseCode, Description, Credits, Duration, Category, IsActive, CreatedDate)
            VALUES
            (@CourseName, @CourseCode, @Description, @Credits, @Duration, @Category, @IsActive, @CreatedDate);
            """;
        await db.ExecuteAsync(sql, course);
    }

    public async Task UpdateCourse(Course course)
    {
        await using var db = CreateConnection();
        const string sql = """
            UPDATE Courses SET
                CourseName = @CourseName,
                CourseCode = @CourseCode,
                Description = @Description,
                Credits = @Credits,
                Duration = @Duration,
                Category = @Category,
                IsActive = @IsActive
            WHERE CourseId = @CourseId;
            """;
        await db.ExecuteAsync(sql, course);
    }

    public async Task DeleteCourse(int id)
    {
        await using var db = CreateConnection();
        await db.ExecuteAsync("DELETE FROM Courses WHERE CourseId = @id;", new { id });
    }
}
