using System;
using System.Globalization;
using System.Windows.Data;

namespace UndertaleModTool
{
    public class LayerGameObjectTextDisplay : IMultiValueConverter
    {
        public object Convert(object[] values, Type targetType, object parameter, CultureInfo culture)
        {
            if (values[1] == null)
                return string.Format("Instance {0} of {1}", values[0], values[2]);
            return string.Format("Instance {0} of {2} (originally ID {1})", values);
        }

        public object[] ConvertBack(object value, Type[] targetTypes, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
