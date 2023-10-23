using Employee_Salary_Service.DTOs;
using Employee_Salary_Service.Interfaces;

namespace Employee_Salary_Service.Services
{
    public static class EmployeeDetails
    {
        public static EmployeeContract GetContract(int employeeID, IEmployeeHistoryService history, DateTime date)
        {
            var contracts = history.GetEmployeeContractHistory(employeeID);
            return contracts.LastOrDefault(c => c.StartDate <= date && (c.EndDate == null || c.EndDate > date));
        }

        public static EmployeeContract GetCurrentContract(int employeeID, IEmployeeHistoryService history)
        {
            var contract = history.GetEmployeeContractHistory(employeeID);
            return contract?.LastOrDefault();
        }

        public static decimal GetSalaryRate(int employeeID, IEmployeeHistoryService history, DateTime date)
        {
            var contract = GetContract(employeeID, history, date);
            var salaryRate = contract.SalaryRates.LastOrDefault(sr => sr.StartDate <= date && (sr.EndDate == null || sr.EndDate > date));
            return salaryRate.Amount;
        }

        public static void UpdateSalary(int employeeId, IEmployeeHistoryService history, decimal newSalary, DateTime date)
        {
            EmployeeContract contract = GetContract(employeeId, history, date);
            contract.ChangeSalary(date, newSalary);
        }

        public static decimal GetWorkedHoursForMonth(int employeeID, IEmployeeHistoryService history, DateTime date)
        {
            var workedHoursHistory = history.GetEmployeeWorkedHoursHistory(employeeID);
            var totalWorkedHours = 0.0m;
            foreach (var workedHours in workedHoursHistory)
            {
                if (workedHours.Date.Year == date.Year && workedHours.Date.Month == date.Month)
                {
                    totalWorkedHours += workedHours.Hours;
                }
            }
            return totalWorkedHours;
        }
    }
}