using System;
using System.IO;
using System.Linq;
using CUE4Parse.FileProvider;
using CUE4Parse.UE4.Objects.UObject;
using SkiaSharp;

class Program
{
    static void Main(string[] args)
    {
        string pakPath = "./paks";   // folder where your pak files are
        string outPath = "./docs";   // GitHub Pages folder

        var provider = new DefaultFileProvider(pakPath, SearchOption.AllDirectories, true);
        provider.Initialize();

        var textures = provider.Files
            .Where(f => f.Key.Contains("/OfferCatalog/Textures/") && f.Key.EndsWith(".uasset"))
            .Select(f => f.Value);

        foreach (var file in textures)
        {
            try
            {
                var export = provider.LoadObject(file.Path);
                if (export is CUE4Parse.UE4.Assets.Exports.Texture.UTexture2D tex)
                {
                    using var bitmap = tex.Decode()?.ToSKBitmap();
                    if (bitmap != null)
                    {
                        var outFile = Path.Combine(outPath, file.NameWithoutExtension + ".png");
                        using var fs = File.OpenWrite(outFile);
                        bitmap.Encode(SKEncodedImageFormat.Png, 100).SaveTo(fs);
                        Console.WriteLine($"Exported {outFile}");
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Failed: {file.Path} - {ex.Message}");
            }
        }
    }
}
