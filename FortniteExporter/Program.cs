using System;
using System.IO;
using CUE4Parse.FileProvider;
using CUE4Parse.UE4.Assets;
using CUE4Parse.UE4.Assets.Exports.Texture;
using SkiaSharp;

class Program
{
    static void Main(string[] args)
    {
        // Path to your PAKs (adjust if needed)
        var provider = new DefaultFileProvider("./paks", SearchOption.AllDirectories, isCaseInsensitive: true);
        provider.Initialize();

        // Create docs folder for GitHub Pages
        Directory.CreateDirectory("./docs");

        foreach (var file in provider.Files)
        {
            if (file.Key.Contains("/OfferCatalog/Textures/")) // shop images only
            {
                try
                {
                    var pkg = provider.LoadPackage(file.Key);
                    var export = pkg.GetExport<UTexture2D>();
                    if (export != null)
                    {
                        var tex = export.Decode();
                        if (tex != null)
                        {
                            using var image = SKImage.FromBitmap(tex);
                            using var data = image.Encode(SKEncodedImageFormat.Png, 100);

                            var safeName = Path.GetFileNameWithoutExtension(file.Key)
                                .Replace("/", "_")
                                .Replace("\\", "_");

                            var outPath = Path.Combine("./docs", safeName + ".png");

                            using var fs = File.Open(outPath, FileMode.Create, FileAccess.Write);
                            data.SaveTo(fs);

                            Console.WriteLine($"✅ Exported: {outPath}");
                        }
                    }
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"❌ Failed on {file.Key}: {ex.Message}");
                }
            }
        }

        Console.WriteLine("🎉 Export finished.");
    }
}
