using Avalonia.Controls;
using Avalonia.Platform.Storage;
using Lindronics.OneOf.Result;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TransportHub.Core.Services.Impl;

public class UserProvidedImageService : IUserProvidedImageService
{
    private readonly IStorageProvider _storageProvider;

    public UserProvidedImageService(IStorageProvider storageProvider)
    {
        _storageProvider = storageProvider;
    }
    public async Task<Result<string?, Exception>> GetImage()
    {
        FilePickerFileType makeType(string name, string mimeType, params string[] pattern)
            => new(name)
            {
                Patterns = pattern,
                MimeTypes = new string[] { mimeType },
            };

        FilePickerFileType[] supportedTypes =
        {
            makeType("png", "image/png" , "*.png"),
            makeType("jpg", "image/jpeg", "*.jpg", "*.jpeg"),
            makeType("gif", "image/gif" , "*.gif"),
        };

        var files = await _storageProvider.OpenFilePickerAsync(new FilePickerOpenOptions
        {
            Title = "Open Text File",
            AllowMultiple = false,
            FileTypeFilter = supportedTypes,
        });

        if (files.Count == 0)
            return null as string;


        try
        {
            var file = files[0];
            // Open reading stream from the first file.
            await using var stream = await file.OpenReadAsync();
            using var memStream = new MemoryStream();

            await stream.CopyToAsync(memStream);

            var contents = memStream.ToArray();
            var base64 = Convert.ToBase64String(contents);


            var extension = Path.GetExtension(file.Name);
            var type = supportedTypes.First(x => x.Patterns!.Any(p => p.Contains(extension)));

            var mimeType = type.MimeTypes![0];


            return $"data:{mimeType};base64,{base64}";

        }
        catch (Exception ex)
        {
            return ex;
        }
    }
}
