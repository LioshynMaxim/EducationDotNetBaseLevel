namespace Module17_WebApplication.DTOs;

public class CategoryDto
{
    public int CategoryID { get; set; }
    public string CategoryName { get; set; } = string.Empty;
    public string? Description { get; set; }
}

public class CategoryCreateDto
{
    public string CategoryName { get; set; } = string.Empty;
    public string? Description { get; set; }
}

public class CategoryUpdateDto
{
    public string CategoryName { get; set; } = string.Empty;
    public string? Description { get; set; }
}
