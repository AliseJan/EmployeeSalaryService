using Employee_Salary_Service.DTOs;
using Employee_Salary_Service.Exceptions;
using Employee_Salary_Service.Interfaces;
using Employee_Salary_Service.Services;

namespace Employee_Salary_Service.Departments
{
    public class HRDepartment : IHRDepartment
    {
        private readonly List<Employee> _employees;
        private readonly IEmployeeHistoryService _employeeHistory;
        private readonly IEmployeeDetailsWrapper _employeeDetailsWrapper;

        public HRDepartment(
            List<Employee> employees,
            IEmployeeHistoryService employeeHistory,
            IEmployeeDetailsWrapper employeeDetailsWrapper)
        {
            _employees =
                employees ?? throw new ArgumentNullException(
                    nameof(employees), "Employees list must be provided");
            _employeeHistory =
                employeeHistory ?? throw new ArgumentNullException(
                    nameof(employeeHistory), "Employee history data must be provided");
            _employeeDetailsWrapper =
                employeeDetailsWrapper ?? throw new ArgumentNullException(
                    nameof(employeeDetailsWrapper), "Employee details services must be provided");
        }

        public List<Employee> AddEmployee(Employee employee, DateTime contractStartDate)
        {
            if (employee == null)
            {
                throw new ArgumentNullException(nameof(employee), "Employee must not be null");
            }

            var validationExceptions = new List<Exception>();

            ValidationUtility.ValidatePositive(employee.Id, nameof(employee.Id), validationExceptions);
            ValidationUtility.ValidateNotEmpty(employee.FullName, nameof(employee.FullName), validationExceptions);
            ValidationUtility.ValidatePositive(employee.HourlySalary, nameof(employee.HourlySalary), validationExceptions);
            ValidationUtility.ValidateNotInPast(contractStartDate, nameof(contractStartDate), validationExceptions);

            if (_employees.Any(e => e.Id == employee.Id))
            {
                validationExceptions.Add(new DuplicateEmployeeException());
            }

            ValidationUtility.ThrowIfInvalid(validationExceptions);

            _employees.Add(employee);
            EmployeeContract contract = new EmployeeContract(contractStartDate, employee.HourlySalary);
            _employeeHistory.AddEmployeeContract(employee.Id, contract);

            return _employees;
        }

        public List<Employee> RemoveEmployee(int employeeId, DateTime contractEndDate)
        {
            var validationExceptions = new List<Exception>();

            ValidationUtility.ValidatePositive(employeeId, nameof(employeeId), validationExceptions);
            ValidationUtility.ValidateNotInPast(contractEndDate, nameof(contractEndDate), validationExceptions);
            ValidationUtility.ThrowIfInvalid(validationExceptions);

            var employeeToRemove = _employees.SingleOrDefault(e => e.Id == employeeId);

            if (employeeToRemove == null)
            {
                throw new InvalidOperationException("Employee not found");
            }

            EmployeeContract contract = _employeeDetailsWrapper.GetCurrentContract(employeeId, _employeeHistory);

            if (contract.EndDate != null)
            {
                throw new InvalidOperationException("Employee has no active contract");
            }

            contract.EndDate = contractEndDate;

            _employees.Remove(employeeToRemove);

            return _employees;
        }

        public void ReportHours(int employeeId, DateTime dateAndTime, int hours, int minutes)
        {
            var validationExceptions = new List<Exception>();

            ValidationUtility.ValidatePositive(employeeId, nameof(employeeId), validationExceptions);
            ValidationUtility.ValidateHourRange(hours, nameof(hours), validationExceptions);
            ValidationUtility.ValidateMinuteRange(minutes, nameof(minutes), validationExceptions);
            ValidationUtility.ThrowIfInvalid(validationExceptions);

            DateTime startTime = dateAndTime;
            DateTime endTime = dateAndTime.AddHours(hours).AddMinutes(minutes);

            decimal workedHours = (decimal)(endTime - startTime).TotalHours;
            WorkedHours workedHoursReport = new WorkedHours(dateAndTime, workedHours);

            _employeeHistory.AddReportedWorkHours(employeeId, workedHoursReport);
        }
    }
}