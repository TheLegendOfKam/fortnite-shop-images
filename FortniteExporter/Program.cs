using System;
using System.IO;
using CUE4Parse.FileProvider;
using CUE4Parse.UE4.Objects.UObject;
using CUE4Parse.UE4.Objects.Engine;
using SkiaSharp;

class Program
{
    static void Main(string[] args)
    {
        // Point to your downloaded PAKs
        var provider = new DefaultFileProvider("./paks", SearchOption.AllDirectories, isCaseInsensitive: true);
        provider.Initialize();

        // Create docs folder for GitHub Pages
        Directory.CreateDirectory("./docs");

        foreach (var file in provider.Files)
        {
            if (file.Key.Contains("/OfferCatalog/Textures/")) // filter only shop images
            {
                try
                {
                    var export = provider.LoadObject<UTexture2D>(file.Key);
                    if (export != null)
                    {
                        var tex = export.Decode();
                        if (tex != null)
                        {
                            using var image = SKImage.FromBitmap(tex);
                            using var data = image.Encode(SKEncodedImageFormat.Png, 100);

                            var outPath = Path.Combine("./docs", Path.GetFileNameWithoutExtension(file.Key) + ".png");
                            using var fs = File.OpenWrite(outPath);
                            data.SaveTo(fs);

                            Console.WriteLine($"Exported: {outPath}");
                        }
                    }
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Failed on {file.Key}: {ex.Message}");
                }
            }
        }
    }
}
