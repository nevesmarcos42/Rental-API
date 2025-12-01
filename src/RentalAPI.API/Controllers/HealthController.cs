using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using RentalAPI.Infrastructure.Persistence;

namespace RentalAPI.API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class HealthController : ControllerBase
{
    private readonly RentalDbContext _dbContext;
    private readonly ILogger<HealthController> _logger;

    public HealthController(RentalDbContext dbContext, ILogger<HealthController> logger)
    {
        _dbContext = dbContext;
        _logger = logger;
    }

    /// <summary>
    /// Health check endpoint for monitoring
    /// </summary>
    /// <returns>Health status of the API and its dependencies</returns>
    [HttpGet]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status503ServiceUnavailable)]
    public async Task<IActionResult> GetHealth()
    {
        try
        {
            // Check database connectivity
            await _dbContext.Database.CanConnectAsync();
            
            var healthStatus = new
            {
                Status = "Healthy",
                Timestamp = DateTime.UtcNow,
                Service = "Rental API",
                Version = "1.0.0",
                Database = "Connected",
                Environment = Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT") ?? "Production"
            };

            return Ok(healthStatus);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Health check failed");
            
            var healthStatus = new
            {
                Status = "Unhealthy",
                Timestamp = DateTime.UtcNow,
                Service = "Rental API",
                Version = "1.0.0",
                Database = "Disconnected",
                Error = ex.Message
            };

            return StatusCode(StatusCodes.Status503ServiceUnavailable, healthStatus);
        }
    }
}
