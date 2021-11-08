using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Data;

namespace Husic.Windows.Converters
{
   internal class TimespanSecondsConverter : IValueConverter
   {
      #region Methods
      public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
      {
         if (value is TimeSpan time)
         {
            if (targetType == typeof(double))
               return time.TotalSeconds;
            throw new ArgumentException("Can only convert the timespan to a double value.",nameof(targetType));
         }
         throw new ArgumentException("Can only convert from timespans.", nameof(value));
      }
      public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
      {
         if (value is double seconds)
         {
            if (targetType == typeof(TimeSpan))
               return TimeSpan.FromSeconds(seconds);
            throw new ArgumentException("Can only convert the double to a timespan value.", nameof(targetType));
         }
         throw new ArgumentException("Can only convert from doubles.", nameof(value));
      }
      #endregion
   }
}
