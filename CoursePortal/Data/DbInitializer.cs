using Microsoft.AspNetCore.Identity;
using Microsoft.Data.Sqlite;

namespace CoursePortal.Data;

public static class DbInitializer
{
    // Create app tables + seed sample courses (not users)
    public static async Task InitializeAsync(IServiceProvider services)
    {
        var config = services.GetRequiredService<IConfiguration>();
        var connectionString = config.GetConnectionString("DefaultConnection")!;

        await using var connection = new SqliteConnection(connectionString);
        await connection.OpenAsync();

        var sql = """
            CREATE TABLE IF NOT EXISTS Categories (
                CategoryId INTEGER PRIMARY KEY AUTOINCREMENT,
                CategoryName TEXT NOT NULL,
                Description TEXT,
                IsActive INTEGER NOT NULL DEFAULT 1
            );

            CREATE TABLE IF NOT EXISTS Courses (
                CourseId INTEGER PRIMARY KEY AUTOINCREMENT,
                CourseName TEXT NOT NULL,
                CourseCode TEXT NOT NULL,
                Description TEXT,
                Credits INTEGER NOT NULL DEFAULT 0,
                Duration TEXT,
                Category TEXT,
                IsActive INTEGER NOT NULL DEFAULT 1,
                CreatedDate TEXT NOT NULL
            );

            CREATE TABLE IF NOT EXISTS Students (
                StudentId INTEGER PRIMARY KEY AUTOINCREMENT,
                UserId TEXT,
                Email TEXT,
                FirstName TEXT NOT NULL,
                LastName TEXT,
                DateOfBirth TEXT,
                Gender TEXT,
                PhoneNumber TEXT,
                Address TEXT,
                ProfilePicture TEXT,
                RegistrationDate TEXT NOT NULL
            );

            CREATE TABLE IF NOT EXISTS Instructors (
                InstructorId INTEGER PRIMARY KEY AUTOINCREMENT,
                FirstName TEXT NOT NULL,
                LastName TEXT,
                Email TEXT,
                PhoneNumber TEXT,
                Specialization TEXT,
                Bio TEXT,
                IsActive INTEGER NOT NULL DEFAULT 1
            );

            CREATE TABLE IF NOT EXISTS Enrollments (
                EnrollmentId INTEGER PRIMARY KEY AUTOINCREMENT,
                StudentId INTEGER NOT NULL,
                CourseId INTEGER NOT NULL,
                EnrollmentDate TEXT NOT NULL,
                Status TEXT NOT NULL,
                CompletionDate TEXT,
                Grade TEXT
            );
            """;

        await using (var cmd = connection.CreateCommand())
        {
            cmd.CommandText = sql;
            await cmd.ExecuteNonQueryAsync();
        }

        // Seed a few courses if the table is empty
        await using (var countCmd = connection.CreateCommand())
        {
            countCmd.CommandText = "SELECT COUNT(*) FROM Courses;";
            var count = Convert.ToInt32(await countCmd.ExecuteScalarAsync());
            if (count == 0)
            {
                await using var seedCmd = connection.CreateCommand();
                seedCmd.CommandText = """
                    INSERT INTO Categories (CategoryName, Description, IsActive) VALUES
                    ('Computer Science', 'CS courses', 1),
                    ('Software Engineering', 'SE courses', 1);

                    INSERT INTO Courses
                    (CourseName, CourseCode, Description, Credits, Duration, Category, IsActive, CreatedDate)
                    VALUES
                    ('Web Development', 'CS101', 'Build modern web apps', 3, '12 weeks', 'Computer Science', 1, datetime('now')),
                    ('Database Systems', 'CS201', 'SQL and data modeling', 3, '10 weeks', 'Computer Science', 1, datetime('now')),
                    ('Software Engineering', 'SE301', 'Design and delivery', 4, '14 weeks', 'Software Engineering', 1, datetime('now'));

                    INSERT INTO Instructors
                    (FirstName, LastName, Email, PhoneNumber, Specialization, Bio, IsActive)
                    VALUES
                    ('Alex', 'Morgan', 'alex@courseportal.com', '000-0000', 'Web & Databases', 'Senior mentor', 1);
                    """;
                await seedCmd.ExecuteNonQueryAsync();
            }
        }

        // Ensure Student and Mentor roles exist
        var roleManager = services.GetRequiredService<RoleManager<IdentityRole>>();
        foreach (var role in new[] { "Student", "Mentor" })
        {
            if (!await roleManager.RoleExistsAsync(role))
                await roleManager.CreateAsync(new IdentityRole(role));
        }
    }
}
