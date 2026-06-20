# 🧠 MyTherapy — Online Mental Health Platform

> A graduation project backend API built with **ASP.NET Core** following **Clean Architecture** principles. MyTherapy connects patients with licensed therapists, enabling appointment booking, secure payments, AI-powered post-session emotion analysis, and a transparent earnings dashboard for therapists.

---

## 📋 Table of Contents

- [Overview](#overview)
- [Architecture](#architecture)
- [Tech Stack](#tech-stack)
- [Features](#features)
- [Getting Started](#getting-started)
- [Project Structure](#project-structure)
- [API Endpoints](#api-endpoints)
- [Booking & Payment Flow](#booking--payment-flow)
- [AI-Powered Session Analysis Flow](#ai-powered-session-analysis-flow)
- [Therapist Earnings](#therapist-earnings)
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
- 🎙️ Post-session audio recording upload & AI-powered emotion/mood analysis
- 💰 Therapist earnings dashboard (today / this month / total, with platform commission applied)
- 🎥 Video call session management _(planned)_
- 💬 Real-time messaging between patients and therapists _(planned)_
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
- **Scalability** — Easy to swap implementations (e.g., change payment gateway, swap AI provider)

---

## Tech Stack

| Layer            | Technology                       |
| ----------------- | --------------------------------- |
| Framework         | ASP.NET Core 9                     |
| Language          | C# 13                                |
| ORM               | Entity Framework Core                |
| Database          | SQL Server                            |
| Authentication    | JWT Bearer Tokens                      |
| Password Hashing  | BCrypt.Net                              |
| Email             | MailKit (Gmail SMTP)                     |
| API Docs          | Swagger / OpenAPI                         |
| Payment Gateway   | Paymob ✅                                  |
| AI Analysis       | FastAPI model on Hugging Face Spaces ✅     |
| Hosting           | Monster ASP.NET ✅                           |
| Validation        | FluentValidation _(planned)_                 |
| Logging           | Serilog _(planned)_                           |
| Video Calls       | Agora / WebRTC _(planned)_                     |

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
- **Webhook Handling** — Paymob webhook updates payment & appointment status automatically; on success, a `Session` record is auto-created for that appointment; failed payments free the slot back up
- **Post-Session Recording Upload** — Therapists upload a `.wav` recording for a completed session
- **AI Emotion Analysis Integration** — Recording is forwarded to a teammate's FastAPI model hosted on Hugging Face Spaces; analysis runs asynchronously (fire-and-forget with polling), not in real time
- **Analysis Status Polling** — A dedicated endpoint checks the AI task's progress and persists the result once available
- **Therapist Earnings Dashboard** — Today / this month / total earnings, plus a recent payments list, all with a 10% platform commission deducted (simulated, no real payout/disbursement)
- **User Directory** — Authenticated users can browse summary lists of all patients/approved therapists, and view full details by ID
- **Ratings & Reviews** — Patients rate therapists after completed sessions (1–5 stars)
- **Running Rating Average** — TherapistProfile rating auto-updated on every new review
- **Duplicate Review Prevention** — One review per appointment enforced at DB level
- **Global Exception Handling** — Consistent JSON error responses across all endpoints
- **Circular Reference Protection** — Safe JSON serialization with IgnoreCycles
- **Database Seeding** — Auto-creates admin account on first run
- **Response DTOs** — No sensitive data (e.g. PasswordHash) ever exposed in API responses
- **Live Deployment** — Backend deployed to Monster ASP.NET, with Paymob webhooks pointed at the live domain

### 🔄 In Progress

- Video call session management (Phase 6)
- Real-time messaging between patients and therapists
- Advanced admin dashboard (Phase 9)
- Security & performance optimization (Phase 10) — FluentValidation, Serilog
- Real payout/disbursement integration (currently earnings are calculated/displayed only, no real money transfer)

---

## Getting Started

### Prerequisites

- [.NET 9 SDK](https://dotnet.microsoft.com/download)
- [SQL Server](https://www.microsoft.com/en-us/sql-server) (or SQL Server Express)
- [Visual Studio 2022](https://visualstudio.microsoft.com/) or [VS Code](https://code.visualstudio.com/)
- [Paymob account](https://paymob.com) for payment integration
- A Gmail account with [App Password](https://myaccount.google.com/apppasswords) enabled for email verification
- Access to the AI teammate's Hugging Face Space endpoint for emotion analysis

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
> ⚠️ Paymob amounts are sent in **cents**, not whole currency units — `PricePerSession` is multiplied by 100 before being sent to Paymob. Paymob also requires a minimum of `amount_cents` ≥ 10.

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

### Setting up Paymob Webhooks

**Local development** — use [ngrok](https://ngrok.com) to expose your local API to Paymob:

```bash
ngrok http https://localhost:7114
```

**Production (current setup)** — the project is deployed on **Monster ASP.NET**, so webhooks point directly at the live domain instead of ngrok.

In either case, set both callback URLs in Paymob Dashboard → Developers → Payment Integrations:

```
Transaction processed callback: https://YOUR-DOMAIN/api/payment/webhook
Transaction response callback:  https://YOUR-DOMAIN/api/payment/webhook
```

> ⚠️ The ngrok URL changes on every restart — remember to update it in the Paymob dashboard each time when testing locally. This is no longer needed once running against the live Monster ASP.NET deployment.

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
│   ├── Session.cs                  # Includes AiTaskId, AiEmotionSummary, AnalysisStatus
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
│   │   ├── VerificationStatusResponse.cs
│   │   └── EarningsResponse.cs
│   ├── AiAnalysis/
│   │   └── AnalysisStatusResponse.cs
│   └── Users/
│       ├── UserSummaryResponse.cs
│       ├── PatientDetailsResponse.cs
│       └── TherapistDetailsResponse.cs
└── Interfaces/
    ├── IAuthService.cs
    ├── IEmailService.cs
    ├── IProfileService.cs
    ├── IPaymobService.cs
    └── IAiAnalysisService.cs

MyTherapy.Infrastructure/
├── Persistence/
│   ├── AppDbContext.cs
│   ├── DbInitializer.cs
│   └── Migrations/
└── Services/
    ├── AuthService.cs
    ├── EmailService.cs
    ├── ProfileService.cs
    ├── PaymobService.cs
    └── AiAnalysisService.cs

MyTherapy.API/
├── Controllers/
│   ├── AuthController.cs
│   ├── AdminTherapistsController.cs
│   ├── ProfileController.cs
│   ├── PatientAvailabilityController.cs
│   ├── PatientBookingController.cs
│   ├── TherapistAvailabilityController.cs   # Includes /earnings endpoint
│   ├── PaymentController.cs
│   ├── ReviewController.cs
│   ├── SessionController.cs                 # Recording upload + AI analysis status
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
| ------ | ----------------------------------- | -------------------------------------------------- | ---- |
| POST   | `/api/auth/send-verification-code`  | Send 6-digit code to email (expires in 10 min)      | ❌   |
| POST   | `/api/auth/verify-email`            | Verify the emailed code before registration         | ❌   |
| POST   | `/api/auth/register/patient`        | Register a new patient (email must be verified)     | ❌   |
| POST   | `/api/auth/register/therapist`      | Register a new therapist (email must be verified)   | ❌   |
| POST   | `/api/auth/login`                   | Login and get JWT token                              | ❌   |

### Admin

| Method | Endpoint                             | Description                           | Auth  |
| ------ | -------------------------------------- | --------------------------------------- | ----- |
| GET    | `/api/admin/therapists/pending`        | List pending therapist verifications     | Admin |
| POST   | `/api/admin/therapists/{id}/approve`   | Approve a therapist                       | Admin |
| POST   | `/api/admin/therapists/{id}/reject`    | Reject a therapist                        | Admin |

### Profile

| Method | Endpoint                           | Description                                                                                       | Auth      |
| ------ | ------------------------------------ | --------------------------------------------------------------------------------------------------- | --------- |
| POST   | `/api/profile/upload-picture`        | Upload/update profile picture                                                                       | Any role  |
| POST   | `/api/profile/upload-license`        | Upload license document; resets status to `Pending` (also used for re-submission after rejection)   | Therapist |
| GET    | `/api/profile/verification-status`   | Check own verification status — accessible even while `Pending`/`Rejected`                          | Therapist |

> ⚠️ `VerifiedTherapistFilter` blocks unverified therapists from most therapist-only actions, but explicitly allows `verification-status` and `upload-license` so a rejected therapist can check their status and retry.

### Patient

| Method | Endpoint                     | Description                                                         | Auth    |
| ------ | ----------------------------- | --------------------------------------------------------------------- | ------- |
| GET    | `/api/patient/availability`   | Browse available (unbooked, future) slots with therapist info          | Patient |
| POST   | `/api/patient/bookings`       | Book a slot → creates `Appointment` (Scheduled), marks slot booked      | Patient |
| GET    | `/api/patient/bookings/my`    | View my appointments                                                     | Patient |

### Therapist

| Method | Endpoint                            | Description                                                        | Auth      |
| ------ | ------------------------------------- | ---------------------------------------------------------------------- | --------- |
| POST   | `/api/therapist/availability`         | Create availability slot (must be in the future, end after start)        | Therapist |
| GET    | `/api/therapist/availability/my`      | View my future slots, including booked ones with patient info            | Therapist |
| DELETE | `/api/therapist/availability/{id}`    | Delete a slot                                                              | Therapist |
| GET    | `/api/therapist/earnings`             | Today / this month / total earnings (90% share) + recent payments list      | Therapist |

### Payment

| Method | Endpoint                  | Description                                                                                          | Auth             |
| ------ | --------------------------- | --------------------------------------------------------------------------------------------------------- | ---------------- |
| POST   | `/api/payment/initiate`     | Initiate Paymob payment for an **existing appointment** (`appointmentId`)                                   | Patient          |
| POST   | `/api/payment/webhook`      | Paymob webhook — updates payment & appointment status; on success, auto-creates a `Session`; frees slot on failure | ❌ (Paymob only) |

### Sessions (AI Analysis)

| Method | Endpoint                                   | Description                                                                                  | Auth      |
| ------ | --------------------------------------------- | -------------------------------------------------------------------------------------------------- | --------- |
| POST   | `/api/sessions/{sessionId}/upload-recording`  | Upload a `.wav` recording for a completed session; submits it to the AI model and starts analysis     | Therapist |
| GET    | `/api/sessions/{sessionId}/analysis-status`   | Poll the analysis status; checks the AI task and persists the result once `Done`                       | Any authenticated user |

### Reviews

| Method | Endpoint                               | Description                                                     | Auth    |
| ------ | --------------------------------------- | ----------------------------------------------------------------- | ------- |
| POST   | `/api/reviews`                          | Submit a review for a completed appointment (1 per appointment)     | Patient |
| GET    | `/api/reviews/therapist/{therapistId}`  | Get all reviews + rating average for a therapist                    | ❌      |

### Users (Directory)

| Method | Endpoint                       | Description                                                            | Auth                    |
| ------ | --------------------------------- | ---------------------------------------------------------------------- | ------------------------ |
| GET    | `/api/users/patients`              | List all patients (name + profile picture only)                          | Any authenticated user |
| GET    | `/api/users/therapists`            | List all **approved** therapists (name + profile picture only)            | Any authenticated user |
| GET    | `/api/users/patients/{id}`         | Get full patient details by ID                                            | Any authenticated user |
| GET    | `/api/users/therapists/{id}`       | Get full therapist details by ID (specialization, price, rating, etc.)    | Any authenticated user |

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
   ✅ Success → Payment = Successful, Appointment stays Scheduled,
                a Session record is auto-created (AnalysisStatus = Pending)
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

## AI-Powered Session Analysis Flow

After a session takes place, the therapist uploads the recording, which is forwarded to a teammate's FastAPI model (hosted on Hugging Face Spaces) for emotion/mood analysis. Since the model processes audio asynchronously, the backend uses a fire-and-forget submission combined with client-side polling:

```
1. Therapist uploads the session recording
   POST /api/sessions/{sessionId}/upload-recording  (.wav file)
        ↓
   Backend saves the file to wwwroot/uploads/recordings/
   Backend submits the file to the AI model's /analyze_session_batch/ endpoint
   AI returns a task_id
        ↓
   Session.AiTaskId = task_id
   Session.AnalysisStatus = Processing
   (Endpoint returns immediately — does not wait for the AI to finish)
        ↓
2. Frontend polls for status
   GET /api/sessions/{sessionId}/analysis-status
        ↓
   Backend checks the AI's /check_task/{task_id} endpoint
        ↓
   ✅ "completed" → result saved to Session.AiEmotionSummary (JSON),
                    Session.AnalysisStatus = Done
   ❌ "failed"    → Session.AnalysisStatus = Failed
   ⏳ otherwise    → Session.AnalysisStatus stays Processing
```

Notes:

- Only the assigned **therapist** for that session may upload the recording.
- If the AI service is unreachable, the relevant endpoint returns a clear `503 Service Unavailable` response rather than a generic server error.
- The AI's full response payload (timeline, clinical insight report, session statistics) is stored as-is in `Session.AiEmotionSummary` once analysis completes.

---

## Therapist Earnings

Therapists can view a simulated earnings summary via:

```
GET /api/therapist/earnings
```

This endpoint calculates the therapist's share from all **Successful** payments tied to their appointments, after deducting a **10% platform commission** (therapist receives 90% of the session price). It returns:

- **Today's earnings**
- **This month's earnings**
- **Total earnings**
- **Recent payments** — a list of the most recent successful payments, each showing the patient's name, profile picture, and the therapist's share of that payment

> ⚠️ This is a **simulated, display-only** earnings dashboard. No real money transfer/disbursement to therapists is implemented — real payouts would require Paymob's separate Payout/Disbursement API along with per-therapist KYC and bank/wallet onboarding, which is out of scope for this graduation project.

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
- **Sessions** — Auto-created on successful payment; stores recording link, AI task ID, AI analysis status, and the AI's JSON result
- **Conversations & Messages** — In-app messaging system _(planned)_
- **Notifications** — System, payment, and reminder notifications _(planned)_

---

## Roadmap

- [x] Phase 1 — Project Setup & Clean Architecture
- [x] Phase 2 — Authentication & JWT Authorization
- [x] Phase 3 — Therapist Verification (Admin) + Re-submission Flow
- [x] Phase 4 — Availability & Appointment Booking
- [x] Phase 5 — Payment Integration (Paymob) ✅
- [x] Phase 5.5 — User Directory Endpoints ✅
- [x] Phase 6 — Session Creation on Payment Success ✅
- [x] Phase 7 — AI Module Integration (recording upload + async analysis polling) ✅
- [x] Phase 7.5 — Therapist Earnings Dashboard (simulated, 10% commission) ✅
- [x] Phase 8 — Ratings & Reviews ✅
- [x] Phase 8.5 — Email Verification (MailKit + Gmail SMTP) ✅
- [x] Phase 11 (partial) — Deployment to Monster ASP.NET ✅
- [ ] Phase 6.5 — Live Video Call Sessions
- [ ] Phase 9 — Advanced Admin Dashboard
- [ ] Phase 10 — Security & Performance Optimization (FluentValidation, Serilog)
- [ ] Phase 12 — Real Payout/Disbursement Integration

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