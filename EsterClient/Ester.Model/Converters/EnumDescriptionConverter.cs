using System;
using System.ComponentModel;
using System.Globalization;
using System.Reflection;
using System.Windows.Data;
using Ester.Model.Enums;

namespace Ester.Model.Converters
{
	public class EnumDescriptionConverter : IValueConverter
	{
		private string GetEnumDescription(Enum enumObj)
		{
			FieldInfo fieldInfo = enumObj.GetType().GetField(enumObj.ToString());
			object[] attribArray = fieldInfo.GetCustomAttributes(false);
			if (attribArray.Length == 0)
			{
				return enumObj.ToString();
			}
			else
			{
				DescriptionAttribute attrib = attribArray[0] as DescriptionAttribute;
				return attrib.Description;
			}
		}

		object IValueConverter.Convert(object value, Type targetType, object parameter, CultureInfo culture)
		{
			if (value == null || value.ToString() == "") return value;

			//AccessModes myEnum = (AccessModes)value;
			return GetEnumDescription((Enum)value);
		}
		object IValueConverter.ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
		{
			return string.Empty;
		}
	}
}
