using EnterpriseStore.Web.Entities;

namespace EnterpriseStore.Web.Repositories;

public interface IUnitOfWork : IDisposable
{
    IRepository<Product> Products { get; }
    IRepository<Order> Orders { get; }
    IRepository<OrderItem> OrderItems { get; }
    Task<int> CompleteAsync();
}

