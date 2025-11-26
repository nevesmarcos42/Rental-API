using Microsoft.EntityFrameworkCore;
using RentalAPI.Domain.Entities;
using RentalAPI.Domain.Interfaces;
using RentalAPI.Infrastructure.Persistence;

namespace RentalAPI.Infrastructure.Repositories;

public class CustomerRepository : Repository<Customer>, ICustomerRepository
{
    public CustomerRepository(RentalDbContext context) : base(context)
    {
    }

    public async Task<Customer?> GetByEmailAsync(string email)
    {
        return await _dbSet
            .FirstOrDefaultAsync(c => c.Email == email);
    }

    public async Task<Customer?> GetByDriversLicenseAsync(string driversLicense)
    {
        return await _dbSet
            .FirstOrDefaultAsync(c => c.DriversLicense == driversLicense);
    }
}
