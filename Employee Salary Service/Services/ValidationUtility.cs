namespace Employee_Salary_Service.Services
{
    public static class ValidationUtility
    {
        public static void ValidateNotEmpty(string value, string paramName, List<Exception> exceptions)
        {
            if (string.IsNullOrWhiteSpace(value))
            {
                exceptions.Add(new ArgumentException($"{paramName} cannot be empty or whitespace", paramName));
            }
        }

        public static void ValidatePositive(int value, string paramName, List<Exception> exceptions)
        {
            if (value <= 0)
            {
                exceptions.Add(new ArgumentOutOfRangeException(paramName, $"{paramName} must be a positive integer"));
            }
        }

        public static void ValidatePositive(decimal value, string paramName, List<Exception> exceptions)
        {
            if (value <= 0)
            {
                exceptions.Add(new ArgumentOutOfRangeException(paramName, $"{paramName} must be a positive decimal"));
            }
        }

        public static void ValidateNotInPast(DateTime date, string paramName, List<Exception> exceptions)
        {
            if (date < DateTime.Today)
            {
                exceptions.Add(new ArgumentOutOfRangeException(paramName, $"{paramName} cannot be in the past"));
            }
        }

        public static void ValidateHourRange(int hours, string paramName, List<Exception> exceptions)
        {
            if (hours < 0 || hours >= 24)
            {
                exceptions.Add(new ArgumentOutOfRangeException(paramName, $"Invalid {paramName}"));
            }
        }

        public static void ValidateMinuteRange(int minutes, string paramName, List<Exception> exceptions)
        {
            if (minutes < 0 || minutes >= 60)
            {
                exceptions.Add(new ArgumentOutOfRangeException(paramName, $"Invalid {paramName}"));
            }
        }

        public static void ThrowIfInvalid(List<Exception> exceptions)
        {
            if (exceptions.Any())
            {
                throw new AggregateException("Multiple validation problems occurred", exceptions);
            }
        }
    }
}