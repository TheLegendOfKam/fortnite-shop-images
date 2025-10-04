using System;
using System.IO;
using CUE4Parse.FileProvider;
using CUE4Parse.UE4.Assets.Exports.Texture;
using CUE4Parse.UE4.Versions;
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

        // ✅ Use the new provider constructor
        var provider = new DefaultFileProvider(pakDir, SearchOption.AllDirectories, true, new VersionContainer(EGame.GAME_UE5_1));
        provider.Initialize();

        Console.WriteLine("Loaded Paks: " + provider.MountedVfs.Count);

        // Only look in OfferCatalog textures (shop images)
        foreach (var file in provider.Files)
        {
            if (file.Key.Contains("/OfferCatalog/Textures/") && file.Key.EndsWith(".uasset", StringComparison.OrdinalIgnoreCase))
            {
                try
                {
                    var exports = provider.LoadAllObjects(file.Key);

                    foreach (var export in exports)
                    {
                        if (export is UTexture2D texture)
                        {
                            var image = texture.DecodeTexture(); // ✅ correct extension
                            if (image != null)
                            {
                                string fileName = Path.GetFileNameWithoutExtension(file.Key) + ".png";
                                string savePath = Path.Combine(outputDir, fileName);

                                using var fs = File.OpenWrite(savePath);
                                image.Encode(SKEncodedImageFormat.Png, 100).SaveTo(fs);

                                Console.WriteLine($"Exported {fileName}");
                            }
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
