using MSAuth.Models;

namespace MSAuth.Interfaces
{
    public interface IEmployeeService
    {
        public Contact GetEmployeeById(string employeeId);
        public List<Contact> GetAllEmployees();
    }
}
