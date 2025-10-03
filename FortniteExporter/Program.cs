using System;
using System.IO;
using CUE4Parse.FileProvider;
using CUE4Parse.UE4.Assets;
using CUE4Parse.UE4.Assets.Exports.Texture;
using CUE4Parse.UE4.Objects.UObject;
using SkiaSharp;

class Program
{
    static void Main(string[] args)
    {
        var provider = new DefaultFileProvider("./paks", SearchOption.AllDirectories, true);
        provider.Initialize();

        Directory.CreateDirectory("./docs");

        foreach (var file in provider.Files)
        {
            if (file.Key.Contains("/OfferCatalog/Textures/")) // shop only
            {
                try
                {
                    var pkg = provider.LoadPackage(file.Key);
                    if (pkg != null)
                    {
                        // Grab the first export and cast to UTexture2D
                        var export = pkg.GetExport(0) as UTexture2D;
                        if (export != null)
                        {
                            var tex = export.DecodeTexture(); // <-- new API name
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
