# Rental API - Sistema de Locação de Veículos

## Sobre o Projeto

API RESTful para gerenciamento de locadora de veículos, desenvolvida em .NET 8 com Clean Architecture e SOLID.

### Características Principais

- Clean Architecture com separação clara de responsabilidades (Domain, Application, Infrastructure, API)
- CQRS Pattern usando MediatR para Commands e Queries
- Repository Pattern com Unit of Work para abstração de acesso a dados
- JWT Authentication com autorização baseada em roles (Admin, Atendente)
- Entity Framework Core 8 com PostgreSQL e Code-First migrations
- Apache Kafka para mensageria assíncrona de eventos
- FluentValidation para validação de inputs
- Swagger/OpenAPI com suporte completo a JWT
- Docker e Docker Compose para orquestração de containers
- Testes Unitários com xUnit, Moq e FluentAssertions
- GitHub Actions para CI/CD automatizado

## Arquitetura

```
Dotnet8-Rental-API/
├── src/
│   ├── RentalAPI.Domain/          # Entidades, Enums, Interfaces do domínio
│   ├── RentalAPI.Application/     # DTOs, Commands, Queries, Handlers, Validators
│   ├── RentalAPI.Infrastructure/  # DbContext, Repositories, Kafka, Auth
│   └── RentalAPI.API/            # Controllers, Middleware, Configuração
├── tests/
│   └── RentalAPI.Tests/          # Testes unitários
├── Dockerfile
├── docker-compose.yml
└── .github/workflows/
    └── build-and-test.yml
```

## Tecnologias Utilizadas

- .NET 8.0 - Framework principal
- ASP.NET Core Web API - API RESTful
- Entity Framework Core 8.0.8 - ORM
- PostgreSQL 16 - Banco de dados relacional
- Apache Kafka - Message broker para eventos
- MediatR 12.4.1 - CQRS implementation
- FluentValidation 11.10.0 - Validação de dados
- BCrypt.Net-Next 4.0.3 - Hash de senhas
- System.IdentityModel.Tokens.Jwt 8.0.2 - Autenticação JWT
- Swagger/OpenAPI - Documentação da API
- xUnit - Framework de testes
- Moq - Mocking para testes
- FluentAssertions - Assertions fluentes
- Docker - Containerização

## Pré-requisitos

- [.NET 8 SDK](https://dotnet.microsoft.com/download/dotnet/8.0)
- [Docker Desktop](https://www.docker.com/products/docker-desktop) (opcional, para execução em containers)
- [PostgreSQL 16](https://www.postgresql.org/download/) (se não usar Docker)

## Configuração e Execução

### Opção 1: Executar com Docker Compose (Recomendado)

```bash
# Clone o repositório
git clone https://github.com/seu-usuario/Dotnet8-Rental-API.git
cd Dotnet8-Rental-API

# Execute todos os serviços (PostgreSQL, Kafka, Zookeeper, API)
docker-compose up -d

# A API estará disponível em: http://localhost:5000
# Swagger UI: http://localhost:5000/swagger
```

### Opção 2: Executar Localmente

```bash
# Clone o repositório
git clone https://github.com/seu-usuario/Dotnet8-Rental-API.git
cd Dotnet8-Rental-API

# Restore dependências
dotnet restore

# Configure a connection string no appsettings.json
# Certifique-se de que PostgreSQL e Kafka estão rodando

# Execute as migrations
dotnet ef database update --project src/RentalAPI.Infrastructure --startup-project src/RentalAPI.API

# Execute a aplicação
dotnet run --project src/RentalAPI.API/RentalAPI.API.csproj

# A API estará disponível em: http://localhost:5000
# Swagger UI: http://localhost:5000/swagger
```

## Executar Testes

```bash
# Executar todos os testes
dotnet test

# Executar testes com cobertura
dotnet test --collect:"XPlat Code Coverage"
```

## Endpoints Principais

### Autenticação

- `POST /api/auth/login` - Login de usuário

### Veículos

- `GET /api/vehicles` - Lista todos os veículos
- `GET /api/vehicles/{id}` - Busca veículo por ID
- `GET /api/vehicles/available` - Lista veículos disponíveis
- `POST /api/vehicles` - Cria novo veículo (Admin)
- `PUT /api/vehicles/{id}` - Atualiza veículo (Admin)
- `DELETE /api/vehicles/{id}` - Remove veículo (Admin)

### Clientes

- `GET /api/customers` - Lista todos os clientes
- `GET /api/customers/{id}` - Busca cliente por ID
- `POST /api/customers` - Cria novo cliente
- `PUT /api/customers/{id}` - Atualiza cliente
- `DELETE /api/customers/{id}` - Remove cliente

### Aluguéis

- `GET /api/rentals` - Lista todos os aluguéis
- `GET /api/rentals/{id}` - Busca aluguel por ID
- `GET /api/rentals/active` - Lista aluguéis ativos
- `POST /api/rentals` - Cria novo aluguel (Admin/Atendente)
- `PUT /api/rentals/{id}/renew` - Renova aluguel (Admin/Atendente)
- `PUT /api/rentals/{id}/complete` - Finaliza aluguel (Admin/Atendente)

## Autenticação

A API utiliza JWT Bearer Token para autenticação. Após fazer login via `/api/auth/login`, utilize o token retornado no header:

```
Authorization: Bearer {seu-token-jwt}
```

### Roles Disponíveis

- Admin: Acesso total a todas as operações
- Atendente: Pode gerenciar aluguéis e consultar dados

## Eventos Kafka

A API publica eventos no Kafka para as seguintes operações:

- `rental.created` - Quando um novo aluguel é criado
- `rental.renewed` - Quando um aluguel é renovado
- `rental.completed` - Quando um aluguel é finalizado

## Migrations

```bash
# Criar nova migration
dotnet ef migrations add NomeDaMigration --project src/RentalAPI.Infrastructure --startup-project src/RentalAPI.API

# Aplicar migrations
dotnet ef database update --project src/RentalAPI.Infrastructure --startup-project src/RentalAPI.API

# Remover última migration
dotnet ef migrations remove --project src/RentalAPI.Infrastructure --startup-project src/RentalAPI.API
```

## Estrutura do Banco de Dados

- **Vehicles** - Veículos disponíveis para locação
- **Customers** - Clientes cadastrados
- **Rentals** - Aluguéis realizados
- **RentalReturns** - Devoluções de veículos
- **Users** - Usuários do sistema

## Contribuindo

1. Fork o projeto
2. Crie uma branch para sua feature (`git checkout -b feature/AmazingFeature`)
3. Commit suas mudanças (`git commit -m 'Add some AmazingFeature'`)
4. Push para a branch (`git push origin feature/AmazingFeature`)
5. Abra um Pull Request

## Licença

Este projeto está sob a licença MIT.
