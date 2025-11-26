using MediatR;
using RentalAPI.Application.Commands.Rentals;
using RentalAPI.Application.DTOs;
using RentalAPI.Application.Interfaces;
using RentalAPI.Domain.Entities;
using RentalAPI.Domain.Enums;
using RentalAPI.Domain.Interfaces;

namespace RentalAPI.Application.Handlers.Rentals;

public class CreateRentalCommandHandler : IRequestHandler<CreateRentalCommand, RentalDto>
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IKafkaProducer _kafkaProducer;

    public CreateRentalCommandHandler(IUnitOfWork unitOfWork, IKafkaProducer kafkaProducer)
    {
        _unitOfWork = unitOfWork;
        _kafkaProducer = kafkaProducer;
    }

    public async Task<RentalDto> Handle(CreateRentalCommand request, CancellationToken cancellationToken)
    {
        // Valida se o cliente existe e está ativo
        var customer = await _unitOfWork.Customers.GetByIdAsync(request.CustomerId);
        if (customer == null)
        {
            throw new KeyNotFoundException("Cliente não encontrado.");
        }

        if (!customer.IsActive)
        {
            throw new InvalidOperationException("Cliente inativo não pode realizar aluguéis.");
        }

        // Valida se o veículo existe e está disponível
        var vehicle = await _unitOfWork.Vehicles.GetByIdAsync(request.VehicleId);
        if (vehicle == null)
        {
            throw new KeyNotFoundException("Veículo não encontrado.");
        }

        if (vehicle.Status != VehicleStatus.Available)
        {
            throw new InvalidOperationException("Veículo não está disponível para locação.");
        }

        // Calcula o número de dias e o valor total
        var days = (request.ExpectedEndDate - request.StartDate).Days;
        if (days <= 0)
        {
            throw new InvalidOperationException("A data de devolução deve ser posterior à data de início.");
        }

        var rental = new Rental
        {
            Id = Guid.NewGuid(),
            CustomerId = request.CustomerId,
            VehicleId = request.VehicleId,
            StartDate = request.StartDate,
            ExpectedEndDate = request.ExpectedEndDate,
            DailyRate = vehicle.DailyRate,
            TotalAmount = vehicle.DailyRate * days,
            Status = RentalStatus.Active,
            InitialMileage = request.InitialMileage,
            Notes = request.Notes,
            CreatedAt = DateTime.UtcNow,
            UpdatedAt = DateTime.UtcNow
        };

        // Atualiza o status do veículo e a quilometragem
        vehicle.Status = VehicleStatus.Rented;
        vehicle.Mileage = request.InitialMileage;

        await _unitOfWork.Rentals.AddAsync(rental);
        await _unitOfWork.Vehicles.UpdateAsync(vehicle);
        await _unitOfWork.SaveChangesAsync();

        // Publica evento no Kafka
        await _kafkaProducer.ProduceAsync("rental-events", 
            $"{{\"event\":\"rental.created\",\"rentalId\":\"{rental.Id}\",\"vehicleId\":\"{vehicle.Id}\",\"customerId\":\"{customer.Id}\"}}");

        return new RentalDto
        {
            Id = rental.Id,
            CustomerId = rental.CustomerId,
            CustomerName = customer.Name,
            VehicleId = rental.VehicleId,
            VehicleDescription = $"{vehicle.Brand} {vehicle.Model} ({vehicle.LicensePlate})",
            StartDate = rental.StartDate,
            ExpectedEndDate = rental.ExpectedEndDate,
            TotalAmount = rental.TotalAmount,
            DailyRate = rental.DailyRate,
            Status = rental.Status,
            InitialMileage = rental.InitialMileage,
            Notes = rental.Notes
        };
    }
}
