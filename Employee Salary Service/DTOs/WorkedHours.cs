namespace Employee_Salary_Service.DTOs
{
    public class WorkedHours
    {
        public DateTime Date { get; }
        public decimal Hours { get; }

        public WorkedHours(DateTime date, decimal hours)
        {
            Date = date;
            Hours = hours;
        }
    }
}