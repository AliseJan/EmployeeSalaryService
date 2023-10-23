using Employee_Salary_Service.DTOs;
using Employee_Salary_Service.Interfaces;

namespace Employee_Salary_Service.Services
{
    public class EmployeeHistoryService : IEmployeeHistoryService
    {
        private readonly Dictionary<int, List<EmployeeContract>> _employeeContractHistory;
        private readonly Dictionary<int, List<WorkedHours>> _employeeWorkedHoursHistory;

        public EmployeeHistoryService(Dictionary<int, List<EmployeeContract>> employeeContractHistory,
            Dictionary<int, List<WorkedHours>> employeeWorkedHoursHistory)
        {
            _employeeContractHistory = employeeContractHistory ?? throw new ArgumentException(
                "Employee contract history is required");
            _employeeWorkedHoursHistory = employeeWorkedHoursHistory ?? throw new ArgumentException(
                "Employee worked hours history is required");
        }

        public void AddEmployeeContract(int employeeId, EmployeeContract contract)
        {
            if (!_employeeContractHistory.ContainsKey(employeeId))
                _employeeContractHistory[employeeId] = new List<EmployeeContract>();

            _employeeContractHistory[employeeId].Add(contract);
        }

        public List<EmployeeContract>? GetEmployeeContractHistory(int employeeID)
        {
            if (_employeeContractHistory.TryGetValue(employeeID, out var contracts))
            {
                return contracts;
            }

            return null;
        }

        public void AddReportedWorkHours(int employeeId, WorkedHours hours)
        {
            if (!_employeeWorkedHoursHistory.ContainsKey(employeeId))
                _employeeWorkedHoursHistory[employeeId] = new List<WorkedHours>();

            _employeeWorkedHoursHistory[employeeId].Add(hours);
        }

        public List<WorkedHours> GetEmployeeWorkedHoursHistory(int employeeID)
        {
            return _employeeWorkedHoursHistory[employeeID];
        }

        public List<int> GetAllEmployeeIds()
        {
            List<int> ids = new List<int>(_employeeContractHistory.Keys);
            return ids;
        }
    }
}