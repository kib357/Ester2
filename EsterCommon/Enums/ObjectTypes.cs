namespace EsterCommon.Enums
{
	public enum ObjectTypes
	{
		FloorPlan = 1,//	Этажный план
		DHSPlan = 2,//	План ИТП
		VentilationPlan = 3, //	Схема ПВУ
		ElectricalGridPlan = 4, //	Схема электроснабжения
		MediaPlan = 5,//	Схема медиа
		Container = 15, //	Группа объектов
		Geometry = 20, //	Геометрия
		Text = 25, //	Надпись
		Textsensor = 30, //	Текстовый датчик
		Room = 50, //	Помещение
		TemperatureSensor = 51,//	Датчик температуры
		LightSensor = 52, //	Датчик освещенности
		WdSensor = 53, //	Датчик протечки
		VolumeSensor = 54, //	Датчик объема
		Motionsensor = 55, //	Датчик движения
		LightGroup = 56, //	Группа ламп
		GroupedLamp = 57, //	Лампа в составе группы
		Lamp = 58, //	Одиночная лампа
		Heater = 59, //	Отопление
		AC = 60, //	Кондиционер
		Cutout = 80, //	Автоматический выключатель электроэнергии
		Camera = 90, //	Видеокамера
		Pump = 100, //	Насос жидкостный
		ThreewayChoke = 101, //	Клапан трехходовой
		TwowayChoke = 102,	//Клапан двухходовой
		WaterFilter = 103, //	Фильтр жидкостный
		Pipe = 104, //	Труба
		VentilationChannell = 120, //	Канал ПВУ
		VentilataionShutter = 121, //	Жалюзи ПВУ
		AirFilter = 122, //	Фильтр ПВУ
		VentilationHeater = 123, //	Электрообогрев ПВУ
		Ventilationradiator = 124, //	Радиатор ПВУ
		VentilationEngine = 125, //	Мотор ПВУ
		FreezeSensor = 126, //	Датчик обмерзания
	}
}
