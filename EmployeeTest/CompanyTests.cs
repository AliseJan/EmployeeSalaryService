using Employee_Salary_Service;
using Employee_Salary_Service.DTOs;
using Employee_Salary_Service.Interfaces;
using FluentAssertions;
using Moq;

namespace CompanyTests
{
    [TestClass]
    public class CompanyTests
    {
        private Mock<IHRDepartment> _hrMock;
        private Mock<IBookkeepingDepartment> _bookkeepingMock;
        private List<Employee> _employeesFromHR;
        private Company _company;
        private EmployeeMonthlyReport[] _expectedReports;

        [TestInitialize]
        public void Setup()
        {
            _hrMock = new Mock<IHRDepartment>();

            _employeesFromHR = new List<Employee>
            {
                new Employee { Id = 1, FullName = "Test One", HourlySalary = 20.0m },
                new Employee { Id = 2, FullName = "Test Two", HourlySalary = 18.0m }
            };

            _hrMock.Setup(hr => hr.AddEmployee(It.IsAny<Employee>(), It.IsAny<DateTime>()))
                .Returns((Employee employee, DateTime contractStartDate) =>                
                {
                    _employeesFromHR.Add(employee);
                    return _employeesFromHR;
                });

            _hrMock.Setup(hr => hr.RemoveEmployee(It.IsAny<int>(), It.IsAny<DateTime>()))
                .Returns((int employeeId, DateTime contractEndDate) =>
                {
                     var employeeToRemove = _employeesFromHR.Find(e => e.Id == employeeId);
                     if (employeeToRemove != null)
                     {
                     _employeesFromHR.Remove(employeeToRemove);
                     }
                     return _employeesFromHR;
                });

            _bookkeepingMock = new Mock<IBookkeepingDepartment>();

            _expectedReports = new EmployeeMonthlyReport[]
            {
                new EmployeeMonthlyReport { EmployeeId = 1, Year = 2023, Month = 1, Salary = 2500.0m },
                new EmployeeMonthlyReport { EmployeeId = 2, Year = 2023, Month = 1, Salary = 2100.0m }
            };

            _bookkeepingMock.Setup(b => b.GetMonthlyReport(It.IsAny<DateTime>(), It.IsAny<DateTime>()))
                .Returns(_expectedReports);

            _company = new Company("TestCompany", _hrMock.Object, _bookkeepingMock.Object);
        }

        [TestMethod]
        public void CreateCompany_SetNameParameter_NameSetCorrectly()
        {
            _company.Name.Should().Be("TestCompany");
        }

        [TestMethod]
        public void CreateCompany_WithoutName_ThrowsArgumentException()
        {
            Action action = () => new Company(null, _hrMock.Object, _bookkeepingMock.Object);

            action.Should().Throw<ArgumentException>();
        }

        [TestMethod]
        public void CreateCompany_WithoutHR_ThrowsArgumentException()
        {
            Action action = () => new Company("Test", null, _bookkeepingMock.Object);

            action.Should().Throw<ArgumentException>();
        }

        [TestMethod]
        public void CreateCompany_WithoutBookkeeping_ThrowsArgumentException()
        {
            Action action = () => new Company("Test", _hrMock.Object, null);

            action.Should().Throw<ArgumentException>();
        }

        [TestMethod]
        public void EmployeesProperty_ShouldReturnCorrectValues()
        {
            var employeesField = _company.GetType().GetField("_employees", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
            employeesField.SetValue(_company, _employeesFromHR);

            var employees = _company.Employees;

            employees.Should().NotBeNull();
            employees.Should().HaveCount(2);
            employees.Should().Contain(e => e.Id == 1 && e.FullName == "Test One" && e.HourlySalary == 20.0m);
            employees.Should().Contain(e => e.Id == 2 && e.FullName == "Test Two" && e.HourlySalary == 18.0m);
        }

        [TestMethod]
        public void AddEmployee_CallsHRToAddEmployee()
        {
            var employee = new Employee { Id = 11, FullName = "Test Name", HourlySalary = 20.0m };
            var contractStartDate = DateTime.Now;

            _company.AddEmployee(employee, contractStartDate);

            _hrMock.Verify(hr => hr.AddEmployee(It.IsAny<Employee>(), It.IsAny<DateTime>()), Times.Once);
        }

        [TestMethod]
        public void AddEmployee_ReturnsEmployeeListWithAddedEmployee()
        {
            var employee = new Employee { Id = 3, FullName = "Test Three", HourlySalary = 22.0m };
            var contractStartDate = DateTime.Now;

            _company.AddEmployee(employee, contractStartDate);

            _employeesFromHR.Should().Contain(employee);
        }

        [TestMethod]
        public void RemoveEmployee_CallsHRToRemoveEmployee()
        {
            var employeeId = 1;
            var contractEndDate = DateTime.Now;

            _company.RemoveEmployee(employeeId, contractEndDate);

            _hrMock.Verify(hr => hr.RemoveEmployee(It.IsAny<int>(), It.IsAny<DateTime>()), Times.Once);
        }

        [TestMethod]
        public void RemoveEmployee_ReturnsEmployeeListWithoutRemovedEmployee()
        {
            _company.RemoveEmployee(1, DateTime.Now);

            _employeesFromHR.Any(e => e.Id == 1).Should().BeFalse();
        }

        [TestMethod]
        public void ReportHours_CallsHRToReportHours()
        {
            var employeeId = 1;
            var dateAndTime = DateTime.Now;
            var hours = 8;
            var minutes = 30;

            _company.ReportHours(employeeId, dateAndTime, hours, minutes);

            _hrMock.Verify(hr => hr.ReportHours(It.IsAny<int>(), It.IsAny<DateTime>(), It.IsAny<int>(), It.IsAny<int>()), Times.Once);
        }

        [TestMethod]
        public void GetMonthlyReport_CallsBookkeepingToGetMonthlyReport()
        {
            var periodStartDate = DateTime.Now.AddMonths(-1);
            var periodEndDate = DateTime.Now;

            _company.GetMonthlyReport(periodStartDate, periodEndDate);

            _bookkeepingMock.Verify(bk => bk.GetMonthlyReport(It.IsAny<DateTime>(), It.IsAny<DateTime>()), Times.Once);
        }

        [TestMethod]
        public void GetMonthlyReport_ReturnsArrayOfMonthlyReports()
        {
            var periodStartDate = new DateTime(2023, 1, 1);
            var periodEndDate = new DateTime(2023, 1, 31);

            var reports = _company.GetMonthlyReport(periodStartDate, periodEndDate);

            reports.Should().NotBeNull();
            reports.Should().BeEquivalentTo(_expectedReports);
        }
    }   
}