using Avalonia;
using Avalonia.Controls;
using Avalonia.Data;
using Avalonia.Data.Converters;
using System.Globalization;

namespace IslandCaller.Converters;

public class DiuToPixelPointConverter : IMultiValueConverter
{
    public object? Convert(IList<object?> values, Type targetType, object? parameter, CultureInfo culture)
    {
        if (values.Count == 3 &&
            values[0] is double x &&
            values[1] is double y &&
            values[2] is TopLevel topLevel)
        {
            var scaling = topLevel.RenderScaling;
            return new PixelPoint((int)Math.Round(x * scaling), (int)Math.Round(y * scaling));
        }
        return BindingOperations.DoNothing;
    }

    public object[] ConvertBack(object? value, Type[] targetTypes, object? parameter, CultureInfo culture)
    {
        if (value is PixelPoint point && parameter is TopLevel topLevel)
        {
            var scaling = topLevel.RenderScaling;
            return new object[] { point.X / scaling, point.Y / scaling, topLevel };
        }
        return new object[] { BindingOperations.DoNothing, BindingOperations.DoNothing, BindingOperations.DoNothing };
    }
}