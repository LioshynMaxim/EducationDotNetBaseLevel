using EFLibrary.DataAccess;
using InfrastructureLibraryAdoEfDapper.Models;
using InfrastructureLibraryAdoEfDapper.Repositories;
using Microsoft.EntityFrameworkCore;

namespace EFLibrary.Repositories;

public class ProductRepository : IProductRepository
{
    private readonly ApplicationDbContext _context;

    public ProductRepository(ApplicationDbContext context)
    {
        _context = context ?? throw new ArgumentNullException(nameof(context));
    }

    public void Create(Product entity)
    {
        _context.Products.Add(entity);
        _context.SaveChanges();
    }

    public Product? Read(int id) => _context.Products.AsNoTracking().FirstOrDefault(p => p.Id == id);

    public void Update(Product entity)
    {
        var existing = _context.Products.FirstOrDefault(p => p.Id == entity.Id);
        
        if (existing == null)
            throw new ArgumentException($"Product with ID {entity.Id} not found.");
        
        _context.Entry(existing).CurrentValues.SetValues(entity);
        _context.SaveChanges();
    }

    public void Delete(int id)
    {
        var product = _context.Products.FirstOrDefault(p => p.Id == id);
        
        if (product != null)
        {
            _context.Products.Remove(product);
            _context.SaveChanges();
        }
    }

    public IEnumerable<Product> GetAll() => _context.Products.AsNoTracking().ToList();
}
