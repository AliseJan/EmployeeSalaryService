namespace Employee_Salary_Service.DTOs
{
    public class SalaryRate
    {
        public DateTime StartDate { get; }
        public DateTime? EndDate { get; set; }
        public decimal Amount { get; }

        public SalaryRate(DateTime date, decimal hourlySalary)
        {
            StartDate = date;
            Amount = hourlySalary;
        }
    }
}