using System.Windows.Controls;

namespace Ester.Model.ValidationRules
{
    public class TextBoxMustHaveValueRule : ValidationRule
    {
        public override ValidationResult Validate(object value, System.Globalization.CultureInfo cultureInfo)
        {
            return value != null && string.IsNullOrWhiteSpace(value.ToString()) ? new ValidationResult(false, "Поле должно быть заполнено") : new ValidationResult(true, null);
        }
    }
}
