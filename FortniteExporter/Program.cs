using System;
using System.IO;
using CUE4Parse.FileProvider;
using CUE4Parse.UE4.Assets.Exports.Texture;
using SkiaSharp;

class Program
{
    static void Main(string[] args)
    {
        string pakDir = Path.Combine(Directory.GetCurrentDirectory(), "paks");
        string outputDir = Path.Combine(Directory.GetCurrentDirectory(), "docs");

        if (!Directory.Exists(pakDir))
        {
            Console.WriteLine("No paks folder found. Put Fortnite .pak files in /paks.");
            return;
        }

        Directory.CreateDirectory(outputDir);

        // Load pak files
        var provider = new DefaultFileProvider(pakDir, SearchOption.AllDirectories, true);
        provider.Initialize();

        Console.WriteLine("Loaded Paks: " + provider.MountedVfs.Count);

        // Example: export ALL textures
        foreach (var file in provider.Files)
        {
            if (file.Key.EndsWith(".uasset", StringComparison.OrdinalIgnoreCase))
            {
                try
                {
                    var export = provider.LoadObject(file.Key);
                    if (export is UTexture2D texture)
                    {
                        var bitmap = texture.Decode() as SKBitmap;
                        if (bitmap != null)
                        {
                            string fileName = Path.GetFileNameWithoutExtension(file.Key) + ".png";
                            string savePath = Path.Combine(outputDir, fileName);

                            using var fs = File.OpenWrite(savePath);
                            bitmap.Encode(SKEncodedImageFormat.Png, 100).SaveTo(fs);

                            Console.WriteLine($"Exported {fileName}");
                        }
                    }
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Failed {file.Key}: {ex.Message}");
                }
            }
        }

        Console.WriteLine("✅ Export finished. Images saved to /docs/");
    }
}
