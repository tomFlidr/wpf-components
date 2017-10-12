using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

// potřeba pro IValueConverter:
using System.Windows.Data;

namespace Models.Persons {
	public class SexConverter:IValueConverter {
		public object Convert (object value, Type targetType, object parameter, CultureInfo culture) {
			return value.ToString().ToLower() == "m" ? "Muž" : "Žena";
		}

		public object ConvertBack (object value, Type targetType, object parameter, CultureInfo culture) {
			return value.ToString().Substring(0, 1).ToLower() == "m" ? "M" : "F";
		}
	}
}
