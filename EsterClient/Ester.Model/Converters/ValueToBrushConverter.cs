using System;
using System.Windows.Data;
using System.Windows.Media;

namespace Ester.Model.Converters
{
	public class ValueToBrushConverter : IValueConverter
	{
		public double MinimumValue { get; set; }
		public double MaximumValue { get; set; }

		private SolidColorBrush[] VentBrushes = new SolidColorBrush[]
		{
			Brushes.RoyalBlue,
			Brushes.DodgerBlue,
			Brushes.DeepSkyBlue,
			Brushes.LightSkyBlue,
			Brushes.LightGray,
			Brushes.LightPink,
			Brushes.HotPink,
			Brushes.DeepPink
				                                
		};

		public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
		{
			double red = 0, green = 0, blue = 0;
			switch ((string)parameter)
			{
				// Конвертер для температуры
				case "Temperature":
					{
						if (value == null)
							return new SolidColorBrush(Colors.Black);

						var valTemp = (double)value;

						// убедиться, что значение входит в диапазон
						valTemp = Math.Max(14, valTemp);
						valTemp = Math.Min(28, valTemp);

						red = (valTemp - 14) / (28 - 14);
						blue = 1 - red;
						green = Math.Abs(1 - Math.Abs(red - blue));

						return new SolidColorBrush(Color.FromScRgb(1,
							(float)(red * 0.60),
							(float)(green * 0.60),
							(float)(blue * 0.60))
						);
					}
				// Конвертер для СКУД
				case "SKUDSwitch":
					{						
						var boolValue = (double?)value == 1;
						return ((bool?)boolValue ?? false) ? Brushes.LimeGreen : Brushes.Tomato;
					}

				// Конвертер для уровня освещенности
				case "HeatLevel":
					if (value == null)
						return new SolidColorBrush(Colors.Black);

					dynamic valLight = value;

					// убедиться, что значение входит в диапазон
					valLight = Math.Max(1, valLight);
					valLight = Math.Min(100, valLight);

					red = (valLight - 1) / (99);
					blue = 0.2 * red;
					green = red * 0.9;

					return new SolidColorBrush(Color.FromScRgb(1,
						(float)(0.5 + red * 0.5),
						(float)(0.5 + green * 0.5),
						(float)(0.2 + blue * 0.5))
					);

				case "Ventilation":
					if (value == null)
						return new SolidColorBrush(Colors.Black);

					dynamic valVent = value;
					return VentBrushes[(int)valVent + 4];

				case "AC":
					if (value == null)
						return new SolidColorBrush(Colors.Black);
					dynamic valAc = value;
					return VentBrushes[4 - (int)valAc];
				default:
					return Brushes.Transparent;
			}

		}

		public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
		{
			throw new NotImplementedException();
		}
	}
}
