using CoursePortal.Models.Interfaces;
using Dapper;
using Microsoft.Data.Sqlite;

namespace CoursePortal.Models.Repositories;

public class CategoryRepository : ICategoryRepository
{
    private readonly string _connectionString;

    public CategoryRepository(IConfiguration configuration)
    {
        _connectionString = configuration.GetConnectionString("DefaultConnection")!;
    }

    private SqliteConnection CreateConnection() => new(_connectionString);

    public async Task<IEnumerable<Category>> GetAllCategories()
    {
        await using var db = CreateConnection();
        return await db.QueryAsync<Category>("SELECT * FROM Categories ORDER BY CategoryId;");
    }

    public async Task<Category?> GetCategoryById(int id)
    {
        await using var db = CreateConnection();
        return await db.QueryFirstOrDefaultAsync<Category>(
            "SELECT * FROM Categories WHERE CategoryId = @id;", new { id });
    }

    public async Task AddCategory(Category category)
    {
        await using var db = CreateConnection();
        const string sql = """
            INSERT INTO Categories (CategoryName, Description, IsActive)
            VALUES (@CategoryName, @Description, @IsActive);
            """;
        await db.ExecuteAsync(sql, category);
    }

    public async Task UpdateCategory(Category category)
    {
        await using var db = CreateConnection();
        const string sql = """
            UPDATE Categories SET
                CategoryName = @CategoryName,
                Description = @Description,
                IsActive = @IsActive
            WHERE CategoryId = @CategoryId;
            """;
        await db.ExecuteAsync(sql, category);
    }

    public async Task DeleteCategory(int id)
    {
        await using var db = CreateConnection();
        await db.ExecuteAsync("DELETE FROM Categories WHERE CategoryId = @id;", new { id });
    }
}
