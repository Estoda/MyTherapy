using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using MyTherapy.Application.Interfaces;
using MyTherapy.Infrastructure.Persistence;
using MyTherapy.Domain.Enums;

namespace MyTherapy.Infrastructure.Services;

public class ProfileService : IProfileService
{
    private readonly AppDbContext _context;
    private readonly IWebHostEnvironment _env;

    public ProfileService(AppDbContext context, IWebHostEnvironment env)
    {
        _context = context;
        _env = env;
    }

    public async Task<string> UploadProfilePictureAsync(Guid userId, Stream fileStream, string fileName, long fileSize)
    {
        var allowedExtensions = new[] { ".jpg", ".jpeg", ".png" };
        var extension = Path.GetExtension(fileName).ToLower();

        if (!allowedExtensions.Contains(extension))
            throw new ArgumentException("Only .jpg, .jpeg, and .png files are allowed.");

        if (fileSize > 5 * 1024 * 1024)
            throw new ArgumentException("File size must not exceed 5MB.");

        var uploadsFolder = Path.Combine(_env.WebRootPath, "uploads", "profiles");
        Directory.CreateDirectory(uploadsFolder);

        var savedFileName = $"{userId}{extension}";
        var fullPath = Path.Combine(uploadsFolder, savedFileName);

        using (var stream = new FileStream(fullPath, FileMode.Create))
        {
            await fileStream.CopyToAsync(stream);
        }

        var user = await _context.Users.FindAsync(userId);
        if (user == null)
            throw new KeyNotFoundException("User not found.");

        var relativePath = $"uploads/profiles/{savedFileName}";
        user.ProfilePicture = relativePath;
        await _context.SaveChangesAsync();

        return relativePath;
    }

    public async Task<string> UploadLicenseDocumentAsync(Guid userId, Stream fileStream, string fileName, long fileSize)
    {
        // 1. Validate file — allow images + PDF
        var allowedExtensions = new[] { ".jpg", ".jpeg", ".png", ".pdf" };
        var extension = Path.GetExtension(fileName).ToLower();

        if (!allowedExtensions.Contains(extension))
            throw new ArgumentException("Only .jpg, .jpeg, .png, and .pdf files are allowed.");

        if (fileSize > 10 * 1024 * 1024) // 10MB limit for documents
            throw new ArgumentException("File size must not exceed 10MB.");

        // 2. Build path
        var uploadsFolder = Path.Combine(_env.WebRootPath, "uploads", "licenses");
        Directory.CreateDirectory(uploadsFolder);

        var savedFileName = $"{userId}{extension}";
        var fullPath = Path.Combine(uploadsFolder, savedFileName);

        // 3. Save file
        using (var stream = new FileStream(fullPath, FileMode.Create))
        {
            await fileStream.CopyToAsync(stream);
        }

        // 4. Update TherapistProfile in DB
        var therapist = await _context.Therapists
            .FirstOrDefaultAsync(t => t.UserId == userId);

        if (therapist == null)
            throw new KeyNotFoundException("Therapist profile not found.");

        if (therapist.LicenseDocumentPath != null && therapist.VerificationStatus != VerificationStatus.Rejected)
            throw new InvalidOperationException("You can only re-upload your license if your previous submission was rejected.");

        var relativePath = $"uploads/licenses/{savedFileName}";
        therapist.LicenseDocumentPath = relativePath;
        therapist.VerificationStatus = VerificationStatus.Pending;
        await _context.SaveChangesAsync();

        return relativePath;
    }
}