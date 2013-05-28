using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Ester.Model.UserControls
{
	[TypeConverter(typeof(DisplayRangeTypeConverter))]
	public class DisplayRange
	{
		public double Start { get; set; }
		public double End { get; set; }

		public DisplayRange(double start, double end)
		{
			Start = start;
			End = end;
		}
	}

	public class DisplayRangeTypeConverter : TypeConverter
	{
		public override bool CanConvertFrom(ITypeDescriptorContext context, Type sourceType)
		{
			return sourceType == typeof(string);
		}

		public override bool CanConvertTo(ITypeDescriptorContext context, Type destinationType)
		{
			return destinationType == typeof(DisplayRange);
		}

		public override object ConvertFrom(ITypeDescriptorContext context, System.Globalization.CultureInfo culture, object value)
		{
			var str = (string)value;
			var i = str.IndexOf(',');
			var start = double.Parse(str.Substring(0, i));
			var end = double.Parse(str.Substring(i + 1, str.Length - i - 1));
			return new DisplayRange(start, end);
		}
	}
}
