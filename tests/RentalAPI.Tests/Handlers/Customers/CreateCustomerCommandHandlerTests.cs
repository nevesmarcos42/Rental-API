using FluentAssertions;
using Moq;
using RentalAPI.Application.Commands.Customers;
using RentalAPI.Application.Handlers.Customers;
using RentalAPI.Domain.Entities;
using RentalAPI.Domain.Interfaces;

namespace RentalAPI.Tests.Handlers.Customers;

public class CreateCustomerCommandHandlerTests
{
    private readonly Mock<IUnitOfWork> _unitOfWorkMock;
    private readonly Mock<ICustomerRepository> _customerRepositoryMock;
    private readonly CreateCustomerCommandHandler _handler;

    public CreateCustomerCommandHandlerTests()
    {
        _unitOfWorkMock = new Mock<IUnitOfWork>();
        _customerRepositoryMock = new Mock<ICustomerRepository>();

        _unitOfWorkMock.Setup(u => u.Customers).Returns(_customerRepositoryMock.Object);

        _handler = new CreateCustomerCommandHandler(_unitOfWorkMock.Object);
    }

    [Fact]
    public async Task Handle_ValidRequest_ShouldCreateCustomer()
    {
        // Arrange
        var command = new CreateCustomerCommand
        {
            Name = "João Silva",
            Email = "joao.silva@email.com",
            Phone = "(11) 98765-4321",
            DriversLicense = "12345678900",
            Address = "Rua das Flores, 123"
        };

        _customerRepositoryMock.Setup(r => r.GetByEmailAsync(It.IsAny<string>()))
            .ReturnsAsync((Customer?)null);

        _customerRepositoryMock.Setup(r => r.GetByDriversLicenseAsync(It.IsAny<string>()))
            .ReturnsAsync((Customer?)null);

        _customerRepositoryMock.Setup(r => r.AddAsync(It.IsAny<Customer>()))
            .ReturnsAsync((Customer c) => c);

        _unitOfWorkMock.Setup(u => u.SaveChangesAsync())
            .ReturnsAsync(1);

        // Act
        var result = await _handler.Handle(command, CancellationToken.None);

        // Assert
        result.Should().NotBeNull();
        result.Name.Should().Be(command.Name);
        result.Email.Should().Be(command.Email);
        result.Phone.Should().Be(command.Phone);
        result.DriversLicense.Should().Be(command.DriversLicense);
        result.Address.Should().Be(command.Address);
        result.IsActive.Should().BeTrue();

        _customerRepositoryMock.Verify(r => r.AddAsync(It.IsAny<Customer>()), Times.Once);
        _unitOfWorkMock.Verify(u => u.SaveChangesAsync(), Times.Once);
    }

    [Fact]
    public async Task Handle_ValidRequest_ShouldGenerateId()
    {
        // Arrange
        var command = new CreateCustomerCommand
        {
            Name = "Maria Santos",
            Email = "maria@email.com",
            Phone = "(21) 97654-3210",
            DriversLicense = "98765432100",
            Address = "Av. Atlântica, 456"
        };

        _customerRepositoryMock.Setup(r => r.GetByEmailAsync(It.IsAny<string>()))
            .ReturnsAsync((Customer?)null);

        _customerRepositoryMock.Setup(r => r.GetByDriversLicenseAsync(It.IsAny<string>()))
            .ReturnsAsync((Customer?)null);

        _customerRepositoryMock.Setup(r => r.AddAsync(It.IsAny<Customer>()))
            .ReturnsAsync((Customer c) => c);

        _unitOfWorkMock.Setup(u => u.SaveChangesAsync())
            .ReturnsAsync(1);

        // Act
        var result = await _handler.Handle(command, CancellationToken.None);

        // Assert
        result.Id.Should().NotBe(Guid.Empty);
    }
}
