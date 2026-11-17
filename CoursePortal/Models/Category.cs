namespace CoursePortal.Models;

public class Category
{
    public int CategoryId { get; set; }

    public String CategoryName { get; set; }
    public String Description { get; set; }
    public bool IsActive { get; set; }
}