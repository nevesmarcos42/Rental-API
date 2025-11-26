using MediatR;
using RentalAPI.Application.Commands.Rentals;
using RentalAPI.Application.DTOs;
using RentalAPI.Application.Interfaces;
using RentalAPI.Domain.Enums;
using RentalAPI.Domain.Interfaces;
using System.Text.Json;

namespace RentalAPI.Application.Handlers.Rentals;

public class RenewRentalCommandHandler : IRequestHandler<RenewRentalCommand, RentalDto>
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IKafkaProducer _kafkaProducer;

    public RenewRentalCommandHandler(IUnitOfWork unitOfWork, IKafkaProducer kafkaProducer)
    {
        _unitOfWork = unitOfWork;
        _kafkaProducer = kafkaProducer;
    }

    public async Task<RentalDto> Handle(RenewRentalCommand request, CancellationToken cancellationToken)
    {
        var rental = await _unitOfWork.Rentals.GetByIdAsync(request.RentalId);

        if (rental == null)
            throw new KeyNotFoundException($"Rental with ID {request.RentalId} not found.");

        if (rental.Status != RentalStatus.Active)
            throw new InvalidOperationException("Only active rentals can be renewed.");

        var vehicle = await _unitOfWork.Vehicles.GetByIdAsync(rental.VehicleId);
        if (vehicle == null)
            throw new KeyNotFoundException($"Vehicle with ID {rental.VehicleId} not found.");

        // Calculate additional cost
        var additionalDays = (request.NewExpectedEndDate - rental.ExpectedEndDate).Days;
        if (additionalDays <= 0)
            throw new InvalidOperationException("New expected end date must be after current expected end date.");

        var additionalCost = additionalDays * vehicle.DailyRate;

        // Update rental
        var oldExpectedEndDate = rental.ExpectedEndDate;
        rental.ExpectedEndDate = request.NewExpectedEndDate;
        rental.TotalAmount += additionalCost;

        await _unitOfWork.SaveChangesAsync();

        // Publish event to Kafka
        var eventData = new
        {
            RentalId = rental.Id,
            CustomerId = rental.CustomerId,
            VehicleId = rental.VehicleId,
            OldExpectedEndDate = oldExpectedEndDate,
            NewExpectedEndDate = rental.ExpectedEndDate,
            AdditionalDays = additionalDays,
            AdditionalCost = additionalCost,
            NewTotalAmount = rental.TotalAmount,
            RenewedAt = DateTime.UtcNow
        };
        await _kafkaProducer.ProduceAsync("rental.renewed", JsonSerializer.Serialize(eventData));

        return new RentalDto
        {
            Id = rental.Id,
            CustomerId = rental.CustomerId,
            VehicleId = rental.VehicleId,
            StartDate = rental.StartDate,
            ExpectedEndDate = rental.ExpectedEndDate,
            ActualEndDate = rental.ActualEndDate,
            TotalAmount = rental.TotalAmount,
            Status = rental.Status
        };
    }
}
