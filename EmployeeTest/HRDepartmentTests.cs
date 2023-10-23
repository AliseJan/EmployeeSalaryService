using Employee_Salary_Service.Departments;
using Employee_Salary_Service.DTOs;
using Employee_Salary_Service.Exceptions;
using Employee_Salary_Service.Interfaces;
using FluentAssertions;
using Moq;

namespace IfEmployeeTests
{
    [TestClass]
    public class HRDepartmentTests
    {
        private HRDepartment _hrDepartment;
        private List<Employee> _employees;
        private Mock<IEmployeeHistoryService> _employeeHistoryServiceMock;
        private Mock<IEmployeeDetailsWrapper> _employeeDetailsWrapperMock;

        [TestInitialize]
        public void Setup()
        {
            _employees = new List<Employee>
        {
            new Employee { Id = 1, FullName = "Test One", HourlySalary = 20.0m }
        };

            _employeeHistoryServiceMock = new Mock<IEmployeeHistoryService>();
            _employeeDetailsWrapperMock = new Mock<IEmployeeDetailsWrapper>();

            _hrDepartment = new HRDepartment(_employees, _employeeHistoryServiceMock.Object, _employeeDetailsWrapperMock.Object);
        }

        [TestMethod]
        public void CreateHRDepartment_WithoutEmployeesList_ThrowsArgumentNullException()
        {
            Action action = () => new HRDepartment(null, _employeeHistoryServiceMock.Object, _employeeDetailsWrapperMock.Object);

            action.Should().Throw<ArgumentNullException>();
        }

        [TestMethod]
        public void CreateHRDepartment_WithoutHistoryService_ThrowsArgumentNullException()
        {
            Action action = () => new HRDepartment(_employees, null, _employeeDetailsWrapperMock.Object);

            action.Should().Throw<ArgumentNullException>();
        }

        [TestMethod]
        public void CreateHRDepartment_WithoutEmployeeDetailsService_ThrowsArgumentNullException()
        {
            Action action = () => new HRDepartment(_employees, _employeeHistoryServiceMock.Object, null);

            action.Should().Throw<ArgumentNullException>();
        }

        [TestMethod]
        public void AddEmployee_ValidEmployee_AddsEmployeeAndContract()
        {
            var employee = new Employee { Id = 11, FullName = "Test Name", HourlySalary = 20.0m };
            var contractStartDate = DateTime.Now;

            var result = _hrDepartment.AddEmployee(employee, contractStartDate);

            _employeeHistoryServiceMock.Verify(
                service => service.AddEmployeeContract(employee.Id, It.IsAny<EmployeeContract>()),
                Times.Once);
            result.Count.Should().Be(2);
        }

        [TestMethod]
        public void AddEmployee_DuplicateEmployee_ThrowsDuplicateEmployeeException()
        {
            var employee = new Employee { Id = 1, FullName = "Test One", HourlySalary = 20.0m };
            var contractStartDate = DateTime.Now;

            Action action = () => _hrDepartment.AddEmployee(employee, contractStartDate);

            action.Should().Throw<DuplicateEmployeeException>();
        }

        [TestMethod]
        public void AddEmployee_NullEmployee_ThrowsArgumentNullException()
        {
            Employee employee = null;
            var contractStartDate = DateTime.Now;

            Action action = () => _hrDepartment.AddEmployee(employee, contractStartDate);

            action.Should().Throw<ArgumentNullException>();
        }

        [TestMethod]
        [DataRow(-1)]
        [DataRow(0)]
        public void AddEmployee_InvalidEmployeeId_ThrowsArgumentOutOfRangeException(int id)
        {
            var employee = new Employee { Id = id, FullName = "Test Name", HourlySalary = 20.0m };
            var contractStartDate = DateTime.Now;

            Action action = () => _hrDepartment.AddEmployee(employee, contractStartDate);

            action.Should().Throw<ArgumentOutOfRangeException>();
        }

        [TestMethod]
        public void AddEmployee_InvalidEmployeeFullName_ThrowsArgumentException()
        {
            var employee = new Employee { Id = 3, FullName = "", HourlySalary = 20.0m };
            var contractStartDate = DateTime.Now;

            Action action = () => _hrDepartment.AddEmployee(employee, contractStartDate);

            action.Should().Throw<ArgumentException>();
        }

        [TestMethod]
        [DataRow(-1.0)]
        [DataRow(0.0)]
        public void AddEmployee_InvalidEmployeeHourlySalary_ThrowsArgumentOutOfRangeException(double salary)
        {
            var employee = new Employee { Id = 3, FullName = "Test Name", HourlySalary = Convert.ToDecimal(salary) };
            var contractStartDate = DateTime.Now;

            Action action = () => _hrDepartment.AddEmployee(employee, contractStartDate);

            action.Should().Throw<ArgumentOutOfRangeException>();
        }

        [TestMethod]
        public void AddEmployee_InvalidDate_ThrowsArgumentOutOfRangeException()
        {
            var employee = new Employee { Id = 3, FullName = "Test Name", HourlySalary = 20.0m };
            var contractStartDate = DateTime.Now.AddDays(-1);

            Action action = () => _hrDepartment.AddEmployee(employee, contractStartDate);

            action.Should().Throw<ArgumentOutOfRangeException>();
        }

        [TestMethod]
        public void RemoveEmployee_ValidEmployee_RemovesEmployeeAndEndsContract()
        {
            var contractEndDate = DateTime.Now;
            var mockContract = new EmployeeContract(DateTime.Now, 20.0m);
            _employeeDetailsWrapperMock.Setup(
                wrapper => wrapper.GetCurrentContract(1, It.IsAny<IEmployeeHistoryService>()))
                .Returns(mockContract);

            var result = _hrDepartment.RemoveEmployee(1, contractEndDate);

            result.Count.Should().Be(0);
            mockContract.EndDate.Equals(contractEndDate);
            _employees.Any(e => e.Id == 1).Should().BeFalse();
        }

        [TestMethod]
        [DataRow(-1)]
        [DataRow(0)]
        public void RemoveEmployee_InvalidEmployeeId_ThrowsArgumentOutOfRangeException(int id)
        {
            var contractEndDate = DateTime.Now;

            Action action = () => _hrDepartment.RemoveEmployee(id, contractEndDate);

            action.Should().Throw<ArgumentOutOfRangeException>();
        }

        [TestMethod]
        public void RemoveEmployee_InvalidDate_ThrowsArgumentOutOfRangeException()
        {
            var employeeId = 1;
            var contractEndDate = DateTime.Now.AddDays(-1);

            Action action = () => _hrDepartment.RemoveEmployee(employeeId, contractEndDate);

            action.Should().Throw<ArgumentOutOfRangeException>();
        }

        [TestMethod]
        public void RemoveEmployee_NonExistingEmployee_ThrowsInvalidOperationException()
        {
            int employeeId = 999;
            DateTime contractEndDate = DateTime.Now;

            Action action = () => _hrDepartment.RemoveEmployee(employeeId, contractEndDate);

            action.Should().Throw<InvalidOperationException>();
        }

        [TestMethod]
        public void RemoveEmployee_NoActiveContract_ThrowsInvalidOperationException()
        {
            int employeeId = 1;
            DateTime contractEndDate = DateTime.Now;

            _employeeDetailsWrapperMock.Setup(
                wrapper => wrapper.GetCurrentContract(employeeId, It.IsAny<IEmployeeHistoryService>()))
                .Returns(new EmployeeContract(DateTime.Now.AddDays(-1), 7.5m) { EndDate = DateTime.Now});

            Action action = () => _hrDepartment.RemoveEmployee(employeeId, contractEndDate);

            action.Should().Throw<InvalidOperationException>()
                .WithMessage("Employee has no active contract");
        }

        [TestMethod]
        public void ReportHours_ValidInput_AddsWorkedHoursReport()
        {
            var employeeId = 1;
            var dateAndTime = DateTime.Now;
            var hours = 8;
            var minutes = 30;

            _hrDepartment.ReportHours(employeeId, dateAndTime, hours, minutes);

            _employeeHistoryServiceMock.Verify(
                service => service.AddReportedWorkHours(employeeId, It.IsAny<WorkedHours>()),
                Times.Once);
        }

        [TestMethod]
        [DataRow(-1)]
        [DataRow(0)]
        public void ReportHours_InvalidEmployeeId_ThrowsArgumentOutOfRangeException(int id)
        {
            var contractEndDate = DateTime.Now;

            Action action = () => _hrDepartment.ReportHours(id, DateTime.Now, 7, 30);

            action.Should().Throw<ArgumentOutOfRangeException>();
        }

        [TestMethod]
        [DataRow(-1)]
        [DataRow(25)]
        public void ReportHours_InvalidHours_ThrowsArgumentOutOfRangeException(int hours)
        {
            var contractEndDate = DateTime.Now;

            Action action = () => _hrDepartment.ReportHours(1, DateTime.Now, hours, 30);

            action.Should().Throw<ArgumentOutOfRangeException>();
        }

        [TestMethod]
        [DataRow(-1)]
        [DataRow(61)]
        public void ReportHours_InvalidMinutes_ThrowsArgumentOutOfRangeException(int minutes)
        {
            var contractEndDate = DateTime.Now;

            Action action = () => _hrDepartment.ReportHours(1, DateTime.Now, 7, minutes);

            action.Should().Throw<ArgumentOutOfRangeException>();
        }
    }
}