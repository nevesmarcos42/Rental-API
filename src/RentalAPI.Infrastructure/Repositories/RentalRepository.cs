using Microsoft.EntityFrameworkCore;
using RentalAPI.Domain.Entities;
using RentalAPI.Domain.Enums;
using RentalAPI.Domain.Interfaces;
using RentalAPI.Infrastructure.Persistence;

namespace RentalAPI.Infrastructure.Repositories;

public class RentalRepository : Repository<Rental>, IRentalRepository
{
    public RentalRepository(RentalDbContext context) : base(context)
    {
    }

    public async Task<IEnumerable<Rental>> GetActiveRentalsAsync()
    {
        return await _dbSet
            .Where(r => r.Status == RentalStatus.Active || r.Status == RentalStatus.Renewed)
            .Include(r => r.Customer)
            .Include(r => r.Vehicle)
            .ToListAsync();
    }

    public async Task<IEnumerable<Rental>> GetByCustomerIdAsync(Guid customerId)
    {
        return await _dbSet
            .Where(r => r.CustomerId == customerId)
            .Include(r => r.Vehicle)
            .ToListAsync();
    }

    public async Task<IEnumerable<Rental>> GetByVehicleIdAsync(Guid vehicleId)
    {
        return await _dbSet
            .Where(r => r.VehicleId == vehicleId)
            .Include(r => r.Customer)
            .ToListAsync();
    }
}
