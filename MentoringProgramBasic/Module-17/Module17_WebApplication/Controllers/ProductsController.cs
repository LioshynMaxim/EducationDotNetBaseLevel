using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Module17_WebApplication.Data;
using Module17_WebApplication.DTOs;
using Module17_WebApplication.Models;

namespace Module17_WebApplication.Controllers;

[Route("api/[controller]")]
[ApiController]
public class ProductsController : ControllerBase
{
    private readonly NorthwindContext _context;

    public ProductsController(NorthwindContext context)
    {
        _context = context;
    }

    // GET: api/products?pageNumber=1&pageSize=10&categoryId=1
    [HttpGet]
    public async Task<ActionResult<PagedResult<Product>>> GetProducts(
        [FromQuery] int pageNumber = 1,
        [FromQuery] int pageSize = 10,
        [FromQuery] int? categoryId = null)
    {
        // Validate parameters
        if (pageNumber < 1)
            pageNumber = 1;
        
        if (pageSize < 1)
            pageSize = 10;
        
        if (pageSize > 100)
            pageSize = 100; // Max page size
        
        // Build query
        var query = _context.Products
            .Include(p => p.Category)
            .AsQueryable();
        
        // Apply category filter if provided
        if (categoryId.HasValue)
        {
            query = query.Where(p => p.CategoryID == categoryId.Value);
        }
        
        // Get total count
        var totalItems = await query.CountAsync();
        
        // Calculate total pages
        var totalPages = (int)Math.Ceiling(totalItems / (double)pageSize);
        
        // Apply pagination
        var items = await query
            .OrderBy(p => p.ProductID)
            .Skip((pageNumber - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync();
        
        // Create paged result
        var result = new PagedResult<Product>
        {
            Items = items,
            PageNumber = pageNumber,
            PageSize = pageSize,
            TotalItems = totalItems,
            TotalPages = totalPages
        };
        
        return Ok(result);
    }

    // GET: api/products/5
    [HttpGet("{id}")]
    public async Task<ActionResult<Product>> GetProduct(int id)
    {
        var product = await _context.Products
            .Include(p => p.Category)
            .FirstOrDefaultAsync(p => p.ProductID == id);

        if (product == null)
        {
            return NotFound();
        }

        return product;
    }

    // POST: api/products
    [HttpPost]
    public async Task<ActionResult<Product>> CreateProduct(Product product)
    {
        _context.Products.Add(product);
        await _context.SaveChangesAsync();

        return CreatedAtAction(nameof(GetProduct), new { id = product.ProductID }, product);
    }

    // PUT: api/products/5
    [HttpPut("{id}")]
    public async Task<IActionResult> UpdateProduct(int id, Product product)
    {
        if (id != product.ProductID)
        {
            return BadRequest();
        }

        _context.Entry(product).State = EntityState.Modified;

        try
        {
            await _context.SaveChangesAsync();
        }
        catch (DbUpdateConcurrencyException)
        {
            if (!ProductExists(id))
            {
                return NotFound();
            }
            else
            {
                throw;
            }
        }

        return NoContent();
    }

    // DELETE: api/products/5
    [HttpDelete("{id}")]
    public async Task<IActionResult> DeleteProduct(int id)
    {
        var product = await _context.Products.FindAsync(id);
        if (product == null)
        {
            return NotFound();
        }

        _context.Products.Remove(product);
        await _context.SaveChangesAsync();

        return NoContent();
    }

    private bool ProductExists(int id)
    {
        return _context.Products.Any(e => e.ProductID == id);
    }
}
