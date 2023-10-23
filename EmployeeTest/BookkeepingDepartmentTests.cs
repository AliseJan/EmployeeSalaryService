using Employee_Salary_Service.Departments;
using Employee_Salary_Service.DTOs;
using Employee_Salary_Service.Interfaces;
using FluentAssertions;
using Moq;

namespace IfEmployeeTests
{
    [TestClass]
    public class BookkeepingDepartmentTests
    {
        private Mock<IEmployeeHistoryService> _employeeHistoryServiceMock;
        private Mock<IEmployeeDetailsWrapper> _employeeDetailsWrapperMock;
        private BookkeepingDepartment _bookkeepingDepartment;

        [TestInitialize]
        public void Setup()
        {
            _employeeHistoryServiceMock = new Mock<IEmployeeHistoryService>();
            _employeeDetailsWrapperMock = new Mock<IEmployeeDetailsWrapper>();
            _bookkeepingDepartment = new BookkeepingDepartment(_employeeHistoryServiceMock.Object, _employeeDetailsWrapperMock.Object);
        }

        [TestMethod]
        public void CreateBookkeepingDepartment_WithoutHistoryService_ThrowsArgumentNullException()
        {
            Action action = () => new BookkeepingDepartment(null, _employeeDetailsWrapperMock.Object);

            action.Should().Throw<ArgumentNullException>();
        }

        [TestMethod]
        public void CreateBookkeepingDepartment_WithoutEmployeeDetailsService_ThrowsArgumentNullException()
        {
            Action action = () => new BookkeepingDepartment(_employeeHistoryServiceMock.Object, null);

            action.Should().Throw<ArgumentNullException>();
        }

        [TestMethod]
        public void GetMonthlyReport_WithInvalidPeriodEndDate_ThrowsInvalidOperationException()
        {
            var periodStartDate = new DateTime(2023, 9, 1);
            var periodEndDate = new DateTime(2023, 8, 31);

            Action action = () => _bookkeepingDepartment.GetMonthlyReport(periodStartDate, periodEndDate);

            action.Should().Throw<InvalidOperationException>();
        }

        [TestMethod]
        public void GetMonthlyReport_WithValidDates_ReturnsMontlyReport()
        {
            _employeeHistoryServiceMock.Setup(service => service.GetAllEmployeeIds())
                .Returns(new List<int> { 1, 2 });

            var contract1 = new EmployeeContract(new DateTime(2023, 7, 1), 10m);
            contract1.ChangeSalary(new DateTime(2023, 8, 1), 15m);
            var contracts1 = new List<EmployeeContract> { contract1 };

            _employeeHistoryServiceMock.Setup(service => service.GetEmployeeContractHistory(1))
                .Returns(contracts1);

            var contract2 = new EmployeeContract(new DateTime(2023, 7, 1), 15m);
            contract2.ChangeSalary(new DateTime(2023, 8, 1), 20m);
            var contracts2 = new List<EmployeeContract> { contract2 };

            _employeeHistoryServiceMock.Setup(service => service.GetEmployeeContractHistory(2))
                .Returns(contracts2);

            var workedHours1 = new List<WorkedHours>
            {
                new WorkedHours(new DateTime(2023, 7, 1), 160),
                new WorkedHours(new DateTime(2023, 8, 1), 150),
            };

            _employeeHistoryServiceMock.Setup(service => service.GetEmployeeWorkedHoursHistory(1))
                .Returns(workedHours1);

            var workedHours2 = new List<WorkedHours>
            {
                new WorkedHours(new DateTime(2023, 7, 1), 150),
                new WorkedHours(new DateTime(2023, 8, 1), 140),
            };

            _employeeHistoryServiceMock.Setup(service => service.GetEmployeeWorkedHoursHistory(2))
                .Returns(workedHours2);

            _employeeDetailsWrapperMock.Setup(wrapper => wrapper.GetContract(It.IsAny<int>(), It.IsAny<IEmployeeHistoryService>(), It.IsAny<DateTime>()))
                .Returns<int, IEmployeeHistoryService, DateTime>((employeeId, history, date) =>
                {
                    var contracts = _employeeHistoryServiceMock.Object.GetEmployeeContractHistory(employeeId);
                    return contracts.FirstOrDefault(contract => contract.StartDate <= date && (contract.EndDate == null || contract.EndDate >= date));
                });

            _employeeDetailsWrapperMock.Setup(wrapper => wrapper.GetWorkedHoursForMonth(It.IsAny<int>(), It.IsAny<IEmployeeHistoryService>(), It.IsAny<DateTime>()))
                .Returns<int, IEmployeeHistoryService, DateTime>((employeeId, history, date) =>
                {
                    var workedHours = _employeeHistoryServiceMock.Object.GetEmployeeWorkedHoursHistory(employeeId);
                    return workedHours.FirstOrDefault(hours => hours.Date.Year == date.Year && hours.Date.Month == date.Month)?.Hours ?? 0;
                });

            _employeeDetailsWrapperMock.Setup(wrapper => wrapper.GetSalaryRate(It.IsAny<int>(), It.IsAny<IEmployeeHistoryService>(), It.IsAny<DateTime>()))
                .Returns<int, IEmployeeHistoryService, DateTime>((employeeId, history, date) =>
                {
                    var contract = _employeeDetailsWrapperMock.Object.GetContract(employeeId, history, date);
                    return contract?.SalaryRates.LastOrDefault(rate => rate.StartDate <= date)?.Amount ?? 0;
                });

            var periodStartDate = new DateTime(2023, 7, 1);
            var periodEndDate = new DateTime(2023, 8, 31);

            var monthlyReport = _bookkeepingDepartment.GetMonthlyReport(periodStartDate, periodEndDate);

            monthlyReport.Length.Should().Be(4);
            monthlyReport[0].EmployeeId.Should().Be(1);
            monthlyReport[0].Year.Should().Be(2023);
            monthlyReport[0].Month.Should().Be(7);
            monthlyReport[0].Salary.Should().Be(1600m);
            monthlyReport[3].Salary.Should().Be(2800m);
        }
    }
}