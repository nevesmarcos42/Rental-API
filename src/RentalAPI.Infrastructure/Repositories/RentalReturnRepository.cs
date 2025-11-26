using Microsoft.EntityFrameworkCore;
using RentalAPI.Domain.Entities;
using RentalAPI.Domain.Interfaces;
using RentalAPI.Infrastructure.Persistence;

namespace RentalAPI.Infrastructure.Repositories;

public class RentalReturnRepository : Repository<RentalReturn>, IRentalReturnRepository
{
    public RentalReturnRepository(RentalDbContext context) : base(context)
    {
    }

    public async Task<RentalReturn?> GetByRentalIdAsync(Guid rentalId)
    {
        return await _dbSet
            .FirstOrDefaultAsync(rr => rr.RentalId == rentalId);
    }
}
