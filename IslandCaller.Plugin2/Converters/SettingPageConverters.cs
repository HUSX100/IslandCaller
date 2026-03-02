using Avalonia.Data.Converters;
using System.Globalization;

namespace IslandCaller.Converters
{

    public class IntToBoolConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is int gender)
                return gender == 1; // 1 → true
            return false;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is bool isChecked)
                return isChecked ? 1 : 0; // true → 1, false → 0
            return 0;
        }
    }

    public class GenderConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is int gender)
            {
                return gender == 0 ? " 男 " : " 女 ";
            }
            return "未知";
        }

        // 不需要转换回去，直接返回 Binding.DoNothing
        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return Avalonia.Data.BindingOperations.DoNothing;
        }
    }

}
