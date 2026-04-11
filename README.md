# 🧠 MyTherapy — Online Mental Health Platform

> A graduation project backend API built with **ASP.NET Core** following **Clean Architecture** principles. MyTherapy connects patients with licensed therapists, enabling appointment booking, secure messaging, video sessions, and AI-powered mental health support.

---

## 📋 Table of Contents

- [🧠 MyTherapy — Online Mental Health Platform](#-mytherapy--online-mental-health-platform)
  - [📋 Table of Contents](#-table-of-contents)
  - [Overview](#overview)
  - [Architecture](#architecture)
    - [Why Clean Architecture?](#why-clean-architecture)
  - [Tech Stack](#tech-stack)
  - [Features](#features)
    - [✅ Implemented](#-implemented)
    - [🔄 In Progress](#-in-progress)
  - [Getting Started](#getting-started)
    - [Prerequisites](#prerequisites)
    - [Installation](#installation)
  - [Project Structure](#project-structure)
  - [API Endpoints](#api-endpoints)
    - [Auth](#auth)
    - [Admin](#admin)
    - [Patient](#patient)
    - [Therapist](#therapist)
  - [Database Schema](#database-schema)
  - [Roadmap](#roadmap)
  - [Team](#team)

---

## Overview

MyTherapy is a full-stack mental health platform that bridges the gap between patients and therapists. The backend provides a secure, scalable REST API that handles:

- 🔐 Authentication & role-based authorization (Patient / Therapist / Admin)
- 📅 Appointment scheduling with availability management
- 💳 Payment processing for session booking
- 🎥 Video call session management
- 💬 Real-time messaging between patients and therapists
- 🤖 AI-powered mood analysis and mental health recommendations
- ⭐ Rating and review system for therapists
- 🛡️ Admin dashboard for platform management

---

## Architecture

The project follows **Clean Architecture** (also known as Onion Architecture), separating concerns across four layers:

```
MyTherapy/
├── MyTherapy.API              # Presentation Layer — Controllers, Middleware
├── MyTherapy.Application      # Application Layer — DTOs, Interfaces, Business Logic
├── MyTherapy.Domain           # Domain Layer — Entities, Enums, Base Classes
└── MyTherapy.Infrastructure   # Infrastructure Layer — EF Core, Services, Persistence
```

### Why Clean Architecture?

- **Independence** — Domain layer has zero external dependencies
- **Testability** — Business logic can be unit tested without the database
- **Maintainability** — Each layer has a single responsibility
- **Scalability** — Easy to swap implementations (e.g., change payment gateway)

---

## Tech Stack

| Layer            | Technology                   |
| ---------------- | ---------------------------- |
| Framework        | ASP.NET Core 9               |
| Language         | C# 13                        |
| ORM              | Entity Framework Core        |
| Database         | SQL Server                   |
| Authentication   | JWT Bearer Tokens            |
| Password Hashing | BCrypt.Net                   |
| API Docs         | Swagger / OpenAPI            |
| Validation       | FluentValidation _(planned)_ |
| Logging          | Serilog _(planned)_          |
| Payments         | Paymob / Stripe _(planned)_  |
| Video Calls      | Agora / WebRTC _(planned)_   |

---

## Features

### ✅ Implemented

- **JWT Authentication** — Register & login for patients, therapists, and admins
- **Role-based Authorization** — Route protection per role
- **Therapist Verification** — Admin approves/rejects therapist license documents
- **Availability Management** — Therapists create and manage time slots
- **Appointment Booking** — Patients browse available slots and book sessions
- **Global Exception Handling** — Consistent error responses across all endpoints
- **Database Seeding** — Auto-creates admin account on first run

### 🔄 In Progress

- Payment integration
- Session management & video calls
- Real-time messaging
- AI module integration
- Ratings & reviews
- Advanced admin dashboard

---

## Getting Started

### Prerequisites

- [.NET 9 SDK](https://dotnet.microsoft.com/download)
- [SQL Server](https://www.microsoft.com/en-us/sql-server) (or SQL Server Express)
- [Visual Studio 2022](https://visualstudio.microsoft.com/) or [VS Code](https://code.visualstudio.com/)

### Installation

**1. Clone the repository**

```bash
git clone https://github.com/yourusername/MyTherapy.git
cd MyTherapy
```

**2. Configure the database connection**

In `MyTherapy.API/appsettings.json`:

```json
{
  "ConnectionStrings": {
    "DefaultConnection": "Server=YOUR_SERVER;Database=MyTherapyDb;Trusted_Connection=True;TrustServerCertificate=True"
  },
  "Jwt": {
    "Key": "your-super-secret-key-min-32-characters",
    "Issuer": "MyTherapy",
    "Audience": "MyTherapyUsers",
    "DurationInMinutes": "60"
  }
}
```

**3. Apply database migrations**

```bash
dotnet ef database update --project MyTherapy.Infrastructure --startup-project MyTherapy.API
```

**4. Run the API**

```bash
cd MyTherapy.API
dotnet run
```

**5. Open Swagger UI**

```
https://localhost:{port}/swagger
```

> 💡 A default admin account is automatically seeded on first run:
>
> - **Email:** `admin@mytherapy.com`
> - **Password:** `Admin@123`

---

## Project Structure

```
MyTherapy.Domain/
├── Common/
│   └── BaseEntity.cs               # Id, CreatedAt
├── Entities/
│   ├── User.cs
│   ├── PatientProfile.cs
│   ├── TherapistProfile.cs
│   ├── AdminProfile.cs
│   ├── AvailabilitySlot.cs
│   ├── Appointment.cs
│   ├── Payment.cs
│   ├── Session.cs
│   ├── Conversation.cs
│   ├── Message.cs
│   └── Notification.cs
└── Enums/
    ├── Role.cs
    ├── Gender.cs
    ├── UserStatus.cs
    ├── AppointmentStatus.cs
    ├── VerificationStatus.cs
    ├── PaymentStatus.cs
    ├── PaymentMethod.cs
    ├── MessageType.cs
    ├── NotificationType.cs
    └── SessionAnalysisStatus.cs

MyTherapy.Application/
├── DTOs/
│   ├── Auth/
│   │   ├── LoginRequest.cs
│   │   ├── RegisterRequest.cs
│   │   ├── RegisterTherapistRequest.cs
│   │   └── AuthResponse.cs
│   ├── Slots/
│   │   ├── CreateSlotRequest.cs
│   │   └── SlotResponse.cs
│   ├── Appointments/
│   │   ├── CreateAppointmentRequest.cs
│   │   └── AppointmentResponse.cs
│   └── Therapists/
│       └── TherapistResponse.cs
└── Interfaces/
    └── IAuthService.cs

MyTherapy.Infrastructure/
├── Persistence/
│   ├── AppDbContext.cs
│   ├── DbInitializer.cs
│   └── Migrations/
└── Services/
    └── AuthService.cs

MyTherapy.API/
├── Controllers/
│   ├── AuthController.cs
│   ├── AdminTherapistsController.cs
│   ├── PatientAvailabilityController.cs
│   ├── PatientAppointmentController.cs
│   └── TherapistAvailabilityController.cs
├── Middleware/
│   └── ExceptionMiddleware.cs
└── Program.cs
```

---

## API Endpoints

### Auth

| Method | Endpoint                       | Description              | Auth |
| ------ | ------------------------------ | ------------------------ | ---- |
| POST   | `/api/auth/register/patient`   | Register a new patient   | ❌   |
| POST   | `/api/auth/register/therapist` | Register a new therapist | ❌   |
| POST   | `/api/auth/login`              | Login and get JWT token  | ❌   |

### Admin

| Method | Endpoint                             | Description                          | Auth  |
| ------ | ------------------------------------ | ------------------------------------ | ----- |
| GET    | `/api/admin/therapists/pending`      | List pending therapist verifications | Admin |
| POST   | `/api/admin/therapists/{id}/approve` | Approve a therapist                  | Admin |
| POST   | `/api/admin/therapists/{id}/reject`  | Reject a therapist                   | Admin |

### Patient

| Method | Endpoint                       | Description            | Auth    |
| ------ | ------------------------------ | ---------------------- | ------- |
| GET    | `/api/patient/availability`    | Browse available slots | Patient |
| POST   | `/api/patient/appointments`    | Book an appointment    | Patient |
| GET    | `/api/patient/appointments/my` | View my appointments   | Patient |

### Therapist

| Method | Endpoint                           | Description              | Auth      |
| ------ | ---------------------------------- | ------------------------ | --------- |
| POST   | `/api/therapist/availability`      | Create availability slot | Therapist |
| GET    | `/api/therapist/availability/my`   | View my slots            | Therapist |
| DELETE | `/api/therapist/availability/{id}` | Delete a slot            | Therapist |

---

## Database Schema

The database follows a normalized relational design with the following core tables:

- **Users** — Base account info for all roles
- **PatientProfiles / TherapistProfiles / AdminProfiles** — Role-specific profile data
- **AvailabilitySlots** — Therapist time slots
- **Appointments** — Booked sessions between patient and therapist
- **Payments** — Transaction records linked to appointments
- **Sessions** — Video call session data with AI analysis
- **Conversations & Messages** — In-app messaging system
- **Notifications** — System, payment, and reminder notifications

---

## Roadmap

- [x] Phase 1 — Project Setup & Architecture
- [x] Phase 2 — Authentication & Authorization
- [x] Phase 3 — Therapist Verification (Admin)
- [x] Phase 4 — Availability & Appointment Booking
- [ ] Phase 5 — Payment Integration (Paymob)
- [ ] Phase 6 — Video Session Management (Agora)
- [ ] Phase 7 — AI Module Integration
- [ ] Phase 8 — Ratings & Reviews
- [ ] Phase 9 — Advanced Admin Dashboard
- [ ] Phase 10 — Security & Performance Optimization
- [ ] Phase 11 — Deployment & Finalization

---

## Team

> This project was built as a graduation project for the **ASP.NET Core Backend Development Track**.

| Role              | Name            |
| ----------------- | --------------- |
| Backend Developer | _Your Name_     |
| AI / ML           | _Teammate Name_ |
| Frontend          | _Teammate Name_ |

---

<p align="center">Made with ❤️ for mental health awareness</p>
