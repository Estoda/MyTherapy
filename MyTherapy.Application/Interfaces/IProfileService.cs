
namespace MyTherapy.Application.Interfaces;

public interface IProfileService
{
    Task<string> UploadProfilePictureAsync(Guid userId, Stream fileStream, string fileName, long fileSize);
}
