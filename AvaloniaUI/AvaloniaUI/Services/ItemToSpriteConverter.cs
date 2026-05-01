using Avalonia.Data.Converters;
using Avalonia.Media.Imaging;
using Avalonia.Platform;
using GameCore.Models.Items;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Text;

namespace AvaloniaUI.Services
{
    public class ItemToSpriteConverter : IValueConverter
    {
        private static readonly Dictionary<string, Bitmap> _cache = new();

        public object? Convert(object? value, Type targetType, object? parameter, CultureInfo culture)
        {
            string name = value is Item item ? item.SpriteName : "slot_empty";
            if (!_cache.TryGetValue(name, out var bitmap))
                try
                {
                    bitmap = new Bitmap(AssetLoader.Open(new Uri($"avares://AvaloniaUI/Assets/{name}.png")));
                    _cache[name] = bitmap;
                }
                catch { return null; }
            return bitmap;
        }
        public object? ConvertBack(object? value, Type targetType, object? parameter, CultureInfo culture)
            => throw new NotImplementedException();
    }
}
