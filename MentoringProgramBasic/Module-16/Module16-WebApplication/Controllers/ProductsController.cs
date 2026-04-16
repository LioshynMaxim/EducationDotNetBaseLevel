using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using Module16_WebApplication.Configuration;
using Module16_WebApplication.Data;
using Module16_WebApplication.Models;
using Module16_WebApplication.ViewModels;

namespace Module16_WebApplication.Controllers;

public class ProductsController : Controller
{
    private readonly NorthwindContext _context;
    private readonly NorthwindSettings _settings;

    public ProductsController(NorthwindContext context, IOptions<NorthwindSettings> settings)
    {
        _context = context;
        _settings = settings.Value;
    }

    // GET: Products
    public async Task<IActionResult> Index()
    {
        var query = _context.Products
            .Include(p => p.Category)
            .Include(p => p.Supplier)
            .OrderBy(p => p.ProductName);
        
        // Apply MaxProductsToShow setting
        var products = _settings.MaxProductsToShow > 0
            ? await query.Take(_settings.MaxProductsToShow).ToListAsync()
            : await query.ToListAsync();
        
        ViewBag.MaxProducts = _settings.MaxProductsToShow;
        ViewBag.TotalProducts = await _context.Products.CountAsync();
        
        return View(products);
    }

    // GET: Products/Details/5
    public async Task<IActionResult> Details(int? id)
    {
        if (id == null)
        {
            return NotFound();
        }

        var product = await _context.Products
            .Include(p => p.Category)
            .Include(p => p.Supplier)
            .FirstOrDefaultAsync(m => m.ProductID == id);

        if (product == null)
        {
            return NotFound();
        }

        return View(product);
    }

    // GET: Products/Create
    public IActionResult Create()
    {
        PopulateDropDownLists();
        return View();
    }

    // POST: Products/Create
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Create(ProductViewModel model)
    {
        if (ModelState.IsValid)
        {
            var product = new Product
            {
                ProductName = model.ProductName,
                SupplierID = model.SupplierID,
                CategoryID = model.CategoryID,
                QuantityPerUnit = model.QuantityPerUnit,
                UnitPrice = model.UnitPrice,
                UnitsInStock = model.UnitsInStock ?? 0,
                UnitsOnOrder = model.UnitsOnOrder ?? 0,
                ReorderLevel = model.ReorderLevel ?? 0,
                Discontinued = model.Discontinued
            };

            _context.Add(product);
            await _context.SaveChangesAsync();
            TempData["SuccessMessage"] = "Product created successfully!";
            return RedirectToAction(nameof(Index));
        }

        PopulateDropDownLists(model.SupplierID, model.CategoryID);
        return View(model);
    }

    // GET: Products/Edit/5
    public async Task<IActionResult> Edit(int? id)
    {
        if (id == null)
        {
            return NotFound();
        }

        var product = await _context.Products.FindAsync(id);
        if (product == null)
        {
            return NotFound();
        }

        var model = new ProductViewModel
        {
            ProductID = product.ProductID,
            ProductName = product.ProductName,
            SupplierID = product.SupplierID,
            CategoryID = product.CategoryID,
            QuantityPerUnit = product.QuantityPerUnit,
            UnitPrice = product.UnitPrice,
            UnitsInStock = product.UnitsInStock,
            UnitsOnOrder = product.UnitsOnOrder,
            ReorderLevel = product.ReorderLevel,
            Discontinued = product.Discontinued
        };

        PopulateDropDownLists(product.SupplierID, product.CategoryID);
        return View(model);
    }

    // POST: Products/Edit/5
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Edit(int id, ProductViewModel model)
    {
        if (id != model.ProductID)
        {
            return NotFound();
        }

        if (ModelState.IsValid)
        {
            try
            {
                var product = await _context.Products.FindAsync(id);
                if (product == null)
                {
                    return NotFound();
                }

                product.ProductName = model.ProductName;
                product.SupplierID = model.SupplierID;
                product.CategoryID = model.CategoryID;
                product.QuantityPerUnit = model.QuantityPerUnit;
                product.UnitPrice = model.UnitPrice;
                product.UnitsInStock = model.UnitsInStock;
                product.UnitsOnOrder = model.UnitsOnOrder;
                product.ReorderLevel = model.ReorderLevel;
                product.Discontinued = model.Discontinued;

                _context.Update(product);
                await _context.SaveChangesAsync();
                TempData["SuccessMessage"] = "Product updated successfully!";
                return RedirectToAction(nameof(Index));
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!ProductExists(model.ProductID))
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }
        }

        PopulateDropDownLists(model.SupplierID, model.CategoryID);
        return View(model);
    }

    private bool ProductExists(int id)
    {
        return _context.Products.Any(e => e.ProductID == id);
    }

    private void PopulateDropDownLists(int? selectedSupplierId = null, int? selectedCategoryId = null)
    {
        ViewBag.SupplierID = new SelectList(_context.Suppliers.OrderBy(s => s.CompanyName), "SupplierID", "CompanyName", selectedSupplierId);
        ViewBag.CategoryID = new SelectList(_context.Categories.OrderBy(c => c.CategoryName), "CategoryID", "CategoryName", selectedCategoryId);
    }
}
