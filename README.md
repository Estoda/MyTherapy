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
    - [Setting up Paymob Webhooks (local development)](#setting-up-paymob-webhooks-local-development)
  - [Project Structure](#project-structure)
  - [API Endpoints](#api-endpoints)
    - [Auth](#auth)
    - [Admin](#admin)
    - [Patient](#patient)
    - [Therapist](#therapist)
    - [Payment](#payment)
    - [Reviews](#reviews)
  - [Payment Flow](#payment-flow)
  - [Database Schema](#database-schema)
  - [Roadmap](#roadmap)
  - [Team](#team)

---

## Overview

MyTherapy is a full-stack mental health platform that bridges the gap between patients and therapists. The backend provides a secure, scalable REST API that handles:

- 🔐 Authentication & role-based authorization (Patient / Therapist / Admin)
- 📅 Appointment scheduling with availability management
- 💳 Payment processing via Paymob before session confirmation
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
| Payment Gateway  | Paymob ✅                    |
| Validation       | FluentValidation _(planned)_ |
| Logging          | Serilog _(planned)_          |
| Video Calls      | Agora / WebRTC _(planned)_   |

---

## Features

### ✅ Implemented

- **JWT Authentication** — Register & login for patients, therapists, and admins
- **Role-based Authorization** — Route protection per role (Patient / Therapist / Admin)
- **Therapist Verification** — Admin approves/rejects therapist license documents
- **Availability Management** — Therapists create and manage time slots
- **Appointment Booking** — Patients browse available slots and initiate booking
- **Payment Integration** — Full Paymob payment flow before appointment confirmation
- **Webhook Handling** — Paymob webhook updates payment & appointment status automatically
- **Ratings & Reviews** — Patients rate therapists after completed sessions (1–5 stars)
- **Running Rating Average** — TherapistProfile rating auto-updated on every new review
- **Duplicate Review Prevention** — One review per appointment enforced at DB level
- **Global Exception Handling** — Consistent JSON error responses across all endpoints
- **Circular Reference Protection** — Safe JSON serialization with IgnoreCycles
- **Database Seeding** — Auto-creates admin account on first run
- **Response DTOs** — No sensitive data (e.g. PasswordHash) ever exposed in API responses

### 🔄 In Progress

- Session management & video calls (Phase 6)
- AI module integration (Phase 7)
- Advanced admin dashboard (Phase 9)
- Security & performance optimization (Phase 10)

---

## Getting Started

### Prerequisites

- [.NET 9 SDK](https://dotnet.microsoft.com/download)
- [SQL Server](https://www.microsoft.com/en-us/sql-server) (or SQL Server Express)
- [Visual Studio 2022](https://visualstudio.microsoft.com/) or [VS Code](https://code.visualstudio.com/)
- [Paymob account](https://paymob.com) for payment integration

### Installation

**1. Clone the repository**

```bash
git clone https://github.com/yourusername/MyTherapy.git
cd MyTherapy
```

**2. Configure `appsettings.json`**

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
  },
  "Paymob": {
    "ApiKey": "your_paymob_api_key",
    "IntegrationId": "your_integration_id",
    "IframeId": "your_iframe_id",
    "BaseUrl": "https://accept.paymob.com/api"
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

### Setting up Paymob Webhooks (local development)

Use [ngrok](https://ngrok.com) to expose your local API to Paymob:

```bash
ngrok http https://localhost:7114
```

Then set both callback URLs in Paymob Dashboard → Developers → Payment Integrations:

```
Transaction processed callback: https://YOUR-NGROK-URL/api/payment/webhook
Transaction response callback:  https://YOUR-NGROK-URL/api/payment/webhook
```

---

## Project Structure

```
MyTherapy.Domain/
├── Common/
│   └── BaseEntity.cs
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
│   ├── Notification.cs
│   └── Review.cs
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
│   ├── Payment/
│   │   ├── PaymentInitiateRequest.cs
│   │   └── PaymentInitiateResponse.cs
│   ├── Reviews/
│   │   ├── CreateReviewRequest.cs
│   │   └── ReviewResponse.cs
│   └── Therapists/
│       └── TherapistResponse.cs
└── Interfaces/
    ├── IAuthService.cs
    └── IPaymobService.cs

MyTherapy.Infrastructure/
├── Persistence/
│   ├── AppDbContext.cs
│   ├── DbInitializer.cs
│   └── Migrations/
└── Services/
    ├── AuthService.cs
    └── PaymobService.cs

MyTherapy.API/
├── Controllers/
│   ├── AuthController.cs
│   ├── AdminTherapistsController.cs
│   ├── PatientAvailabilityController.cs
│   ├── PatientAppointmentController.cs
│   ├── TherapistAvailabilityController.cs
│   ├── PaymentController.cs
│   └── ReviewController.cs
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
| GET    | `/api/patient/appointments/my` | View my appointments   | Patient |

### Therapist

| Method | Endpoint                           | Description              | Auth      |
| ------ | ---------------------------------- | ------------------------ | --------- |
| POST   | `/api/therapist/availability`      | Create availability slot | Therapist |
| GET    | `/api/therapist/availability/my`   | View my slots            | Therapist |
| DELETE | `/api/therapist/availability/{id}` | Delete a slot            | Therapist |

### Payment

| Method | Endpoint                | Description                                           | Auth             |
| ------ | ----------------------- | ----------------------------------------------------- | ---------------- |
| POST   | `/api/payment/initiate` | Initiate Paymob payment for a slot                    | Patient          |
| POST   | `/api/payment/webhook`  | Paymob webhook — updates payment & appointment status | ❌ (Paymob only) |

### Reviews

| Method | Endpoint                               | Description                                      | Auth    |
| ------ | -------------------------------------- | ------------------------------------------------ | ------- |
| POST   | `/api/reviews`                         | Submit a review for a completed appointment      | Patient |
| GET    | `/api/reviews/therapist/{therapistId}` | Get all reviews + rating average for a therapist | ❌      |

---

## Payment Flow

```
Patient calls POST /api/payment/initiate { slotId }
        ↓
API creates Appointment (Scheduled) + Payment (Pending)
        ↓
API calls Paymob → returns { paymentUrl, appointmentId }
        ↓
Frontend opens paymentUrl (Paymob iframe)
        ↓
Patient enters card details on Paymob
        ↓
Paymob calls POST /api/payment/webhook
        ↓
✅ Success → Payment = Successful, Appointment = Confirmed
❌ Failure → Payment = Failed, Appointment = Cancelled, Slot freed
```

---

## Database Schema

The database follows a normalized relational design with the following core tables:

- **Users** — Base account info for all roles
- **PatientProfiles / TherapistProfiles / AdminProfiles** — Role-specific profile data
- **AvailabilitySlots** — Therapist time slots
- **Appointments** — Booked sessions between patient and therapist (linked to slot & payment)
- **Payments** — Transaction records with Paymob transaction ID, status, and method
- **Reviews** — Patient reviews linked to appointments; auto-updates therapist rating average
- **Sessions** — Video call session data with AI analysis
- **Conversations & Messages** — In-app messaging system
- **Notifications** — System, payment, and reminder notifications

---

## Roadmap

- [x] Phase 1 — Project Setup & Clean Architecture
- [x] Phase 2 — Authentication & JWT Authorization
- [x] Phase 3 — Therapist Verification (Admin)
- [x] Phase 4 — Availability & Appointment Booking
- [x] Phase 5 — Payment Integration (Paymob) ✅
- [ ] Phase 6 — Video Session Management
- [ ] Phase 7 — AI Module Integration
- [x] Phase 8 — Ratings & Reviews ✅
- [ ] Phase 9 — Advanced Admin Dashboard
- [ ] Phase 10 — Security & Performance Optimization
- [ ] Phase 11 — Deployment & Finalization

---

## Team

> This project was built as a graduation project for the **ASP.NET Core Backend Development Track**.

| Role              | Name           |
| ----------------- | -------------- |
| Backend Developer | Ahmed Amin     |
| AI / ML           | Ahmed Magdy    |
| AI / ML           | Mohamed Younes |
| Frontend          | Duaa Magdy     |
| Frontend          | Menna Mohamed  |

---

<p align="center">Made with ❤️ for mental health awareness</p>
