using System;
using System.IO;
using CUE4Parse.FileProvider;
using CUE4Parse.UE4.Assets.Exports.Texture;

class Program {
    static void Main() {
        string pakDir = "./paks";
        string outDir = "./docs";

        var provider = new DefaultFileProvider(pakDir, SearchOption.AllDirectories);
        provider.Initialize();

        foreach (var file in provider.Files) {
            if (file.Value.Path.Contains("/OfferCatalog/Textures/")) {
                try {
                    var obj = provider.LoadObject(file.Value.Path);
                    if (obj is UTexture2D tex) {
                        var data = tex.Decode();
                        var savePath = Path.Combine(outDir, file.Value.Path.TrimStart('/') + ".png");
                        Directory.CreateDirectory(Path.GetDirectoryName(savePath)!);
                        File.WriteAllBytes(savePath, data);
                        Console.WriteLine($"Exported {savePath}");
                    }
                } catch { }
            }
        }
    }
}
