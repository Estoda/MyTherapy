using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.EntityFrameworkCore;
using MyTherapy.Domain.Enums;
using MyTherapy.Infrastructure.Persistence;
using System.Security.Claims;

namespace MyTherapy.API.Filters;

public class VerifiedTherapistFilter : IAsyncActionFilter
{
    private readonly AppDbContext _context;

    public VerifiedTherapistFilter(AppDbContext context) => _context = context;

    public async Task OnActionExecutionAsync(ActionExecutingContext context, ActionExecutionDelegate next)
    {
        var user = context.HttpContext.User;

        var role = user.FindFirstValue(ClaimTypes.Role);
        if (role != nameof(Role.Therapist))
        {
            await next();
            return;
        }

        var allowedPaths = new[]
        {
            "/api/profile/verification-status",
            "/api/profile/upload-license"
        };

        var path = context.HttpContext.Request.Path.Value;
        if (path != null && allowedPaths.Any(p => path.Equals(p, StringComparison.OrdinalIgnoreCase)))
        {
            await next();
            return;
        }

        var userIdStr = user.FindFirstValue(ClaimTypes.NameIdentifier);
        if (!Guid.TryParse(userIdStr, out var userId))
        {
            context.Result = new UnauthorizedResult();
            return;
        }

        var therapist = await _context.Therapists
            .FirstOrDefaultAsync(t => t.UserId == userId);

        if (therapist == null || therapist.VerificationStatus != VerificationStatus.Approved)
        {
            context.Result = new ObjectResult(new
            {
                statusCode = 403,
                message = "Your account is pending admin verification. You cannot perform this action yet."

            })
            {
                StatusCode = 403
            };
            return;
        }

        await next();
    }
}
