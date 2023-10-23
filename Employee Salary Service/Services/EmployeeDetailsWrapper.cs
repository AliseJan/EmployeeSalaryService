using Employee_Salary_Service.DTOs;
using Employee_Salary_Service.Interfaces;

namespace Employee_Salary_Service.Services
{
    public class EmployeeDetailsWrapper : IEmployeeDetailsWrapper
    {
        public EmployeeContract GetContract(int employeeID, IEmployeeHistoryService history, DateTime date)
        {
            return EmployeeDetails.GetContract(employeeID, history, date);
        }

        public EmployeeContract GetCurrentContract(int employeeID, IEmployeeHistoryService history)
        {
            return EmployeeDetails.GetCurrentContract(employeeID, history);
        }

        public decimal GetSalaryRate(int employeeID, IEmployeeHistoryService history, DateTime date)
        {
            return EmployeeDetails.GetSalaryRate(employeeID, history, date);
        }

        public decimal GetWorkedHoursForMonth(int employeeID, IEmployeeHistoryService history, DateTime date)
        {
            return EmployeeDetails.GetWorkedHoursForMonth(employeeID, history, date);
        }

        public void UpdateSalary(int employeeId, IEmployeeHistoryService history, decimal newSalary, DateTime date)
        {
            EmployeeDetails.UpdateSalary(employeeId, history, newSalary, date);
        }
    }
}