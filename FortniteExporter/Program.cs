using System;
using System.IO;
using System.Linq;
using CUE4Parse.FileProvider;
using CUE4Parse.UE4.Assets.Exports.Texture;
using SkiaSharp;
using CUE4Parse.UE4.Assets;
using CUE4Parse_Conversion.Textures; 

class Program
{
    static async Task Main(string[] args)
    {
        // point to paks folder
        var provider = new DefaultFileProvider("./paks", SearchOption.AllDirectories, true);
        provider.Initialize();

        // load all textures
        var textures = provider.LoadAllObjects<UTexture2D>();

        foreach (var tex in textures)
        {
            try
            {
                // decode to SKBitmap
                var skBitmap = tex.DecodeTexture();
                if (skBitmap == null) continue;

                // save as PNG
                using var image = SKImage.FromBitmap(skBitmap);
                using var data = image.Encode(SKEncodedImageFormat.Png, 100);

                var path = Path.Combine("docs", tex.Name + ".png");
                Directory.CreateDirectory("docs");
                using var fs = File.OpenWrite(path);
                data.SaveTo(fs);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Failed to export {tex.Name}: {ex.Message}");
            }
        }

        Console.WriteLine("✅ Export complete!");
    }
}
