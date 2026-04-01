# ✈️ Flight Booking System (FBS)

A .NET 10 flight seat reservation system built with **Clean Architecture**, **Domain-Driven Design (DDD)**, and **CQRS**, fully deployed to **Microsoft Azure**.

[![.NET](https://img.shields.io/badge/.NET-10.0-blue)](https://dotnet.microsoft.com/)
[![Architecture](https://img.shields.io/badge/Architecture-Clean%20Architecture%20%2B%20DDD-green)](https://learn.microsoft.com/en-us/dotnet/architecture/)
[![Azure](https://img.shields.io/badge/Cloud-Microsoft%20Azure-0089D6)](https://azure.microsoft.com/)
[![CI/CD](https://img.shields.io/badge/CI%2FCD-GitHub%20Actions-2088FF)](https://github.com/features/actions)
[![License](https://img.shields.io/badge/License-MIT-yellow.svg)](LICENSE)

## 📋 Table of Contents

- [Overview](#-overview)
- [Architecture](#-architecture)
- [Azure Infrastructure](#-azure-infrastructure)
- [Technologies](#-technologies)
- [Project Structure](#-project-structure)
- [API Endpoints](#-api-endpoints)
- [Domain Model](#-domain-model)
- [Getting Started](#-getting-started)
- [Configuration](#-configuration)
- [CI/CD](#-cicd)
- [Monitoring](#-monitoring)

---

## 🔍 Overview

FBS allows passengers to search for flights, reserve seats, confirm or cancel reservations, and receive email notifications at each lifecycle stage. The system enforces a 10-minute confirmation window — unconfirmed reservations are automatically expired and their seats released.

### Core Features

- **Flight Search** — search available flights by route and date with real-time seat availability
- **Seat Reservation** — reserve a specific seat with conflict prevention via optimistic concurrency (RowVersion)
- **Reservation Lifecycle** — `Pending → Confirmed / Cancelled / Expired`
- **Automatic Expiration** — Azure Function Timer trigger expires unconfirmed reservations every 2 minutes
- **Email Notifications** — event-driven email notifications at every lifecycle transition via Mailtrap API
- **Resilient HTTP** — Polly v8 standard resilience pipeline (retry + circuit breaker) for outbound HTTP calls

---

## 🏗️ Architecture

### Clean Architecture Layers

```
┌─────────────────────────────────────────────────────┐
│               FBS.API  (Presentation)               │
│    REST Controllers · GlobalExceptionHandler        │
│    OpenTelemetry (OTLP → New Relic)                 │
└──────────────────────────┬──────────────────────────┘
                           │
┌──────────────────────────▼──────────────────────────┐
│            FBS.Application  (Use Cases)             │
│  Commands · Queries · DTOs · FluentValidation       │
│  MediatR Pipelines: Logging·Validation·Transactions │
└──────────────────────────┬──────────────────────────┘
                           │
┌──────────────────────────▼──────────────────────────┐
│              FBS.Domain  (Business Logic)           │
│   Aggregates · Value Objects · Domain Events        │
│   Business Rules · Specifications · Result<T>       │
└──────────────────────────┬──────────────────────────┘
                           │
┌──────────────────────────▼──────────────────────────┐
│           FBS.Infrastructure  (Data & I/O)          │
│   EF Core · Repositories · UnitOfWork               │
│   ServiceBusEventPublisher · DomainEventDispatcher  │
└─────────────────────────────────────────────────────┘
```

### Key Patterns

| Pattern | Implementation |
|---|---|
| **CQRS** | Full separation: Commands (write) and Queries (read) via MediatR. Shared Azure SQL database. |
| **DDD** | Aggregates (`Flight`, `Reservation`), Value Objects (`FlightNumber`, `SeatNumber`, `Email`), Domain Events |
| **Repository + Unit of Work** | `IFlightRepository`, `IReservationRepository`, `IUnitOfWork` — EF Core implementations in Infrastructure |
| **Specification Pattern** | `ExpiredReservationsSpecification`, `ReservationsByFlightSpecification` for reusable query logic |
| **Pipeline Behaviors** | `ValidationBehavior` (FluentValidation), `TransactionBehavior` (EF Core), `LoggingBehavior` |
| **Result Pattern** | `Result<T>` with `ErrorType` instead of throwing exceptions for business failures |
| **Domain Events** | Raised inside aggregates, dispatched after `SaveChanges` via `IDomainEventDispatcher` |
| **Managed Identity** | Passwordless auth from App Service and Functions to SQL, Service Bus, Storage |

---

## ☁️ Azure Infrastructure

All infrastructure is described as code with **Bicep** (`infra/`) and deployed via GitHub Actions.

```
Azure Resource Group: rg-fbs-dev (West Europe)
│
├── App Service (Linux B1)          fbs-dev
│   └── FBS.API  →  Azure SQL (Poland Central)
│
├── Azure Function (Consumption)    fbns-func-dev
│   └── FBS.Function.Notifications
│       Service Bus Trigger → Mailtrap API
│
├── Azure Function (Consumption)    expire-func-dev
│   └── FBS.Function.ExpiredReservations
│       Timer Trigger (every 2 min) → Azure SQL
│
├── Azure Service Bus (Basic)       fbs-servicebus-dev
│   └── Queue: reservation-events
│
├── Azure SQL (Basic 5 DTU)         fbs-sql-server-dev / fbs-db-dev
│
├── Storage Account                 storagefbsdev
│   (Functions runtime state, Timer trigger locks)
│
└── Application Insights            fbs-dev
    (shared across all three applications)
```

### Managed Identity RBAC

| Principal | Resource | Role |
|---|---|---|
| `fbs-dev` | Azure SQL | `db_datareader` + `db_datawriter` |
| `fbs-dev` | Service Bus | `Azure Service Bus Data Sender` |
| `fbns-func-dev` | Service Bus | `Azure Service Bus Data Receiver` |
| `fbns-func-dev` | Storage | Blob Owner · Queue Contributor · Table Contributor |
| `expire-func-dev` | Azure SQL | `db_datareader` + `db_datawriter` |
| `expire-func-dev` | Service Bus | `Azure Service Bus Data Sender` |
| `expire-func-dev` | Storage | Blob Owner · Queue Contributor · Table Contributor |

---

## 🛠️ Technologies

### Core

| Technology | Version | Purpose |
|---|---|---|
| **.NET / ASP.NET Core** | 10.0 | API framework |
| **Entity Framework Core** | 10.0 | ORM, Code-First migrations |
| **Azure SQL** | — | Relational database |
| **MediatR** | 12.x | CQRS, pipeline behaviors |
| **FluentValidation** | 11.x | Request validation |

### Azure & Cloud

| Technology | Purpose |
|---|---|
| **Azure App Service** (Linux) | Hosts FBS.API |
| **Azure Functions v4** (Windows, isolated) | Notifications and expiration background processing |
| **Azure Service Bus** | Event-driven messaging between API and Functions |
| **Azure SQL** | Database with Entra-ID-only authentication |
| **Azure Storage** | Functions runtime state and distributed timer locks |
| **Azure Managed Identity** | Passwordless auth across all Azure services |

### Observability & DevOps

| Technology | Purpose |
|---|---|
| **Application Insights** | Telemetry, live metrics, log stream |
| **New Relic APM** | Distributed tracing, error inbox, performance dashboards |
| **OpenTelemetry** (OTLP/HTTP) | Vendor-neutral instrumentation for Linux App Service |
| **GitHub Actions** | 4 CI/CD pipelines (Infrastructure + 3 app deployments) |
| **Bicep** | Infrastructure as Code |
| **OIDC Workload Identity Federation** | Passwordless GitHub → Azure authentication |

### Libraries

| Library | Purpose |
|---|---|
| **Polly v8 / Microsoft.Extensions.Http.Resilience** | HTTP retry + circuit breaker |
| **Bogus** | Flight seed data generation |
| **Mailtrap API** | Email delivery (sandbox) |

---

## 📁 Project Structure

```
flight-booking-system/
├── src/
│   ├── FBS.API/                          # ASP.NET Core Web API (Linux App Service)
│   │   ├── Controllers/                  # FlightsController, ReservationsController
│   │   ├── DTOs/                         # Request DTOs
│   │   ├── Middlewares/                  # GlobalExceptionHandler (ValidationProblemDetails)
│   │   └── Program.cs                    # App bootstrap, OpenTelemetry setup
│   │
│   ├── FBS.Application/                  # Use cases (no framework dependencies)
│   │   ├── Commands/                     # CreateReservation, ConfirmReservation,
│   │   │                                 # CancelReservation, ExpireReservation
│   │   ├── Queries/                      # GetAvailableFlights, GetFlightByNumber,
│   │   │                                 # GetReservation
│   │   └── Common/
│   │       ├── Behaviors/                # ValidationBehavior, TransactionBehavior,
│   │       │                             # LoggingBehavior
│   │       └── Result/                   # Result<T>, ErrorType
│   │
│   ├── FBS.Domain/                       # Pure business logic, no dependencies
│   │   ├── Flight/                       # Flight aggregate, Seat, Value Objects,
│   │   │                                 # SeatReservedEvent, SeatReleasedEvent
│   │   ├── Reservation/                  # Reservation aggregate, Value Objects,
│   │   │                                 # ReservationCreated/Confirmed/Cancelled/ExpiredEvent
│   │   ├── Common/
│   │   │   ├── Base/                     # AggregateRoot<T>, Entity<T>, DomainEventBase
│   │   │   ├── Rules/                    # Business rule classes + IBusinessRule
│   │   │   └── Specifications/           # Specification<T>, ExpiredReservationsSpecification
│   │   ├── Repositories/                 # IFlightRepository, IReservationRepository, IUnitOfWork
│   │   └── SharedKernel/                 # Email, PhoneNumber, Airport value objects
│   │
│   ├── FBS.Infrastructure/               # EF Core, repositories, external services
│   │   ├── Persistence/                  # ApplicationDbContext, EF configs, migrations
│   │   ├── EventDispatcher/              # IDomainEventDispatcher, post-save dispatch
│   │   ├── Events/                       # ServiceBusEventPublisher, EventMapper, DTOs
│   │   ├── BackgroundJobs/               # ExpireReservationsJob (used by Azure Function)
│   │   └── Seed/                         # FlightDataSeeder (Bogus)
│   │
│   ├── FBS.Function.Notifications/       # Azure Function — Service Bus trigger
│   │   ├── Functions/                    # ProcessReservationEventFunction
│   │   │                                 # Complete/Abandon/DeadLetter settlement
│   │   ├── Notification/                 # NotificationService
│   │   ├── Email/                        # MailtrapApiEmailService, options
│   │   └── Templates/                    # EmailTemplates (HTML)
│   │
│   └── FBS.Function.ExpiredReservations/ # Azure Function — Timer trigger
│       └── Functions/                    # ExpireReservationsFunction (every 2 min)
│
├── infra/                                # Bicep Infrastructure as Code
│   ├── main.bicep
│   ├── main.bicepparam
│   └── modules/                          # app-insights, api-app, function-app,
│                                         # service-bus, sql, storage
│
├── .github/workflows/
│   ├── 01-infrastructure.yml             # Bicep deploy (on infra/** changes)
│   ├── 02-deploy-api.yml                 # Build + deploy FBS.API
│   ├── 03-deploy-notifications.yml       # Build + zip-deploy FBS.Function.Notifications
│   └── 04-deploy-expire.yml              # Build + zip-deploy FBS.Function.ExpiredReservations
│
├── postman/
│   ├── FBS-API.postman_collection.json   # 26 test cases with pre-request scripts
│   ├── FBS-Local.postman_environment.json
│   └── FBS-AzureDev.postman_environment.json
│
└── scripts/
    └── setup-rbac.ps1                    # One-time RBAC role assignment script
```

---

## 🔌 API Endpoints

### Flights

| Method | Endpoint | Description | Response |
|--------|----------|-------------|----------|
| `GET` | `/api/flights/search?departureAirport=BCN&arrivalAirport=ATL&date=2026-04-08` | Search flights by route and date | `200 FlightSummaryDto[]` / `400` |
| `GET` | `/api/flights/{flightNumber}` | Get flight details with full seat map | `200 FlightDetailsDto` / `400` / `404` |

### Reservations

| Method | Endpoint | Description | Response |
|--------|----------|-------------|----------|
| `POST` | `/api/reservations/create` | Create a new Pending reservation | `201 CreateReservationResponse` / `400` / `404` / `409` |
| `GET` | `/api/reservations/get/{id}` | Get reservation details | `200 ReservationDetailsDto` / `400` / `404` |
| `PUT` | `/api/reservations/confirm/{id}` | Confirm a Pending reservation | `204` / `400` / `404` / `409` |
| `DELETE` | `/api/reservations/cancel/{id}` | Cancel a Pending reservation | `204` / `400` / `404` / `409` |

### Example: Create Reservation

**POST** `/api/reservations/create`

```json
{
  "flightNumber": "AA387",
  "seatNumber": "5C",
  "firstName": "John",
  "lastName": "Doe",
  "email": "john.doe@example.com",
  "phone": "+380971234567"
}
```

**201 Created:**
```json
{
  "reservationId": "3fa85f64-5717-4562-b3fc-2c963f66afa6",
  "flightId": "fb29e71f-5219-4dc9-8b8b-daf709de2afb",
  "flightNumber": "AA387",
  "seatNumber": "5C",
  "passengerName": "John Doe",
  "status": "Pending",
  "createdAt": "2026-04-08T10:00:00Z",
  "expiresAt": "2026-04-08T10:10:00Z"
}
```

---

## 🎯 Domain Model

### Aggregates

**Flight**
- `Flight` (AggregateRoot) → owns `Seat` collection
- Value Objects: `FlightId`, `FlightNumber`, `SeatNumber`, `Airport`
- Domain Events: `SeatReservedEvent`, `SeatReleasedEvent`
- Business Rules: seat must be available to reserve; seat must be reserved to release

**Reservation**
- `Reservation` (AggregateRoot)
- Value Objects: `ReservationId`, `PassengerInfo`, `Email`, `PhoneNumber`
- States: `Pending → Confirmed | Cancelled | Expired`
- Domain Events: `ReservationCreatedEvent`, `ReservationConfirmedEvent`, `ReservationCancelledEvent`, `ReservationExpiredEvent`
- Business Rules:
  - `ReservationMustBePendingToConfirmRule`
  - `ReservationMustNotBeExpiredToConfirmRule`
  - `CannotCancelNonPendingReservationRule` (Pending only; Confirmed/Expired/Cancelled → 409)
  - `OnlyPendingReservationsCanExpireRule`

### Event Flow

```
FBS.API
  └─► CreateReservationCommandHandler
        ├─► Flight.ReserveSeat()  → SeatReservedEvent (internal)
        ├─► Reservation.Create()  → ReservationCreatedEvent
        └─► UnitOfWork.SaveChanges()
              └─► DomainEventDispatcher
                    └─► ServiceBusEventPublisher
                          └─► Service Bus Queue: reservation-events
                                └─► FBS.Function.Notifications
                                      └─► MailtrapApiEmailService
```

---

## 🚀 Getting Started

### Prerequisites

- [.NET 10 SDK](https://dotnet.microsoft.com/download/dotnet/10.0)
- SQL Server (LocalDB is sufficient for local development)
- [Azure Functions Core Tools v4](https://learn.microsoft.com/en-us/azure/azure-functions/functions-run-local) (for running Functions locally)
- [Azurite](https://learn.microsoft.com/en-us/azure/storage/common/storage-use-azurite) (local Azure Storage emulator)
- Azure CLI (`az login` for Managed Identity local development)

### Local Setup

**1. Clone the repository**

```bash
git clone https://github.com/komenday/flight-booking-system.git
cd flight-booking-system
```

**2. Configure `appsettings.Development.json` in `FBS.API`**

```json
{
  "ConnectionStrings": {
    "DefaultConnection": "Server=(localdb)\\mssqllocaldb;Database=FBS_Dev;Trusted_Connection=True;"
  }
}
```

**3. Apply migrations and seed data**

```bash
cd src/FBS.Infrastructure
dotnet ef database update --startup-project ../FBS.API
```

Migrations run automatically on startup. Seed data (15 random flights) is inserted if the Flights table is empty.

**4. Run the API**

```bash
cd src/FBS.API
dotnet run
```

Swagger UI: [http://localhost:5000/swagger](http://localhost:5000/swagger)

**5. Run Functions locally (optional)**

Start Azurite, then in separate terminals:

```bash
cd src/FBS.Function.Notifications
func start

cd src/FBS.Function.ExpiredReservations
func start
```

### Database Migrations

```bash
# Add a new migration
dotnet ef migrations add MigrationName \
  --project src/FBS.Infrastructure \
  --startup-project src/FBS.API

# Apply migrations
dotnet ef database update \
  --project src/FBS.Infrastructure \
  --startup-project src/FBS.API
```

---

## ⚙️ Configuration

### FBS.API — `appsettings.json`

```json
{
  "ConnectionStrings": {
    "DefaultConnection": ""
  },
  "ServiceBus": {
    "FullyQualifiedNamespace": "fbs-servicebus-dev.servicebus.windows.net",
    "QueueName": "reservation-events",
    "TimeoutSeconds": 30,
    "MaxRetryAttempts": 3
  },
  "EventPublisher": {
    "BaseUrl": "",
    "RetryCount": 3,
    "TimeoutSeconds": 30
  },
  "NewRelic": {
    "LicenseKey": "",
    "ServiceName": "FBS-API",
    "OtlpEndpoint": "https://otlp.eu01.nr-data.net:4318"
  }
}
```

### FBS.Function.Notifications — `local.settings.json`

```json
{
  "IsEncrypted": false,
  "Values": {
    "AzureWebJobsStorage": "UseDevelopmentStorage=true",
    "FUNCTIONS_WORKER_RUNTIME": "dotnet-isolated",
    "ServiceBusConnection__fullyQualifiedNamespace": "fbs-servicebus-dev.servicebus.windows.net",
    "MailtrapApi__ApiToken": "YOUR_TOKEN",
    "MailtrapApi__InboxId": "YOUR_INBOX_ID",
    "MailtrapApi__FromEmail": "noreply@flightbooking.com",
    "MailtrapApi__FromName": "Flight Booking System"
  }
}
```

### FBS.Function.ExpiredReservations — `local.settings.json`

```json
{
  "IsEncrypted": false,
  "Values": {
    "AzureWebJobsStorage": "UseDevelopmentStorage=true",
    "FUNCTIONS_WORKER_RUNTIME": "dotnet-isolated",
    "ServiceBus__FullyQualifiedNamespace": "fbs-servicebus-dev.servicebus.windows.net",
    "ServiceBus__QueueName": "reservation-events",
    "EventPublisher__BaseUrl": "https://placeholder.local",
    "EventPublisher__RetryCount": "3",
    "EventPublisher__TimeoutSeconds": "30"
  },
  "ConnectionStrings": {
    "DefaultConnection": "Server=(localdb)\\mssqllocaldb;Database=FBS_Dev;Trusted_Connection=True;"
  }
}
```

---

## 🔄 CI/CD

Four GitHub Actions workflows with OIDC Workload Identity Federation (no client secrets):

| Workflow | Trigger | What it does |
|---|---|---|
| `01-infrastructure.yml` | Push to `infra/**` or manual | Deploys Bicep — validates then applies Incremental |
| `02-deploy-api.yml` | Push to `src/FBS.API/**` or shared layers | Build → Publish → Configure App Settings → Deploy → Health check |
| `03-deploy-notifications.yml` | Push to `src/FBS.Function.Notifications/**` | Build → Publish → Zip → Configure App Settings → Zip Deploy |
| `04-deploy-expire.yml` | Push to `src/FBS.Function.ExpiredReservations/**` | Build → Publish → Zip → Configure App Settings → Zip Deploy |

### Required GitHub Secrets

| Secret | Used in |
|---|---|
| `AZURE_CLIENT_ID` | All workflows (OIDC) |
| `AZURE_TENANT_ID` | All workflows (OIDC) |
| `AZURE_SUBSCRIPTION_ID` | All workflows (OIDC) |
| `SQL_CONNECTION_STRING` | `02`, `04` |
| `MAILTRAP_API_TOKEN` | `03` |
| `MAILTRAP_INBOX_ID` | `03` |
| `NEW_RELIC_LICENSE_KEY` | `02`, `03`, `04` |

---

## 📊 Monitoring

### Application Insights

Shared across all three applications. Filter by `cloud_RoleName` in Log Analytics:

```kusto
traces
| where cloud_RoleName == "fbs-dev"
| order by timestamp desc
```

### New Relic APM

**FBS.API (Linux)** — OpenTelemetry OTLP integration:
- Distributed traces for all HTTP requests and outbound HttpClient calls
- Configurable via `NewRelic:*` app settings
- Skips `/health` endpoint to reduce noise

**FBS.Function.Notifications / FBS.Function.ExpiredReservations (Windows)** — Native New Relic .NET Agent via `NewRelic.Agent` NuGet package:
- Auto-instruments Service Bus message processing, HTTP, async workflows
- Configured via `NEW_RELIC_*` environment variables + `CORECLR_*` profiler settings

All three services appear under **APM & Services** in New Relic UI.

---

## 📄 License

This project is licensed under the MIT License.

---

## 🔗 Links

- **Repository**: [github.com/komenday/flight-booking-system](https://github.com/komenday/flight-booking-system)
- **Live API**: [fbs-dev.azurewebsites.net/swagger](https://fbs-dev.azurewebsites.net/swagger)
