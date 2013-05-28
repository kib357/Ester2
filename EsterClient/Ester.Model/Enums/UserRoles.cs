using System.ComponentModel;

namespace Ester.Model.Enums
{
    [Description("Роли пользователей программы")]
    public enum UserRoles
    {
        [Description("Администратор")]
        Admin,
        [Description("Оператор")]
        Operator,
        [Description("Не определен")]
        None
    }
}
