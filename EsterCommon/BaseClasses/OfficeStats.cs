using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Practices.Prism.ViewModel;

namespace EsterCommon.BaseClasses
{
    public class OfficeStats : NotificationObject
    {
        private string _name;
        public string Name
        {
            get { return _name; }
            set { _name = value; RaisePropertyChanged("Name"); }
        }

        private AddressValue<string> _temperature;
        public AddressValue<string> Temperature
        {
            get { return _temperature; }
            set { _temperature = value; RaisePropertyChanged("Temperature"); }
        }

        private AddressValue<string> _settedTemperature;
        public AddressValue<string> SettedTemperature
        {
            get { return _settedTemperature; }
            set { _settedTemperature = value; RaisePropertyChanged("SettedTemperature"); }
        }

        private AddressValue<bool?> _settedTemperatureBacstatAllowed;
        public AddressValue<bool?> SettedTemperatureBacstatAllowed
        {
            get { return _settedTemperatureBacstatAllowed; }
            set { _settedTemperatureBacstatAllowed = value; RaisePropertyChanged("SettedTemperatureBacstatAllowed"); }
        }

        private AddressValue<string> _ventilation;
        public AddressValue<string> Ventilation
        {
            get { return _ventilation; }
            set { _ventilation = value; RaisePropertyChanged("Ventilation"); }
        }

        private AddressValue<bool?> _ventilationBacstatAllowed;
        public AddressValue<bool?> VentilationBacstatAllowed
        {
            get { return _ventilationBacstatAllowed; }
            set { _ventilationBacstatAllowed = value; RaisePropertyChanged("VentilationBacstatAllowed"); }
        }

        private AddressValue<string> _aCLevel;
        public AddressValue<string> ACLevel
        {
            get { return _aCLevel; }
            set { _aCLevel = value; RaisePropertyChanged("ACLevel"); }
        }

        private AddressValue<bool?> _aCBacstatAllowed;
        public AddressValue<bool?> ACBacstatAllowed
        {
            get { return _aCBacstatAllowed; }
            set { _aCBacstatAllowed = value; RaisePropertyChanged("ACBacstatAllowed"); }
        }

        private List<string> _cameras;
        public List<string> Cameras
        {
            get { return _cameras; }
            set { _cameras = value; RaisePropertyChanged("Cameras"); }
        }

        private Dictionary<string, bool> _doors;
        public Dictionary<string, bool> Doors
        {
            get { return _doors; }
            set { _doors = value; RaisePropertyChanged("Doors"); }
        }

        private AddressValue<string> _lightLevel;
        public AddressValue<string> LightLevel
        {
            get { return _lightLevel; }
            set { _lightLevel = value; RaisePropertyChanged("LightLevel"); }
        }

        private AddressValue<string> _settedLightLevel;
        public AddressValue<string> SettedLightLevel
        {
            get { return _settedLightLevel; }
            set { _settedLightLevel = value; RaisePropertyChanged("SettedLightLevel"); }
        }

        private AddressValue<bool?> _lightLevelBacstatAllowed;
        public AddressValue<bool?> LightLevelBacstatAlowed
        {
            get { return _lightLevelBacstatAllowed; }
            set { _lightLevelBacstatAllowed = value; RaisePropertyChanged("LightLevelBacstatAlowed"); }
        }

        public OfficeStats(
            string name,
            AddressValue<string> temperature,
            AddressValue<string> settedTemperature,
            AddressValue<bool?> settedTemperatureBacstatAllowedAddress,
            AddressValue<string> ventilation,
            AddressValue<bool?> ventilationBacstatAllowedAddress,
            AddressValue<string> conding,
            AddressValue<bool?> aCBacstatAllowedAddress,
            AddressValue<string> light,
            AddressValue<string> settedLight,
            AddressValue<bool?> lightBacstatAlowed, List<string> cameras, Dictionary<string, bool> doors)
        {
            Name = name;
            Temperature = temperature;
            SettedTemperature = settedTemperature;
            SettedTemperatureBacstatAllowed = settedTemperatureBacstatAllowedAddress;
            Ventilation = ventilation;
            VentilationBacstatAllowed = ventilationBacstatAllowedAddress;
            ACLevel = conding;
            ACBacstatAllowed = aCBacstatAllowedAddress;
            LightLevel = light;
            SettedLightLevel = settedLight;
            LightLevelBacstatAlowed = lightBacstatAlowed;
            Cameras = cameras;
            Doors = doors;
        }
    }
}