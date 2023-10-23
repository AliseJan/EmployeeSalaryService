using Employee_Salary_Service.DTOs;

namespace Employee_Salary_Service.Interfaces
{
    public interface IEmployeeHistoryService
    {
        void AddEmployeeContract(int employeeId, EmployeeContract contract);
        List<EmployeeContract>? GetEmployeeContractHistory(int employeeID);
        void AddReportedWorkHours(int employeeId, WorkedHours hours);
        List<WorkedHours> GetEmployeeWorkedHoursHistory(int employeeID);
        List<int> GetAllEmployeeIds();
    }
}