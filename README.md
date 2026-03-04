# ✈️ FBS - Flight Booking System

A robust .NET-based flight reservation system implementing Domain-Driven Design (DDD) principles with Clean Architecture. Features real-time seat management, automated reservation expiration, and event-driven notifications.

[![.NET](https://img.shields.io/badge/.NET-10.0-blue)](https://dotnet.microsoft.com/)
[![Architecture](https://img.shields.io/badge/Architecture-Clean%20DDD-green)](https://learn.microsoft.com/en-us/dotnet/architecture/)
[![License](https://img.shields.io/badge/License-MIT-yellow.svg)](LICENSE)

## 📋 Table of Contents

- [Features](#-features)
- [Architecture](#-architecture)
- [Technologies](#-technologies)
- [Getting Started](#-getting-started)
- [Project Structure](#-project-structure)
- [API Endpoints](#-api-endpoints)
- [Domain Model](#-domain-model)
- [Configuration](#-configuration)
- [Development](#-development)
- [Testing](#-testing)
- [Deployment](#-deployment)
- [License](#-license)

## ✨ Features

### Core Functionality
- **Flight Management** - Browse available flights with real-time seat availability
- **Seat Reservation** - Reserve specific seats with automatic conflict prevention
- **Reservation Lifecycle** - Pending → Confirmed → Expired/Cancelled states
- **Automatic Expiration** - Background job expires unpaid reservations after 10 minutes
- **Real-time Updates** - Concurrent reservation handling with optimistic locking

### Technical Features
- **Domain-Driven Design** - Rich domain model with business rules enforcement
- **Clean Architecture** - Clear separation of concerns across layers
- **Event-Driven** - Domain events for loose coupling and notifications
- **CQRS Pattern** - Separate read and write operations with MediatR
- **Repository Pattern** - Abstracted data access with Unit of Work
- **Background Jobs** - Hangfire for recurring tasks and job scheduling
- **API Documentation** - Interactive Swagger/OpenAPI documentation
- **Retry Logic** - Resilient database operations with exponential backoff

## 🏗️ Architecture

### Clean Architecture Layers

```
┌─────────────────────────────────────────┐
│           FBS.API (Presentation)        │
│   Controllers, Middleware, Endpoints    │
└─────────────────┬───────────────────────┘
                  │
┌─────────────────▼───────────────────────┐
│      FBS.Application (Use Cases)        │
│  Commands, Queries, DTOs, Validators    │
└─────────────────┬───────────────────────┘
                  │
┌─────────────────▼───────────────────────┐
│        FBS.Domain (Business Logic)      │
│  Entities, Value Objects, Rules, Events │
└─────────────────┬───────────────────────┘
                  │
┌─────────────────▼───────────────────────┐
│    FBS.Infrastructure (Data & I/O)      │
│  EF Core, Repositories, External APIs   │
└─────────────────────────────────────────┘
```

### Key Patterns & Principles

- **Domain-Driven Design (DDD)** - Aggregates, Entities, Value Objects, Domain Events
- **CQRS** - Command/Query separation with MediatR
- **Repository Pattern** - Data access abstraction
- **Unit of Work** - Transaction management
- **Specification Pattern** - Reusable business rules
- **Pipeline Behaviors** - Cross-cutting concerns (validation, transactions, logging)

## 🛠️ Technologies

### Core Stack
- **.NET 10.0** - Latest LTS framework
- **ASP.NET Core 10.0** - Web API framework
- **Entity Framework Core 10.0** - ORM for database access
- **SQL Server** - Relational database

### Libraries & Tools
- **MediatR** - CQRS and mediator pattern implementation
- **FluentValidation** - Request validation
- **Hangfire** - Background job processing
- **Polly** - Resilience and transient fault handling
- **Serilog** - Structured logging
- **Swagger/OpenAPI** - API documentation

## 🚀 Getting Started

### Prerequisites

- [.NET 10.0 SDK](https://dotnet.microsoft.com/download/dotnet/10.0)
- [SQL Server 2019+](https://www.microsoft.com/sql-server) or SQL Server Express
- [Visual Studio 2022](https://visualstudio.microsoft.com/) or [JetBrains Rider](https://www.jetbrains.com/rider/) (recommended)
- Optional: [SQL Server Management Studio](https://learn.microsoft.com/en-us/sql/ssms/download-sql-server-management-studio-ssms)

### Installation

1. **Clone the repository**
   ```bash
   git clone https://github.com/yourusername/flight-booking-system.git
   cd flight-booking-system
   ```

2. **Configure database connection**
   
   Update `src/FBS.API/appsettings.json`:
   ```json
   {
     "ConnectionStrings": {
       "DefaultConnection": "Server=localhost;Database=FlightBookingSystem;Trusted_Connection=True;TrustServerCertificate=True"
     }
   }
   ```

3. **Apply database migrations**
   ```bash
   cd src/FBS.Infrastructure
   dotnet ef database update --startup-project ../FBS.API
   ```

4. **Run the application**
   ```bash
   cd ../FBS.API
   dotnet run
   ```

5. **Access the API**
   - Swagger UI: https://localhost:5001/swagger
   - API Base URL: https://localhost:5001/api

### Quick Test

Create a test reservation:
```bash
curl -X POST https://localhost:5001/api/reservations \
  -H "Content-Type: application/json" \
  -d '{
    "flightNumber": "AA1234",
    "seatNumber": "12A",
    "passengerFirstName": "John",
    "passengerLastName": "Doe",
    "passengerEmail": "john.doe@example.com"
  }'
```

## 📁 Project Structure

```
src/
├── FBS.API/                    # Presentation Layer
│   ├── Controllers/            # API endpoints
│   ├── Middleware/             # Custom middleware
│   └── Program.cs              # Application entry point
│
├── FBS.Application/            # Application Layer
│   ├── Commands/               # Write operations
│   │   ├── CreateReservation/
│   │   ├── ConfirmReservation/
│   │   ├── CancelReservation/
│   │   └── ExpireReservation/
│   ├── Queries/                # Read operations
│   │   ├── GetAvailableFlights/
│   │   ├── GetFlightByNumber/
│   │   └── GetReservation/
│   ├── Common/                 # Shared application logic
│   │   ├── Behaviors/          # MediatR pipeline behaviors
│   │   └── Result/             # Result pattern implementation
│   └── EventHandlers/          # Domain event handlers
│
├── FBS.Domain/                 # Domain Layer
│   ├── Flight/                 # Flight aggregate
│   │   ├── Flight.cs           # Aggregate root
│   │   ├── FlightId.cs         # Value object
│   │   ├── FlightNumber.cs     # Value object
│   │   ├── SeatNumber.cs       # Value object
│   │   ├── Events/             # Domain events
│   │   └── Rules/              # Business rules
│   ├── Reservation/            # Reservation aggregate
│   │   ├── Reservation.cs      # Aggregate root
│   │   ├── ReservationId.cs    # Value object
│   │   ├── ReservationStatus.cs # Enumeration
│   │   ├── Events/             # Domain events
│   │   └── Rules/              # Business rules
│   ├── SharedKernel/           # Shared value objects
│   │   ├── Email.cs
│   │   └── PassengerInfo.cs
│   ├── Common/                 # Domain infrastructure
│   │   ├── Base/               # Base classes
│   │   └── Interfaces/         # Domain interfaces
│   ├── Repositories/           # Repository interfaces
│   └── Services/               # Domain services
│
└── FBS.Infrastructure/         # Infrastructure Layer
    ├── Persistence/            # Data access
    │   ├── ApplicationDbContext.cs
    │   ├── Configurations/     # EF Core configurations
    │   ├── Repositories/       # Repository implementations
    │   └── Migrations/         # Database migrations
    ├── EventDispatcher/        # Domain event dispatcher
    ├── Events/                 # External event publisher
    ├── BackgroundJobs/         # Hangfire jobs
    ├── Services/               # Infrastructure services
    └── Seed/                   # Database seeding
```

## 🔌 API Endpoints

### Flights

| Method | Endpoint | Description |
|--------|----------|-------------|
| `GET` | `/api/flights` | Get all available flights |
| `GET` | `/api/flights/{flightNumber}` | Get flight details by number |

### Reservations

| Method | Endpoint | Description |
|--------|----------|-------------|
| `POST` | `/api/reservations` | Create a new reservation |
| `GET` | `/api/reservations/{id}` | Get reservation details |
| `POST` | `/api/reservations/{id}/confirm` | Confirm a reservation |
| `DELETE` | `/api/reservations/{id}` | Cancel a reservation |

### Health

| Method | Endpoint | Description |
|--------|----------|-------------|
| `GET` | `/health` | Health check endpoint |

### Example Request: Create Reservation

**POST** `/api/reservations`

```json
{
  "flightNumber": "AA1234",
  "seatNumber": "12A",
  "passengerFirstName": "John",
  "passengerLastName": "Doe",
  "passengerEmail": "john.doe@example.com"
}
```

**Response** (201 Created):
```json
{
  "isSuccess": true,
  "value": {
    "reservationId": "3fa85f64-5717-4562-b3fc-2c963f66afa6",
    "flightNumber": "AA1234",
    "seatNumber": "12A",
    "status": "Pending",
    "expiresAt": "2024-02-26T12:15:00Z",
    "message": "Reservation created successfully. Please confirm within 15 minutes."
  }
}
```

## 🎯 Domain Model

### Aggregates

#### Flight Aggregate
- **Root Entity**: `Flight`
- **Value Objects**: `FlightId`, `FlightNumber`, `SeatNumber`
- **Business Rules**:
  - Cannot reserve already reserved seat
  - Cannot release unreserved seat
  - Flight must have available capacity

#### Reservation Aggregate
- **Root Entity**: `Reservation`
- **Value Objects**: `ReservationId`, `PassengerInfo`, `Email`
- **States**: Pending → Confirmed / Cancelled / Expired
- **Business Rules**:
  - Can only confirm pending reservations
  - Cannot cancel confirmed reservations
  - Expires automatically after 15 minutes if not confirmed

### Domain Events

#### Internal Events (Local Processing)
- `SeatReservedEvent` - Triggered when seat is reserved
- `SeatReleasedEvent` - Triggered when seat is released

#### External Events (Sent to FBNS)
- `ReservationCreatedEvent` - Sent to notification system
- `ReservationConfirmedEvent` - Sent to notification system
- `ReservationCancelledEvent` - Sent to notification system
- `ReservationExpiredEvent` - Sent to notification system

## ⚙️ Configuration

### Database Configuration

**appsettings.json**:
```json
{
  "ConnectionStrings": {
    "DefaultConnection": "Server=localhost;Database=FlightBookingSystem;Integrated Security=true;TrustServerCertificate=True",
    "HangfireConnection": "Server=localhost;Database=FlightBookingSystem;Integrated Security=true;TrustServerCertificate=True"
  }
}
```

### Event Publisher Configuration

**appsettings.json**:
```json
{
  "EventPublisher": {
    "BaseUrl": "https://localhost:5002",
    "ApiKey": "your-api-key-here",
    "RetryCount": 3,
    "TimeoutSeconds": 10
  }
}
```

### Logging Configuration

**appsettings.json**:
```json
{
  "Serilog": {
    "MinimumLevel": {
      "Default": "Information",
      "Override": {
        "Microsoft": "Warning",
        "Microsoft.EntityFrameworkCore": "Warning"
      }
    },
    "WriteTo": [
      {
        "Name": "Console"
      },
      {
        "Name": "File",
        "Args": {
          "path": "logs/fbs-.log",
          "rollingInterval": "Day"
        }
      }
    ]
  }
}
```

## 👨‍💻 Development

### Running in Development

```bash
# Run with hot reload
dotnet watch run --project src/FBS.API

# Run with specific environment
dotnet run --project src/FBS.API --environment Development
```

### Database Migrations

```bash
# Add new migration
cd src/FBS.Infrastructure
dotnet ef migrations add MigrationName --startup-project ../FBS.API

# Apply migrations
dotnet ef database update --startup-project ../FBS.API

# Rollback migration
dotnet ef database update PreviousMigrationName --startup-project ../FBS.API

# Remove last migration (if not applied)
dotnet ef migrations remove --startup-project ../FBS.API
```

### Seeding Data

The system automatically seeds sample flights on startup (Development environment only):

```csharp
// In Program.cs
if (app.Environment.IsDevelopment())
{
    using var scope = app.Services.CreateScope();
    var seeder = scope.ServiceProvider.GetRequiredService<FlightDataSeeder>();
    await seeder.SeedAsync();
}
```

### Background Jobs

View Hangfire dashboard: https://localhost:5001/hangfire

**Jobs:**
- `ExpireReservationsJob` - Runs every minute to expire old reservations

## 🧪 Testing

### Run Tests

```bash
# Run all tests
dotnet test

# Run with coverage
dotnet test /p:CollectCoverage=true

# Run specific test project
dotnet test tests/FBS.Domain.Tests
```

### Test Structure

```
tests/
├── FBS.Domain.Tests/           # Domain logic tests
│   ├── FlightTests.cs
│   ├── ReservationTests.cs
│   └── BusinessRulesTests.cs
├── FBS.Application.Tests/      # Application logic tests
│   ├── Commands/
│   └── Queries/
└── FBS.API.Tests/              # API integration tests
    └── Controllers/
```

## 🚢 Deployment

### Docker

```bash
# Build image
docker build -t fbs:latest .

# Run container
docker run -d -p 5000:80 \
  -e ConnectionStrings__DefaultConnection="your-connection-string" \
  fbs:latest
```

### Azure App Service

```bash
# Publish to Azure
az webapp up --name your-app-name --resource-group your-rg --runtime "DOTNETCORE:10.0"
```

### Environment Variables

Required environment variables:
- `ConnectionStrings__DefaultConnection` - Database connection string
- `EventPublisher__BaseUrl` - FBNS API URL
- `EventPublisher__ApiKey` - FBNS API key

## 📄 License

This project is licensed under the MIT License - see the [LICENSE](LICENSE) file for details.

## 📞 Contact

- **Project Link**: [https://github.com/komenday/flight-booking-system](https://github.com/komenday/flight-booking-system)
- **Documentation**: [https://docs.flightbooking.dev](https://docs.flightbooking.dev)
- **Issues**: [https://github.com/komenday/flight-booking-system/issues](https://github.com/komenday/flight-booking-system/issues)

## 🙏 Acknowledgments

- [Clean Architecture by Robert C. Martin](https://blog.cleancoder.com/uncle-bob/2012/08/13/the-clean-architecture.html)
- [Domain-Driven Design by Eric Evans](https://www.domainlanguage.com/ddd/)
- [Microsoft Architecture Guides](https://learn.microsoft.com/en-us/dotnet/architecture/)
- [MediatR Documentation](https://github.com/jbogard/MediatR)

---

Built with ❤️ using .NET 10 and Clean Architecture principles.
