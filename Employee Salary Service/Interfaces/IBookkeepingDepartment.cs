using Employee_Salary_Service.DTOs;

namespace Employee_Salary_Service.Interfaces
{
    public interface IBookkeepingDepartment
    {
        EmployeeMonthlyReport[] GetMonthlyReport(DateTime periodStartDate, DateTime periodEndDate);
    }
}