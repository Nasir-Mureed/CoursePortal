using CoursePortal.Models.Interfaces;
using Dapper;
using Microsoft.Data.Sqlite;

namespace CoursePortal.Models.Repositories;

public class EnrollmentRepository : IEnrollmentRepository
{
    private readonly string _connectionString;

    public EnrollmentRepository(IConfiguration configuration)
    {
        _connectionString = configuration.GetConnectionString("DefaultConnection")!;
    }

    private SqliteConnection CreateConnection() => new(_connectionString);

    private const string DetailsSql = """
        SELECT e.EnrollmentId, e.StudentId, e.CourseId, e.EnrollmentDate, e.Status, e.Grade,
               c.CourseName, c.CourseCode, c.Category,
               TRIM(COALESCE(s.FirstName, '') || ' ' || COALESCE(s.LastName, '')) AS StudentName
        FROM Enrollments e
        INNER JOIN Courses c ON c.CourseId = e.CourseId
        INNER JOIN Students s ON s.StudentId = e.StudentId
        """;

    public async Task<IEnumerable<EnrollmentDetails>> GetAllEnrollments()
    {
        await using var db = CreateConnection();
        return await db.QueryAsync<EnrollmentDetails>(DetailsSql + " ORDER BY e.EnrollmentId DESC;");
    }

    public async Task<IEnumerable<EnrollmentDetails>> GetByStudentId(int studentId)
    {
        await using var db = CreateConnection();
        return await db.QueryAsync<EnrollmentDetails>(
            DetailsSql + " WHERE e.StudentId = @studentId ORDER BY e.EnrollmentId DESC;",
            new { studentId });
    }

    public async Task<Enrollment?> GetEnrollmentById(int id)
    {
        await using var db = CreateConnection();
        return await db.QueryFirstOrDefaultAsync<Enrollment>(
            "SELECT * FROM Enrollments WHERE EnrollmentId = @id;", new { id });
    }

    public async Task<bool> IsEnrolled(int studentId, int courseId)
    {
        await using var db = CreateConnection();
        var count = await db.ExecuteScalarAsync<int>(
            "SELECT COUNT(*) FROM Enrollments WHERE StudentId = @studentId AND CourseId = @courseId;",
            new { studentId, courseId });
        return count > 0;
    }

    public async Task AddEnrollment(Enrollment enrollment)
    {
        await using var db = CreateConnection();
        const string sql = """
            INSERT INTO Enrollments
            (StudentId, CourseId, EnrollmentDate, Status, CompletionDate, Grade)
            VALUES
            (@StudentId, @CourseId, @EnrollmentDate, @Status, @CompletionDate, @Grade);
            """;
        await db.ExecuteAsync(sql, enrollment);
    }

    public async Task UpdateEnrollment(Enrollment enrollment)
    {
        await using var db = CreateConnection();
        const string sql = """
            UPDATE Enrollments SET
                Status = @Status,
                CompletionDate = @CompletionDate,
                Grade = @Grade
            WHERE EnrollmentId = @EnrollmentId;
            """;
        await db.ExecuteAsync(sql, enrollment);
    }

    public async Task DeleteEnrollment(int id)
    {
        await using var db = CreateConnection();
        await db.ExecuteAsync("DELETE FROM Enrollments WHERE EnrollmentId = @id;", new { id });
    }
}
