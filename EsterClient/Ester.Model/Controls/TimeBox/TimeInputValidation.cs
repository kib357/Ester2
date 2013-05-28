using System;

namespace Ester.Model.Controls.TimeBox
{
    public static class TimeInputValidation
    {
        public static bool IsValidHours(string timeString)
        {
            return IsValidTimeSpan(timeString, 0, 24);
        }

        public static bool IsValidMinutes(string timeString)
        {
            return IsValidTimeSpan(timeString, 0, 60);
        }

        public static bool IsValidSeconds(string timeString)
        {
            return IsValidTimeSpan(timeString, 0, 60);
        }

        private static bool IsValidTimeSpan(string timeString, int minValue, int maxValue)
        {
            int time;

            if (!int.TryParse(timeString, out time))
            {
                throw new ArgumentException("timeString is not a number");
            }

            if (time > maxValue || time < minValue)
                return false;

            return true;
        }

        
    }
}