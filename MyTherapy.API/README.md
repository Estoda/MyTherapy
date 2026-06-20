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
    - [Profile](#profile)
    - [Patient](#patient)
    - [Therapist](#therapist)
    - [Payment](#payment)
    - [Reviews](#reviews)
    - [Users (Directory)](#users-directory)
  - [Booking \& Payment Flow](#booking--payment-flow)
  - [Database Schema](#database-schema)
  - [Roadmap](#roadmap)
  - [Team](#team)

---

## Overview

MyTherapy is a full-stack mental health platform that bridges the gap between patients and therapists. The backend provides a secure, scalable REST API that handles:

- 🔐 Authentication & role-based authorization (Patient / Therapist / Admin)
- 📧 Email verification before account creation
- 📅 Appointment booking with availability management
- 💳 Payment processing via Paymob after booking, before session confirmation
- 🪪 Therapist license verification, with re-submission after rejection
- 👥 Public browsing of patients & therapists (summary + detailed views)
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
├── MyTherapy.API              # Presentation Layer — Controllers, Middleware, Filters
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
| Email            | MailKit (Gmail SMTP)         |
| API Docs         | Swagger / OpenAPI            |
| Payment Gateway  | Paymob ✅                    |
| Validation       | FluentValidation _(planned)_ |
| Logging          | Serilog _(planned)_          |
| Video Calls      | Agora / WebRTC _(planned)_   |

---

## Features

### ✅ Implemented

- **JWT Authentication** — Register & login for patients, therapists, and admins
- **Email Verification** — 6-digit code sent via Gmail SMTP before account creation; codes expire after 10 minutes
- **Role-based Authorization** — Route protection per role (Patient / Therapist / Admin)
- **Therapist Verification** — Admin approves/rejects therapist license documents
- **License Re-submission** — Rejected therapists can re-upload their license, which resets their status back to Pending for another admin review
- **Verification Status Check** — Therapists (including unverified ones) can check their own verification status at any time
- **Availability Management** — Therapists create, view, and delete time slots; past slots are automatically excluded
- **Appointment Booking** — Patients book a slot directly (`Scheduled` status); the slot is immediately marked as booked
- **Payment-After-Booking Flow** — Patients pay for an existing appointment via Paymob; payment is tied to the appointment, not the raw slot
- **Webhook Handling** — Paymob webhook updates payment & appointment status automatically; failed payments free the slot back up
- **User Directory** — Authenticated users can browse summary lists of all patients/therapists, and view full details by ID
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
- A Gmail account with [App Password](https://myaccount.google.com/apppasswords) enabled for email verification

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
  },
  "Email": {
    "From": "your-gmail@gmail.com",
    "Password": "your-gmail-app-password",
    "Host": "smtp.gmail.com",
    "Port": "587"
  }
}
```

> ⚠️ Never commit real credentials to source control. Use environment variables or `appsettings.Development.json` for local secrets.
> ⚠️ Paymob amounts are sent in **cents**, not whole currency units — `PricePerSession` is multiplied by 100 before being sent to Paymob.

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

> ⚠️ The ngrok URL changes on every restart — remember to update it in the Paymob dashboard each time.

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
│   ├── Review.cs
│   └── EmailVerification.cs
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
│   │   ├── AuthResponse.cs
│   │   ├── SendVerificationCodeRequest.cs
│   │   └── VerifyEmailRequest.cs
│   ├── Slots/
│   │   ├── CreateSlotRequest.cs
│   │   ├── PatientSlotResponse.cs
│   │   └── TherapistSlotResponse.cs
│   ├── Appointments/
│   │   ├── CreateAppointmentRequest.cs
│   │   └── AppointmentResponse.cs
│   ├── Payment/
│   │   ├── PaymentInitiateRequest.cs
│   │   └── PaymentInitiateResponse.cs
│   ├── Reviews/
│   │   ├── CreateReviewRequest.cs
│   │   └── ReviewResponse.cs
│   ├── Therapists/
│   │   └── VerificationStatusResponse.cs
│   └── Users/
│       ├── UserSummaryResponse.cs
│       ├── PatientDetailsResponse.cs
│       └── TherapistDetailsResponse.cs
└── Interfaces/
    ├── IAuthService.cs
    ├── IEmailService.cs
    ├── IProfileService.cs
    └── IPaymobService.cs

MyTherapy.Infrastructure/
├── Persistence/
│   ├── AppDbContext.cs
│   ├── DbInitializer.cs
│   └── Migrations/
└── Services/
    ├── AuthService.cs
    ├── EmailService.cs
    ├── ProfileService.cs
    └── PaymobService.cs

MyTherapy.API/
├── Controllers/
│   ├── AuthController.cs
│   ├── AdminTherapistsController.cs
│   ├── ProfileController.cs
│   ├── PatientAvailabilityController.cs
│   ├── PatientBookingController.cs
│   ├── TherapistAvailabilityController.cs
│   ├── PaymentController.cs
│   ├── ReviewController.cs
│   └── UsersController.cs
├── Filters/
│   └── VerifiedTherapistFilter.cs
├── Middleware/
│   └── ExceptionMiddleware.cs
└── Program.cs
```

---

## API Endpoints

### Auth

| Method | Endpoint                           | Description                                       | Auth |
| ------ | ---------------------------------- | ------------------------------------------------- | ---- |
| POST   | `/api/auth/send-verification-code` | Send 6-digit code to email (expires in 10 min)    | ❌   |
| POST   | `/api/auth/verify-email`           | Verify the emailed code before registration       | ❌   |
| POST   | `/api/auth/register/patient`       | Register a new patient (email must be verified)   | ❌   |
| POST   | `/api/auth/register/therapist`     | Register a new therapist (email must be verified) | ❌   |
| POST   | `/api/auth/login`                  | Login and get JWT token                           | ❌   |

### Admin

| Method | Endpoint                             | Description                          | Auth  |
| ------ | ------------------------------------ | ------------------------------------ | ----- |
| GET    | `/api/admin/therapists/pending`      | List pending therapist verifications | Admin |
| POST   | `/api/admin/therapists/{id}/approve` | Approve a therapist                  | Admin |
| POST   | `/api/admin/therapists/{id}/reject`  | Reject a therapist                   | Admin |

### Profile

| Method | Endpoint                           | Description                                                                                       | Auth      |
| ------ | ---------------------------------- | ------------------------------------------------------------------------------------------------- | --------- |
| POST   | `/api/profile/upload-picture`      | Upload/update profile picture                                                                     | Any role  |
| POST   | `/api/profile/upload-license`      | Upload license document; resets status to `Pending` (also used for re-submission after rejection) | Therapist |
| GET    | `/api/profile/verification-status` | Check own verification status — accessible even while `Pending`/`Rejected`                        | Therapist |

> ⚠️ `VerifiedTherapistFilter` blocks unverified therapists from most therapist-only actions, but explicitly allows `verification-status` and `upload-license` so a rejected therapist can check their status and retry.

### Patient

| Method | Endpoint                    | Description                                                        | Auth    |
| ------ | --------------------------- | ------------------------------------------------------------------ | ------- |
| GET    | `/api/patient/availability` | Browse available (unbooked, future) slots with therapist info      | Patient |
| POST   | `/api/patient/bookings`     | Book a slot → creates `Appointment` (Scheduled), marks slot booked | Patient |
| GET    | `/api/patient/bookings/my`  | View my appointments                                               | Patient |

### Therapist

| Method | Endpoint                           | Description                                                       | Auth      |
| ------ | ---------------------------------- | ----------------------------------------------------------------- | --------- |
| POST   | `/api/therapist/availability`      | Create availability slot (must be in the future, end after start) | Therapist |
| GET    | `/api/therapist/availability/my`   | View my future slots, including booked ones with patient info     | Therapist |
| DELETE | `/api/therapist/availability/{id}` | Delete a slot                                                     | Therapist |

### Payment

| Method | Endpoint                | Description                                                                  | Auth             |
| ------ | ----------------------- | ---------------------------------------------------------------------------- | ---------------- |
| POST   | `/api/payment/initiate` | Initiate Paymob payment for an **existing appointment** (`appointmentId`)    | Patient          |
| POST   | `/api/payment/webhook`  | Paymob webhook — updates payment & appointment status, frees slot on failure | ❌ (Paymob only) |

### Reviews

| Method | Endpoint                               | Description                                                     | Auth    |
| ------ | -------------------------------------- | --------------------------------------------------------------- | ------- |
| POST   | `/api/reviews`                         | Submit a review for a completed appointment (1 per appointment) | Patient |
| GET    | `/api/reviews/therapist/{therapistId}` | Get all reviews + rating average for a therapist                | ❌      |

### Users (Directory)

| Method | Endpoint                     | Description                                                            | Auth                   |
| ------ | ---------------------------- | ---------------------------------------------------------------------- | ---------------------- |
| GET    | `/api/users/patients`        | List all patients (name + profile picture only)                        | Any authenticated user |
| GET    | `/api/users/therapists`      | List all **approved** therapists (name + profile picture only)         | Any authenticated user |
| GET    | `/api/users/patients/{id}`   | Get full patient details by ID                                         | Any authenticated user |
| GET    | `/api/users/therapists/{id}` | Get full therapist details by ID (specialization, price, rating, etc.) | Any authenticated user |

---

## Booking & Payment Flow

Booking and payment are two separate steps — the patient secures the slot first, then pays for it:

```
1. Patient browses available slots
   GET /api/patient/availability
        ↓
2. Patient books a slot
   POST /api/patient/bookings { slotId }
        ↓
   API creates Appointment (Scheduled) + marks slot.IsBooked = true
        ↓
3. Patient initiates payment for that appointment
   POST /api/payment/initiate { appointmentId }
        ↓
   API creates Payment (Pending), calls Paymob, returns { paymentUrl, appointmentId }
        ↓
4. Frontend opens paymentUrl (Paymob iframe)
   Patient enters card details
        ↓
5. Paymob calls POST /api/payment/webhook automatically
        ↓
   ✅ Success → Payment = Successful, Appointment stays Scheduled
   ❌ Failure → Payment = Failed, Appointment = Cancelled, slot.IsBooked = false (slot freed)
```

Paymob credentials stored in `appsettings.json`:

```json
"Paymob": {
  "ApiKey": "...",
  "IntegrationId": "...",
  "IframeId": "...",
  "BaseUrl": "https://accept.paymob.com/api"
}
```

> 💡 Paymob requires `amount_cents` ≥ 10 — `PricePerSession` (in EGP) is multiplied by 100 before being sent.

---

## Database Schema

The database follows a normalized relational design with the following core tables:

- **Users** — Base account info for all roles
- **PatientProfiles / TherapistProfiles / AdminProfiles** — Role-specific profile data
- **EmailVerifications** — Temporary email verification records (code + expiry + verified flag)
- **AvailabilitySlots** — Therapist time slots; linked to `Appointments` via `SlotId`
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
- [x] Phase 3 — Therapist Verification (Admin) + Re-submission Flow
- [x] Phase 4 — Availability & Appointment Booking
- [x] Phase 5 — Payment Integration (Paymob) ✅
- [x] Phase 5.5 — User Directory Endpoints ✅
- [ ] Phase 6 — Video Session Management
- [ ] Phase 7 — AI Module Integration
- [x] Phase 8 — Ratings & Reviews ✅
- [x] Phase 8.5 — Email Verification (MailKit + Gmail SMTP) ✅
- [ ] Phase 9 — Advanced Admin Dashboard
- [ ] Phase 10 — Security & Performance Optimization
- [ ] Phase 11 — Deployment & Finalization

---

## Team

> This project was built as a graduation project for **Fayoum University — Faculty of Computers and Artificial Intelligence**.

| Role              | Name           |
| ----------------- | -------------- |
| Backend Developer | Ahmed Amin     |
| AI / ML           | Ahmed Magdy    |
| AI / ML           | Mohamed Younes |
| Frontend          | Duaa Magdy     |
| Frontend          | Menna Mohamed  |

---

<p align="center">Made with ❤️ for mental health awareness</p>
