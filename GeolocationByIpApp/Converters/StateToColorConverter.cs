using System;
using System.Globalization;
using System.Windows.Data;
using System.Windows.Media;
using static GeolocationApp.Model.Enums;

namespace GeolocationApp
{
    public class StateToColorConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is ResponseState responseState)
                switch (responseState)
                {
                    case ResponseState.Success:
                        return Brushes.Green;
                    case ResponseState.Notification:
                        return Brushes.Orange;
                    case ResponseState.Fail:
                        return Brushes.Red;
                }


            return Brushes.Black; // Default color
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}