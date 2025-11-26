using Microsoft.EntityFrameworkCore;
using RentalAPI.Domain.Entities;
using RentalAPI.Domain.Enums;
using RentalAPI.Domain.Interfaces;
using RentalAPI.Infrastructure.Persistence;

namespace RentalAPI.Infrastructure.Repositories;

public class VehicleRepository : Repository<Vehicle>, IVehicleRepository
{
    public VehicleRepository(RentalDbContext context) : base(context)
    {
    }

    public async Task<IEnumerable<Vehicle>> GetAvailableVehiclesAsync()
    {
        return await _dbSet
            .Where(v => v.Status == VehicleStatus.Available)
            .ToListAsync();
    }

    public async Task<Vehicle?> GetByLicensePlateAsync(string licensePlate)
    {
        return await _dbSet
            .FirstOrDefaultAsync(v => v.LicensePlate == licensePlate);
    }
}
