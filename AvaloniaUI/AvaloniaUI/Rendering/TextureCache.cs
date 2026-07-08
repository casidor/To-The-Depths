using Avalonia.Media.Imaging;
using Avalonia.Platform;
using System;
using System.Collections.Generic;

namespace AvaloniaUI.Rendering
{
    internal static class TextureCache
    {
        private static readonly Dictionary<string, Bitmap> _cache = new();

        public static Bitmap? Get(string spriteName)
        {
            if (_cache.TryGetValue(spriteName, out var bitmap))
                return bitmap;

            try
            {
                bitmap = new Bitmap(AssetLoader.Open(new Uri($"avares://AvaloniaUI/Assets/{spriteName}.png")));
                _cache[spriteName] = bitmap;
                return bitmap;
            }
            catch
            {
                return null;
            }
        }
    }
}