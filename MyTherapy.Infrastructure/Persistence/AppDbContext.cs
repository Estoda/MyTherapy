using Microsoft.EntityFrameworkCore;
using MyTherapy.Domain.Entities;

namespace MyTherapy.Infrastructure.Persistence;

public class AppDbContext : DbContext
{
    public AppDbContext(DbContextOptions<AppDbContext> options) 
        : base(options) { }


    public DbSet<User> Users => Set<User>();
    public DbSet<PatientProfile> Patients => Set<PatientProfile>();
    public DbSet<TherapistProfile> Therapists => Set<TherapistProfile>();
    public DbSet<AdminProfile> Admins => Set<AdminProfile>();
    public DbSet<AvailabilitySlot> AvailabilitySlots => Set<AvailabilitySlot>();
    public DbSet<Appointment> Appointments => Set<Appointment>();
    public DbSet<Payment> Payments => Set<Payment>();
    public DbSet<Session> Sessions => Set<Session>();
    public DbSet<Conversation> Conversations => Set<Conversation>();
    public DbSet<Message> Messages => Set<Message>();
    public DbSet<Notification> Notifications => Set<Notification>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        // User -> Profiles (1:1)
        modelBuilder.Entity<User>()
            .HasOne(u => u.PatientProfile)
            .WithOne(p => p.User)
            .HasForeignKey<PatientProfile>(p => p.UserId)
            .OnDelete(DeleteBehavior.Cascade);

        modelBuilder.Entity<User>()
            .HasOne(u => u.TherapistProfile)
            .WithOne(t => t.User)
            .HasForeignKey<TherapistProfile>(t => t.UserId)
            .OnDelete(DeleteBehavior.Cascade);

        modelBuilder.Entity<User>()
            .HasOne(u => u.AdminProfile)
            .WithOne(a => a.User)
            .HasForeignKey<AdminProfile>(a => a.UserId)
            .OnDelete(DeleteBehavior.Cascade);

        // User -> Email (1:1) - unique index
        modelBuilder.Entity<User>()
            .HasIndex(u => u.Email)
            .IsUnique();

        // PatientProfile -> PreferredTherapist (0:1) - optional relationship
        modelBuilder.Entity<PatientProfile>()
            .HasOne(p => p.PreferredTherapist)
            .WithMany()
            .HasForeignKey(p => p.PreferredTherapistId)
            .OnDelete(DeleteBehavior.NoAction);

        // TherapistProfile -> AvailabilitySlots (1:N)
        modelBuilder.Entity<AvailabilitySlot>()
            .HasOne(s => s.Therapist)
            .WithMany(t => t.AvailabilitySlots)
            .HasForeignKey(s => s.TherapistId)
            .OnDelete(DeleteBehavior.Cascade);

        // Appointment -> AvailabilitySlot (N:1)
        modelBuilder.Entity<Appointment>()
            .HasOne(a => a.Slot)
            .WithMany()
            .HasForeignKey(a => a.SlotId)
            .OnDelete(DeleteBehavior.NoAction);

        // Appointment -> Patient & Therapist (N:1)
        modelBuilder.Entity<Appointment>()
            .HasOne(a => a.Patient)
            .WithMany()
            .HasForeignKey(a => a.PatientId)
            .OnDelete(DeleteBehavior.NoAction);

        modelBuilder.Entity<Appointment>()
            .HasOne(a => a.Therapist)
            .WithMany()
            .HasForeignKey(a => a.TherapistId)
            .OnDelete(DeleteBehavior.NoAction);

        // Appointment → Payment (0:1) - optional relationship
        modelBuilder.Entity<Appointment>()
            .HasOne(a => a.Payment)
            .WithOne(p => p.Appointment)
            .HasForeignKey<Appointment>(a => a.PaymentId)
            .OnDelete(DeleteBehavior.NoAction);

        // Appointment → Session (0:1) - optional relationship
        modelBuilder.Entity<Session>()
            .HasOne(s => s.Appointment)
            .WithOne(a => a.Session)
            .HasForeignKey<Session>(s => s.AppointmentId)
            .OnDelete(DeleteBehavior.Cascade);

        // Payment → User (N:1)
        modelBuilder.Entity<Payment>()
            .HasOne(p => p.User)
            .WithMany()
            .HasForeignKey(p => p.UserId)
            .OnDelete(DeleteBehavior.NoAction);

        modelBuilder.Entity<Payment>()
            .Property(p => p.Amount)
            .HasColumnType("decimal(10,2)");

        // Conversation → Patient & Therapist (N:1)
        modelBuilder.Entity<Conversation>()
            .HasOne(c => c.Patient)
            .WithMany()
            .HasForeignKey(c => c.PatientId)
            .OnDelete(DeleteBehavior.NoAction);

        modelBuilder.Entity<Conversation>()
            .HasOne(c => c.Therapist)
            .WithMany()
            .HasForeignKey(c => c.TherapistId)
            .OnDelete(DeleteBehavior.NoAction);

        // Message → Conversation (N:1)
        modelBuilder.Entity<Message>()
            .HasOne(m => m.Conversation)
            .WithMany(c => c.Messages)
            .HasForeignKey(m => m.ConversationId)
            .OnDelete(DeleteBehavior.Cascade);
        // Message → Sender & Receiver (N:1)
        modelBuilder.Entity<Message>()
            .HasOne(m => m.Sender)
            .WithMany()
            .HasForeignKey(m => m.SenderId)
            .OnDelete(DeleteBehavior.NoAction);
        // Message → Receiver (N:1)
        modelBuilder.Entity<Message>()
            .HasOne(m => m.Receiver)
            .WithMany()
            .HasForeignKey(m => m.ReceiverId)
            .OnDelete(DeleteBehavior.NoAction);

        // Notification → User (N:1)
        modelBuilder.Entity<Notification>()
            .HasOne(n => n.User)
            .WithMany()
            .HasForeignKey(n => n.UserId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}
