using Employee_Salary_Service.DTOs;
using Employee_Salary_Service.Interfaces;
using Employee_Salary_Service.Services;
using FluentAssertions;
using Moq;

namespace EmployeeTest
{
    [TestClass]
    public class EmployeeDetailsTests
    {
        private Mock<IEmployeeHistoryService> _employeeHistoryServiceMock;
        private Dictionary<int, List<EmployeeContract>> _employeeContractHistory;
        private List<EmployeeContract> _contracts;

        [TestInitialize]
        public void Setup()
        {
            _employeeHistoryServiceMock = new Mock<IEmployeeHistoryService>();
            _employeeContractHistory = new Dictionary<int, List<EmployeeContract>>();
            _contracts = new List<EmployeeContract>
            {
                new EmployeeContract(new DateTime(2023, 7, 1), 15.0m) {EndDate = new DateTime(2023, 7, 31)},
                new EmployeeContract(new DateTime(2023, 8, 1), 20.0m) {EndDate = new DateTime(2023, 8, 31)},
                new EmployeeContract(new DateTime(2023, 9, 1), 25.0m),
            };

            _employeeContractHistory = new Dictionary<int, List<EmployeeContract>>
            {
                {1, _contracts }
            };

            _employeeHistoryServiceMock
            .Setup(history => history.GetEmployeeContractHistory(It.IsAny<int>()))
            .Returns<int>(employeeId =>
            {
                if (_employeeContractHistory.TryGetValue(employeeId, out var _contracts))
                {
                    return _contracts;
                }
                return null;
            });
        }

        [TestMethod]
        public void GetContract_AnyContractPeriodIncludesDate_ReturnsCorrectContract()
        {
            var employeeID = 1;
            var date = new DateTime(2023, 8, 2);

            var contract = EmployeeDetails.GetContract(employeeID, _employeeHistoryServiceMock.Object, date);

            contract.Should().NotBeNull();
            contract.StartDate.Should().Be(new DateTime(2023, 8, 1));
            contract.SalaryRates.Should().HaveCount(1);
            contract.SalaryRates[0].Amount.Should().Be(20.0m);
        }

        [TestMethod]
        public void GetContract_NoneContractPeriodIncludesDate_ReturnsNull()
        {
            var employeeID = 1;
            var date = new DateTime(2023, 6, 2);

            var contract = EmployeeDetails.GetContract(employeeID, _employeeHistoryServiceMock.Object, date);

            contract.Should().BeNull();
        }

        [TestMethod]
        public void GetCurrentContract_EmployeeHasAtLeast1Contract_ReturnsCurrentContract()
        {
            var employeeID = 1;

            var contract = EmployeeDetails.GetCurrentContract(employeeID, _employeeHistoryServiceMock.Object);

            contract.Should().NotBeNull();
            contract.StartDate.Should().Be(new DateTime(2023, 9, 1));
            contract.SalaryRates.Should().HaveCount(1);
            contract.SalaryRates[0].Amount.Should().Be(25.0m);
        }

        [TestMethod]
        public void GetCurrentContract_EmployeeHasNoContracts_ReturnsNull()
        {
            var employeeID = 2;

            var contract = EmployeeDetails.GetCurrentContract(employeeID, _employeeHistoryServiceMock.Object);

            contract.Should().BeNull();
        }

        [TestMethod]
        public void GetSalaryRate_ReturnsSalaryRate()
        {
            var employeeId = 1;
            var date = new DateTime(2023, 8, 10);
            var expectedSalaryRate = 20.0m;

            var result = EmployeeDetails.GetSalaryRate(employeeId, _employeeHistoryServiceMock.Object, date);

            result.Should().Be(expectedSalaryRate);
        }

        [TestMethod]
        public void UpdateSalary_SalaryIsUpdated()
        {
            var employeeId = 1;
            var date = new DateTime(2023, 9, 2);
            var newSalary = 30.0m;

            EmployeeDetails.UpdateSalary(employeeId, _employeeHistoryServiceMock.Object, newSalary, date);

            var updatedContract = EmployeeDetails.GetContract(employeeId, _employeeHistoryServiceMock.Object, date);

            updatedContract.SalaryRates.Count.Should().Be(2);
            updatedContract.SalaryRates.Last().Amount.Should().Be(30.0m);
        }

        [TestMethod]
        public void GetWorkedHoursForMonth_ReturnsMonthlyWorkedHoursSummary()
        {
            var employeeId = 1;
            var date = new DateTime(2023, 8, 1);
            var workedHoursInMonth = new List<WorkedHours>
            {
                new WorkedHours(new DateTime(2023, 8, 5), 8.0m),
                new WorkedHours(new DateTime(2023, 8, 12), 7.5m),
                new WorkedHours(new DateTime(2023, 9, 1), 6.0m),
            };

            _employeeHistoryServiceMock
                .Setup(history => history.GetEmployeeWorkedHoursHistory(employeeId))
                .Returns(workedHoursInMonth);

            var totalWorkedHours = EmployeeDetails.GetWorkedHoursForMonth(employeeId, _employeeHistoryServiceMock.Object, date);

            var expectedTotalWorkedHours = workedHoursInMonth
                .Where(wh => wh.Date.Year == date.Year && wh.Date.Month == date.Month)
                .Sum(wh => wh.Hours);

            totalWorkedHours.Should().Be(expectedTotalWorkedHours);
        }

    }
}