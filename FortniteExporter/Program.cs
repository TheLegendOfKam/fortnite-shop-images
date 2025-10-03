using System;
using CUE4Parse.FileProvider;
using CUE4Parse.UE4.Assets.Exports.Texture;
using System.IO;

class Program
{
    static void Main()
    {
        string pakDir = "./paks";   // Fortnite pak files go here (next step we’ll automate download)
        string outDir = "./docs";

        var provider = new DefaultFileProvider(pakDir, SearchOption.AllDirectories);
        provider.Initialize();

        foreach (var file in provider.Files)
        {
            if (file.Value.Path.StartsWith("/OfferCatalog/Textures"))
            {
                try
                {
                    var obj = provider.LoadObject(file.Value.Path);
                    if (obj is UTexture2D tex)
                    {
                        var png = tex.Decode();
                        var savePath = Path.Combine(outDir, file.Value.Path.TrimStart('/') + ".png");
                        Directory.CreateDirectory(Path.GetDirectoryName(savePath));
                        File.WriteAllBytes(savePath, png);
                        Console.WriteLine("Exported " + savePath);
                    }
                }
                catch { }
            }
        }
    }
}
