using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Module16_WebApplication.Data;

namespace Module16_WebApplication.Controllers;

public class CategoriesController : Controller
{
    private readonly NorthwindContext _context;

    public CategoriesController(NorthwindContext context)
    {
        _context = context;
    }

    // GET: Categories
    public async Task<IActionResult> Index()
    {
        var categories = await _context.Categories
            .OrderBy(c => c.CategoryName)
            .ToListAsync();
        
        return View(categories);
    }
}
