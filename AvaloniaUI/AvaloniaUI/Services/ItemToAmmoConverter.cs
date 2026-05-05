using Avalonia.Data.Converters;
using GameCore.Models.Items;
using System;
using System.Globalization;

namespace AvaloniaUI.Services
{
    public class ItemToAmmoConverter : IValueConverter
    {
        public object? Convert(object? value, Type targetType, object? parameter, CultureInfo culture)
        {
            if (value is RangedWeapon rw) return $"{rw.Ammo}/{rw.MaxAmmo}";
            return "";
        }
        public object? ConvertBack(object? value, Type targetType, object? parameter, CultureInfo culture)
            => throw new NotImplementedException();
    }
}
