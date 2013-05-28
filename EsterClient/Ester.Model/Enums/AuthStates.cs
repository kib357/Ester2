using System.ComponentModel;

namespace Ester.Model.Enums
{
    [Description("Режимы ацтентификации")]
    public enum AuthStates
    {
        [Description("Прошел аутентификацию")]
        Authentiticated,
        [Description("Не прошел аутентификацию")]
        NonAuthentiticated
    }
}
