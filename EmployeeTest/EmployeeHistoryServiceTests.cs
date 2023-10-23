using Employee_Salary_Service.DTOs;
using Employee_Salary_Service.Services;
using FluentAssertions;

namespace IfEmployeeTests
{
    [TestClass]
    public class EmployeeHistoryServiceTests
    {
        private Dictionary<int, List<EmployeeContract>> _contractHistory;
        private Dictionary<int, List<WorkedHours>> _workedHoursHistory;
        private EmployeeHistoryService _service;
        private int _employeeId;
        private EmployeeContract _contract;
        private WorkedHours _workedHours;

        [TestInitialize]
        public void Setup()
        {
            _contractHistory = new Dictionary<int, List<EmployeeContract>>();
            _workedHoursHistory = new Dictionary<int, List<WorkedHours>>();
            _service = new EmployeeHistoryService(_contractHistory, _workedHoursHistory);

            _employeeId = 1;
            _contract = new EmployeeContract(DateTime.Now, 20.0m);
            _workedHours = new WorkedHours(DateTime.Now, 7.5m);

            _service.AddEmployeeContract(_employeeId, _contract);
            _service.AddReportedWorkHours(_employeeId, _workedHours);
        }

        [TestMethod]
        public void CreateEmployeeHistoryService_WithoutContracts_ThrowsArgumentException()
        {
            Action action = () => new EmployeeHistoryService(null, _workedHoursHistory);

            action.Should().Throw<ArgumentException>();
        }

        [TestMethod]
        public void CreateEmployeeHistoryService_WithoutWorkedHours_ThrowsArgumentException()
        {
            Action action = () => new EmployeeHistoryService(_contractHistory, null);

            action.Should().Throw<ArgumentException>();
        }

        [TestMethod]
        public void AddEmployeeContract_ToNewEmployee_CreatesNewEmployeeRecordWithNewContract()
        {
            var newEmployeeId = 2;
            var newContract = new EmployeeContract(DateTime.Now, 20.0m);

            _service.AddEmployeeContract(newEmployeeId, newContract);

            _contractHistory.ContainsKey(newEmployeeId).Should().BeTrue();
            _contractHistory[newEmployeeId].Count.Should().Be(1);
            _contractHistory[newEmployeeId].Contains(newContract);
        }

        [TestMethod]
        public void AddEmployeeContract_ToExistingEmployee_AddsNewContractForExistingEmployee()
        {
            var secondContract = new EmployeeContract(DateTime.Now.AddDays(1), 25.0m);

            _service.AddEmployeeContract(_employeeId, secondContract);

            _contractHistory.ContainsKey(_employeeId).Should().BeTrue();
            _contractHistory[_employeeId].Count.Should().Be(2);
            _contractHistory[_employeeId].Contains(secondContract);
        }

        [TestMethod]
        public void GetEmployeeContractHistory_EmployeeHasContractsStored_ReturnsContractsList()
        {
            var contract1 = new EmployeeContract(DateTime.Now, 20.0m);
            var contract2 = new EmployeeContract(DateTime.Now.AddDays(1), 25.0m);
            _contractHistory[_employeeId] = new List<EmployeeContract> { contract1, contract2 };

            var contracts = _service.GetEmployeeContractHistory(_employeeId);

            contracts.Should().NotBeNull();
            contracts.Count.Should().Be(2);
            contracts.Contains(contract1);
        }

        [TestMethod]
        public void GetEmployeeContractHistory_EmployeeHasNoContractsStored_ReturnsNull()
        {
            var contracts = _service.GetEmployeeContractHistory(1234);

            contracts.Should().BeNull();
        }

        [TestMethod]
        public void AddReportedWorkedHours_ToNewEmployee_CreatesNewEmployeeRecordWithWorkedHours()
        {
            var newEmployeeId = 3;
            var hours = new WorkedHours(DateTime.Now, 7.5m);

            _service.AddReportedWorkHours(newEmployeeId, hours);

            _workedHoursHistory.ContainsKey(newEmployeeId).Should().BeTrue();
            _workedHoursHistory[newEmployeeId].Count.Should().Be(1);
            _workedHoursHistory[newEmployeeId].Contains(hours);
        }

        [TestMethod]
        public void AddReportedHours_ToExistingEmployee_AddsWorkingHoursToExistingEmployee()
        {
            var hoursYesterday = new WorkedHours(DateTime.Now.AddDays(-1), 6.5m);

            _service.AddReportedWorkHours(_employeeId, hoursYesterday);

            _workedHoursHistory.ContainsKey(_employeeId).Should().BeTrue();
            _workedHoursHistory[_employeeId].Count.Should().Be(2);
            _workedHoursHistory[_employeeId].Contains(hoursYesterday);
        }

        [TestMethod]
        public void GetEmployeeWorkedHoursHistory_ReturnsWorkedHoursList()
        {
            var hoursYesterday = new WorkedHours(DateTime.Now.AddDays(-1), 6.5m);

            _service.AddReportedWorkHours(_employeeId, hoursYesterday);

            var workedHours = _service.GetEmployeeWorkedHoursHistory(_employeeId);

            workedHours.Should().NotBeNull();
            workedHours.Count.Should().Be(2);
            workedHours.Contains(hoursYesterday);
        }

        [TestMethod]
        public void GetAllEmployeeIds_ReturnsListOfEmployeeIDs()
        {
            var employeeId2 = 2;
            var contract2 = new EmployeeContract(DateTime.Now, 15.0m);

            _service.AddEmployeeContract(employeeId2, contract2);

            var ids = _service.GetAllEmployeeIds();

            ids.Should().NotBeNull();
            ids.Count.Should().Be(2);
            ids.Contains(_employeeId).Should().BeTrue();
            ids.Contains(employeeId2).Should().BeTrue();
        }
    }
}