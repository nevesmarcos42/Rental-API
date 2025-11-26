using MediatR;
using RentalAPI.Application.Commands.Rentals;
using RentalAPI.Application.DTOs;
using RentalAPI.Application.Interfaces;
using RentalAPI.Domain.Entities;
using RentalAPI.Domain.Enums;
using RentalAPI.Domain.Interfaces;

namespace RentalAPI.Application.Handlers.Rentals;

public class CompleteRentalCommandHandler : IRequestHandler<CompleteRentalCommand, RentalReturnDto>
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IKafkaProducer _kafkaProducer;

    public CompleteRentalCommandHandler(IUnitOfWork unitOfWork, IKafkaProducer kafkaProducer)
    {
        _unitOfWork = unitOfWork;
        _kafkaProducer = kafkaProducer;
    }

    public async Task<RentalReturnDto> Handle(CompleteRentalCommand request, CancellationToken cancellationToken)
    {
        var rental = await _unitOfWork.Rentals.GetByIdAsync(request.RentalId);
        if (rental == null)
        {
            throw new KeyNotFoundException("Aluguel não encontrado.");
        }

        if (rental.Status == RentalStatus.Completed)
        {
            throw new InvalidOperationException("Este aluguel já foi finalizado.");
        }

        var vehicle = await _unitOfWork.Vehicles.GetByIdAsync(rental.VehicleId);
        if (vehicle == null)
        {
            throw new KeyNotFoundException("Veículo não encontrado.");
        }

        // Calcula multa por atraso (1.5x o valor da diária por dia de atraso)
        decimal lateFee = 0;
        if (request.ReturnDate > rental.ExpectedEndDate)
        {
            var lateDays = (request.ReturnDate - rental.ExpectedEndDate).Days;
            lateFee = lateDays * rental.DailyRate * 1.5m;
        }

        // Define taxa de danos baseada na condição do veículo
        decimal damageFee = request.Condition switch
        {
            VehicleCondition.Excellent => 0,
            VehicleCondition.Good => 0,
            VehicleCondition.Fair => 500,
            VehicleCondition.Poor => 2000,
            _ => 0
        };

        var rentalReturn = new RentalReturn
        {
            Id = Guid.NewGuid(),
            RentalId = rental.Id,
            ReturnDate = request.ReturnDate,
            Condition = request.Condition,
            LateFee = lateFee,
            DamageFee = damageFee,
            Notes = request.Notes,
            InspectionApproved = request.Condition is VehicleCondition.Excellent or VehicleCondition.Good,
            InspectedBy = request.InspectedBy,
            CreatedAt = DateTime.UtcNow,
            UpdatedAt = DateTime.UtcNow
        };

        rental.ActualEndDate = request.ReturnDate;
        rental.FinalMileage = request.FinalMileage;
        rental.Status = RentalStatus.Completed;
        rental.TotalAmount += lateFee + damageFee;

        // Se o veículo estiver em condição ruim, envia para manutenção
        vehicle.Status = request.Condition == VehicleCondition.Poor ? VehicleStatus.Maintenance : VehicleStatus.Available;
        vehicle.Mileage = request.FinalMileage;

        await _unitOfWork.RentalReturns.AddAsync(rentalReturn);
        await _unitOfWork.Rentals.UpdateAsync(rental);
        await _unitOfWork.Vehicles.UpdateAsync(vehicle);
        await _unitOfWork.SaveChangesAsync();

        // Publica evento de finalização no Kafka
        await _kafkaProducer.ProduceAsync("rental-events", 
            $"{{\"event\":\"rental.completed\",\"rentalId\":\"{rental.Id}\",\"totalAmount\":{rental.TotalAmount},\"condition\":\"{request.Condition}\"}}");

        return new RentalReturnDto
        {
            Id = rentalReturn.Id,
            RentalId = rentalReturn.RentalId,
            ReturnDate = rentalReturn.ReturnDate,
            Condition = rentalReturn.Condition,
            LateFee = rentalReturn.LateFee,
            DamageFee = rentalReturn.DamageFee,
            Notes = rentalReturn.Notes,
            InspectionApproved = rentalReturn.InspectionApproved,
            InspectedBy = rentalReturn.InspectedBy
        };
    }
}
