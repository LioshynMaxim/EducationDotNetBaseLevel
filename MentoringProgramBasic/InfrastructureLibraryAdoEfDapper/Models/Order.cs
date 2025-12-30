namespace InfrastructureLibraryAdoEfDapper.Models;

public class Order
{
    public int Id { get; set; }
    public OrderStatus Status { get; set; }
    public DateTime CreateDate { get; set; }
    public DateTime UpdateDate { get; set; }
    public int ProductId { get; set; }
    public Product? Product { get; set; }
}
