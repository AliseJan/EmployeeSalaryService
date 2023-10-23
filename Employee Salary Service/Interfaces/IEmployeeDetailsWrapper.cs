using Employee_Salary_Service.DTOs;

namespace Employee_Salary_Service.Interfaces
{
    public interface IEmployeeDetailsWrapper
    {
        EmployeeContract GetContract(int employeeID, IEmployeeHistoryService history, DateTime date);
        EmployeeContract GetCurrentContract(int employeeID, IEmployeeHistoryService history);
        decimal GetSalaryRate(int employeeID, IEmployeeHistoryService history, DateTime date);
        void UpdateSalary(int employeeId, IEmployeeHistoryService history, decimal newSalary, DateTime date);
        decimal GetWorkedHoursForMonth(int employeeID, IEmployeeHistoryService history, DateTime date);
    }
}