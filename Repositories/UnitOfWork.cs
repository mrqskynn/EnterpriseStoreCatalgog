using EnterpriseStore.Web.Data;
using EnterpriseStore.Web.Entities;

namespace EnterpriseStore.Web.Repositories;

public class UnitOfWork : IUnitOfWork
{
    private readonly ApplicationDbContext _context;

    public IRepository<Product> Products { get; }
    public IRepository<Order> Orders { get; }
    public IRepository<OrderItem> OrderItems { get; }

    public UnitOfWork(ApplicationDbContext context)
    {
        _context = context;
        Products = new Repository<Product>(_context);
        Orders = new Repository<Order>(_context);
        OrderItems = new Repository<OrderItem>(_context);
    }

    public async Task<int> CompleteAsync() => await _context.SaveChangesAsync();

    public void Dispose() => _context.Dispose();
}

