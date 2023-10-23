using Employee_Salary_Service.DTOs;
using Employee_Salary_Service.Interfaces;

namespace Employee_Salary_Service
{
    public class Company : ICompany
    {
        private List<Employee> _employees;
        private readonly IHRDepartment _hr;
        private readonly IBookkeepingDepartment _bookkeeping;
        public Employee[] Employees => _employees.ToArray();
        public string Name { get; }

        public Company(string name, IHRDepartment hr, IBookkeepingDepartment bookkeeping)
        {
            Name = name ?? throw new ArgumentException("Name cannot be empty");
            _hr = hr ?? throw new ArgumentException("HR is required");
            _bookkeeping = bookkeeping ?? throw new ArgumentException("Bookkeeping is required");
            _employees = new List<Employee>();
        }

        public void AddEmployee(Employee employee, DateTime contractStartDate)
        {
            _employees = _hr.AddEmployee(employee, contractStartDate);
        }

        public void RemoveEmployee(int employeeId, DateTime contractEndDate)
        {
            _employees = _hr.RemoveEmployee(employeeId, contractEndDate);
        }

        public void ReportHours(int employeeId, DateTime dateAndTime, int hours, int minutes)
        {
            _hr.ReportHours(employeeId, dateAndTime, hours, minutes);
        }

        public EmployeeMonthlyReport[] GetMonthlyReport(DateTime periodStartDate, DateTime periodEndDate)
        {
            return _bookkeeping.GetMonthlyReport(periodStartDate, periodEndDate);          
        }
    }
}