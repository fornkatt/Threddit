namespace Threddit.Application.Interfaces.Driven;

public interface IImageStorageService
{
    Task<string> StoreImageAsync(Stream imageStream, ImageCategory imageCategory, CancellationToken ct = default);
}

public enum ImageCategory
{
    ProfilePicture,
    PostImage,
    CommentImage
}