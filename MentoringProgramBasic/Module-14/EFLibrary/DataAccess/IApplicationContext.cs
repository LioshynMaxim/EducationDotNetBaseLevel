using InfrastructureLibraryAdoEfDapper.Models;
using Microsoft.EntityFrameworkCore;

namespace EFLibrary.DataAccess
{
    public interface IApplicationContext : IDisposable
    {
        DbSet<Order> Orders { get; }
        DbSet<Product> Products { get; }
    }
}
