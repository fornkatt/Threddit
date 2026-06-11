using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Threddit.Application.Interfaces.Driven;
using Threddit.Contracts.Common;
using Threddit.Contracts.Responses.Images;

namespace Threddit.API.Controllers;

[ApiController]
[Route("api/images")]
public class ImageController : ControllerBase
{
    private readonly IImageStorageService _imageStorageService;

    private static readonly HashSet<string> AllowedMimeTypes =
    [
        "image/jpeg",
        "image/jpg",
        "image/png",
        "image/gif",
        "image/webp"
    ];

    private const long MaxFileSizeBytes = 10 * 1024 * 1024;

    public ImageController(
        IImageStorageService imageStorageService
    )
    {
        _imageStorageService = imageStorageService;
    }

    [HttpPost("upload")]
    [Authorize(Policy = "NotBanned")]
    public async Task<ActionResult<UploadImageApiResponse>> UploadImage(
        IFormFile file,
        [FromQuery] string category = "post",
        CancellationToken ct = default)
    {
        if (file.Length == 0)
            return BadRequest(new ErrorResponse("No file provided."));

        if (file.Length > MaxFileSizeBytes)
            return BadRequest(new ErrorResponse("Image must be under 10MB."));

        if (!AllowedMimeTypes.Contains(file.ContentType.ToLower()))
            return BadRequest(new ErrorResponse("Unsupported image format. Use JPEG, PNG, GIF, or WEBP."));

        var imageCategory = category.ToLower() switch
        {
            "profile" => ImageCategory.ProfilePicture,
            "comment" => ImageCategory.CommentImage,
            _ => ImageCategory.PostImage
        };

        await using var stream = file.OpenReadStream();
        var url = await _imageStorageService.StoreImageAsync(stream, imageCategory, ct);
        return Ok(new UploadImageApiResponse(url));
    }
}