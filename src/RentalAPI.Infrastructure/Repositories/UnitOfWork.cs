using RentalAPI.Domain.Interfaces;
using RentalAPI.Infrastructure.Persistence;

namespace RentalAPI.Infrastructure.Repositories;

public class UnitOfWork : IUnitOfWork
{
    private readonly RentalDbContext _context;
    private IVehicleRepository? _vehicles;
    private ICustomerRepository? _customers;
    private IRentalRepository? _rentals;
    private IRentalReturnRepository? _rentalReturns;
    private IUserRepository? _users;

    public UnitOfWork(RentalDbContext context)
    {
        _context = context;
    }

    public IVehicleRepository Vehicles => _vehicles ??= new VehicleRepository(_context);
    public ICustomerRepository Customers => _customers ??= new CustomerRepository(_context);
    public IRentalRepository Rentals => _rentals ??= new RentalRepository(_context);
    public IRentalReturnRepository RentalReturns => _rentalReturns ??= new RentalReturnRepository(_context);
    public IUserRepository Users => _users ??= new UserRepository(_context);

    public async Task<int> SaveChangesAsync()
    {
        return await _context.SaveChangesAsync();
    }

    public void Dispose()
    {
        _context.Dispose();
    }
}
