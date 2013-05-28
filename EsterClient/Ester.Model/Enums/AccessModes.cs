using System.ComponentModel;

namespace Ester.Model.Enums
{
    [Description("������ ������� ������")]
    public enum AccessModes
    {
        [Description("�� ���������")]
        Unset = 0,
        [Description("������ �� ���������")]
        CardOnly = 1,
        [Description("������ �������")]
        AlwaysOpen = 2,
        [Description("����� ����")]
        OfficeMode = 4
    }
}
