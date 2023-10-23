namespace Employee_Salary_Service.DTOs
{
    public class EmployeeContract
    {
        public DateTime StartDate { get; }
        public DateTime? EndDate { get; set; }
        public List<SalaryRate> SalaryRates { get; }

        public EmployeeContract(DateTime startDate, decimal hourlySalary)
        {
            StartDate = startDate;
            SalaryRates = new List<SalaryRate>
            {
                new SalaryRate(startDate, hourlySalary)
            };
        }

        public void ChangeSalary(DateTime date, decimal newHourlySalary)
        {
            SalaryRates[SalaryRates.Count - 1].EndDate = date.AddDays(-1);
            SalaryRates.Add(new SalaryRate(date, newHourlySalary));
        }
    }
}