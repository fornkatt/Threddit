using SkiaSharp;
using Threddit.Application.Interfaces.Driven;

namespace Threddit.Infrastructure.Services;

public sealed class LocalImageStorageService : IImageStorageService
{
    private readonly string _uploadPath;
    private readonly string _baseUrl;

    public LocalImageStorageService(string uploadPath, string baseUrl)
    {
        _uploadPath = uploadPath;
        _baseUrl = baseUrl.TrimEnd('/');
        Directory.CreateDirectory(_uploadPath);
    }

    public async Task<string> StoreImageAsync(Stream imageStream, ImageCategory imageCategory,
        CancellationToken ct = default)
    {
        using var ms = new MemoryStream();
        await imageStream.CopyToAsync(ms, ct);
        var inputBytes = ms.ToArray();

        using var bitmap = SKBitmap.Decode(inputBytes)
                           ?? throw new InvalidOperationException("Failed to decode image.");

        SKBitmap processed;

        if (imageCategory == ImageCategory.ProfilePicture)
        {
            var size = Math.Min(bitmap.Width, bitmap.Height);
            var cropRect = new SKRectI(
                (bitmap.Width - size) / 2,
                (bitmap.Height - size) / 2,
                (bitmap.Width + size) / 2,
                (bitmap.Height + size) / 2);

            using var cropped = new SKBitmap(size, size);
            bitmap.ExtractSubset(cropped, cropRect);
            processed = cropped.Resize(new SKImageInfo(128, 128), SKFilterQuality.High)
                        ?? throw new InvalidOperationException("Failed to resize image.");
        }
        else
        {
            const int maxW = 1920, maxH = 1080;
            if (bitmap.Width > maxW || bitmap.Height > maxH)
            {
                var scale = Math.Min((float)maxW / bitmap.Width, (float)maxH / bitmap.Height);
                var newW = (int)(bitmap.Width * scale);
                var newH = (int)(bitmap.Height * scale);
                processed = bitmap.Resize(new SKImageInfo(newW, newH), SKFilterQuality.High)
                            ?? throw new InvalidOperationException("Failed to resize image.");
            }
            else
            {
                processed = bitmap;
            }
        }
        
        using var image = SKImage.FromBitmap(processed);
        using var data = image.Encode(SKEncodedImageFormat.Webp, 80);
        
        if (processed != bitmap)
            processed.Dispose();

        var fileName = $"{Guid.NewGuid()}.webp";
        var filePath = Path.Combine(_uploadPath, fileName);
        await using var fileStream = File.Create(filePath);
        data.SaveTo(fileStream);

        return $"{_baseUrl}/uploads/{fileName}";
    }
}