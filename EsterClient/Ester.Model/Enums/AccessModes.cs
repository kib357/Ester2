using System.ComponentModel;

namespace Ester.Model.Enums
{
    [Description("Режимы доступа дверей")]
    public enum AccessModes
    {
        [Description("Не определен")]
        Unset = 0,
        [Description("Только по карточкам")]
        CardOnly = 1,
        [Description("Всегда открыто")]
        AlwaysOpen = 2,
        [Description("Режим Офис")]
        OfficeMode = 4
    }
}
