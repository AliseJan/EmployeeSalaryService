using Employee_Salary_Service.DTOs;

namespace Employee_Salary_Service.Interfaces
{
    public interface IHRDepartment
    {
        List<Employee> AddEmployee(Employee employee, DateTime contractStartDate);
        List<Employee> RemoveEmployee(int employeeId, DateTime contractEndDate);
        void ReportHours(int employeeId, DateTime dateAndTime, int hours, int minutes);
    }
}