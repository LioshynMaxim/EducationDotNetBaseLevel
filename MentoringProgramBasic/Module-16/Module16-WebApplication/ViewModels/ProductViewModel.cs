using System.ComponentModel.DataAnnotations;

namespace Module16_WebApplication.ViewModels;

public class ProductViewModel
{
    public int ProductID { get; set; }

    [Required(ErrorMessage = "Product name is required")]
    [StringLength(40, MinimumLength = 3, ErrorMessage = "Product name must be between 3 and 40 characters")]
    [Display(Name = "Product Name")]
    public string ProductName { get; set; } = string.Empty;

    [Display(Name = "Supplier")]
    public int? SupplierID { get; set; }

    [Required(ErrorMessage = "Category is required")]
    [Display(Name = "Category")]
    public int? CategoryID { get; set; }

    [StringLength(20, ErrorMessage = "Quantity per unit cannot exceed 20 characters")]
    [Display(Name = "Quantity Per Unit")]
    public string? QuantityPerUnit { get; set; }

    [Required(ErrorMessage = "Unit price is required")]
    [Range(0.01, 10000, ErrorMessage = "Unit price must be between $0.01 and $10,000")]
    [DataType(DataType.Currency)]
    [Display(Name = "Unit Price")]
    public decimal? UnitPrice { get; set; }

    [Range(0, short.MaxValue, ErrorMessage = "Units in stock cannot be negative")]
    [Display(Name = "Units In Stock")]
    public short? UnitsInStock { get; set; }

    [Range(0, short.MaxValue, ErrorMessage = "Units on order cannot be negative")]
    [Display(Name = "Units On Order")]
    public short? UnitsOnOrder { get; set; }

    [Range(0, short.MaxValue, ErrorMessage = "Reorder level cannot be negative")]
    [Display(Name = "Reorder Level")]
    public short? ReorderLevel { get; set; }

    [Display(Name = "Discontinued")]
    public bool Discontinued { get; set; }
}
