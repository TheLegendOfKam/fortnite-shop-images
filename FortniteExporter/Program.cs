using System;
using System.IO;
using System.Linq;
using CUE4Parse.FileProvider;
using CUE4Parse.UE4.Assets.Exports.Texture;
using SkiaSharp;

class Program
{
    static void Main()
    {
        var provider = new DefaultFileProvider("paks", SearchOption.AllDirectories, true);
        provider.Initialize();

        Directory.CreateDirectory("docs");

        foreach (var file in provider.Files.Values.Where(f => f.Path.EndsWith(".uasset")))
        {
            try
            {
                var obj = provider.LoadObject(file);
                if (obj is UTexture2D tex)
                {
                    using var image = tex.Decode();
                    using var data = image.Encode(SKEncodedImageFormat.Png, 100);

                    var name = Path.GetFileNameWithoutExtension(file.Path);
                    var outPath = Path.Combine("docs", name + ".png");

                    File.WriteAllBytes(outPath, data.ToArray());
                    Console.WriteLine($"Exported {outPath}");
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Failed {file.Path}: {ex.Message}");
            }
        }
    }
}
