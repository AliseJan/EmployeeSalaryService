using Employee_Salary_Service.DTOs;
using Employee_Salary_Service.Interfaces;

namespace Employee_Salary_Service.Departments
{
    public class BookkeepingDepartment : IBookkeepingDepartment
    {
        private readonly IEmployeeHistoryService _employeeHistoryService;
        private readonly IEmployeeDetailsWrapper _employeeDetails;

        public BookkeepingDepartment(IEmployeeHistoryService employeeHistoryService, IEmployeeDetailsWrapper employeeDetails)
        {
            _employeeHistoryService =
                employeeHistoryService ?? throw new ArgumentNullException(
                    nameof(employeeHistoryService), "Employee history service is required");
            _employeeDetails = employeeDetails ?? throw new ArgumentNullException(
                nameof(employeeDetails), "Employee details service is required");
        }

        public EmployeeMonthlyReport[] GetMonthlyReport(DateTime periodStartDate, DateTime periodEndDate)
        {
            if (periodEndDate <= periodStartDate)
            {
                throw new InvalidOperationException("Period start date must be before end date");
            }

            var reports = new List<EmployeeMonthlyReport>();
            var employeeIds = _employeeHistoryService.GetAllEmployeeIds();
            var totalSalary = 0m;

            foreach (var employeeId in employeeIds)
            {
                for (var date = periodStartDate; date <= periodEndDate; date = date.AddMonths(1))
                {
                    var contract = _employeeDetails.GetContract(employeeId, _employeeHistoryService, date);
                    if (contract != null)
                    {
                        var workedHours = _employeeDetails.GetWorkedHoursForMonth(employeeId, _employeeHistoryService, date);
                        var salaryRate = _employeeDetails.GetSalaryRate(employeeId, _employeeHistoryService, date);
                        totalSalary = workedHours * salaryRate;
                    }
                    reports.Add(new EmployeeMonthlyReport
                    {
                        EmployeeId = employeeId,
                        Year = date.Year,
                        Month = date.Month,
                        Salary = totalSalary
                    });
                }
            }
            return reports.ToArray();
        }
    }
}