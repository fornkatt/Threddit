using Azure.Storage.Blobs;
using Azure.Storage.Blobs.Models;
using SkiaSharp;
using Threddit.Application.Interfaces.Driven;

namespace Threddit.Infrastructure.Services;

public sealed class AzureBlobImageStorageService : IImageStorageService
{
    private readonly BlobContainerClient _container;

    public AzureBlobImageStorageService(string connectionString, string containerName)
    {
        var blobServiceClient = new BlobServiceClient(connectionString);
        _container = blobServiceClient.GetBlobContainerClient(containerName);
    }

    public async Task<string> StoreImageAsync(Stream imageStream, ImageCategory imageCategory,
        CancellationToken ct = default)
    {
        using var ms = new MemoryStream();
        await imageStream.CopyToAsync(ms, ct);
        var inputBytes = ms.ToArray();

        using var bitmap = SKBitmap.Decode(inputBytes) ??
                           throw new InvalidOperationException("Failed to decode image.");

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
                processed = bitmap.Resize(
                                new SKImageInfo((int)(bitmap.Width * scale), (int)(bitmap.Height * scale)),
                                SKFilterQuality.High)
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

        using var output = new MemoryStream();
        data.SaveTo(output);
        output.Position = 0;

        var blobName = $"{Guid.NewGuid()}.webp";
        var blob = _container.GetBlobClient(blobName);
        await blob.UploadAsync(output, new BlobHttpHeaders { ContentType = "image/webp" }, cancellationToken: ct);

        return blob.Uri.ToString();
    }
}