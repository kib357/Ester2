using System.ComponentModel;

namespace Ester.Modules.Building.Model
{
	[Description("Режимы просмотра подсистем")]
	public enum ViewModes
	{
		[Description("Обзор")]
		Overview = 0,
		[Description("Освещение")]
		Ligths = 1,
		[Description("Микроклимат")]
		Microclimat = 2,
		[Description("Вентиляция")]
		Ventilation = 4
	}
}
