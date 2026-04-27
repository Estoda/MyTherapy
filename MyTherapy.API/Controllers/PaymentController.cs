using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using MyTherapy.Application.DTOs.Payment;
using MyTherapy.Application.Interfaces;
using MyTherapy.Domain.Entities;
using MyTherapy.Domain.Enums;
using MyTherapy.Infrastructure.Persistence;
using System.Security.Claims;
using System.Text.Json;

namespace MyTherapy.API.Controllers;

[Authorize(Roles = "Patient")]
[ApiController]
[Route("api/payment")]
public class PaymentController : ControllerBase
{
    private readonly AppDbContext _context;
    private readonly IPaymobService _paymob;

    public PaymentController(AppDbContext context, IPaymobService paymob)
    {
        _context = context;
        _paymob = paymob;
    }

    [HttpPost("initiate")]
    public async Task<IActionResult> InitiatePayment(PaymentInitiateRequest request)
    {
        var userId = Guid.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);

        var patient = await _context.Patients
            .Include(p => p.User)
            .FirstOrDefaultAsync(p => p.UserId == userId);

        if (patient == null)
            return NotFound("Patient profile not found.");

        var slot = await _context.AvailabilitySlots
            .Include(s => s.Therapist)
            .FirstOrDefaultAsync(s => s.Id == request.SlotId);

        if (slot == null)
            return NotFound("Slot not found.");

        if (slot.IsBooked)
            return BadRequest("Slot is already booked.");

        var amount = slot.Therapist.PricePerSession;

        // ✅ Step 1: Try Paymob FIRST before touching the DB
        var appointment = new Appointment
        {
            PatientId = patient.Id,
            TherapistId = slot.TherapistId,
            SlotId = slot.Id,
            AppointmentDateTime = slot.StartTime,
            DurationMinutes = (int)(slot.EndTime - slot.StartTime).TotalMinutes,
            Status = AppointmentStatus.Scheduled
        };

        _context.Appointments.Add(appointment);

        var payment = new Payment
        {
            UserId = userId,
            AppointmentId = appointment.Id,
            Amount = amount,
            Currency = "EGP",
            Method = PaymentMethod.CreditCard,
            Status = PaymentStatus.Pending
        };

        _context.Payments.Add(payment);
        appointment.PaymentId = payment.Id;

        // ✅ Step 2: Call Paymob BEFORE saving slot as booked
        string paymentUrl;
        try
        {
            paymentUrl = await _paymob.InitiatePaymentAsync(
                appointment.Id,
                amount,
                patient.User.Email,
                patient.User.FullName);
        }
        catch (Exception ex)
        {
            // ✅ Paymob failed → don't save anything
            return StatusCode(500, new { message = $"Payment gateway error: {ex.Message}" });
        }

        // ✅ Step 3: Only save to DB if Paymob succeeded
        slot.IsBooked = true;
        await _context.SaveChangesAsync();

        return Ok(new PaymentInitiateResponse
        {
            PaymentUrl = paymentUrl,
            Appointment = appointment.Id
        });
    }

    [AllowAnonymous]
    [HttpPost("webhook")]
    public async Task<IActionResult> Webhook()
    {
        using var reader = new StreamReader(Request.Body); // Read the raw request body
        var body = await reader.ReadToEndAsync(); // Get the JSON string from the request body
        var doc = JsonDocument.Parse(body); // Parse the JSON string into a JsonDocument
        var obj = doc.RootElement.GetProperty("obj"); // Access the "obj" property from the JSON

        var success = obj.GetProperty("success").GetBoolean();
        var merchantOrderId = obj.GetProperty("order").GetProperty("merchant_order_id").GetString();

        if (!Guid.TryParse(merchantOrderId, out var appointmentId)) // Extract the appointment ID from the merchant order ID
            return BadRequest("Invalid order reference!");

        var appointment = await _context.Appointments
            .Include(a => a.Payment)
            .FirstOrDefaultAsync(a => a.Id == appointmentId);

        if (appointment == null)
            return NotFound("Appointment not found!");

        if (success)
        {
            appointment.Payment!.Status = PaymentStatus.Successful;
            appointment.Payment.PaymentDate = DateTime.UtcNow;
            appointment.Payment.TransactionId = obj.GetProperty("id").GetInt64().ToString();
            appointment.Status = AppointmentStatus.Scheduled;
        }
        else
        {
            appointment.Payment!.Status = PaymentStatus.Failed;
            appointment.Status = AppointmentStatus.Cancelled;

            var slot = await _context.AvailabilitySlots
                .FirstOrDefaultAsync(s => s.TherapistId == appointment.TherapistId && s.StartTime == appointment.AppointmentDateTime);

            if (slot != null)
                slot.IsBooked = false; // Free up the slot if payment failed

        }
        await _context.SaveChangesAsync();
        return Ok();
    }
}
