namespace Employee_Salary_Service.Exceptions
{
    public class DuplicateEmployeeException : Exception
    {
        public DuplicateEmployeeException() : base("Cannot add employee with active contract")
        {
        }
    }
}