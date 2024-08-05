using MSAuth.Models;

namespace MSAuth.Interfaces
{
    public interface IEmployeeService
    {
        public Contact GetEmployeeById(string employeeId);
        public List<Contact> GetAllEmployees();
        public List<Contact> SearchEmployees(string employeeName);
        public List<Contact> GetSortedEmployees(string sortCode);
    }
}
