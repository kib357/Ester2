using System.ComponentModel;

namespace Ester.Model.Enums
{
    [Description("���� ������������� ���������")]
    public enum UserRoles
    {
        [Description("�������������")]
        Admin,
        [Description("��������")]
        Operator,
        [Description("�� ���������")]
        None
    }
}
