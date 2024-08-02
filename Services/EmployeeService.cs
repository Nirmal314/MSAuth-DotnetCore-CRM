using Microsoft.PowerPlatform.Dataverse.Client;
using Microsoft.Xrm.Sdk;
using Microsoft.Xrm.Sdk.Query;
using MSAuth.Interfaces;
using MSAuth.Models;

namespace MSAuth.Services
{
    public class EmployeeService : IEmployeeService
    {
        private readonly ServiceClient _service;
        public EmployeeService(ServiceClient service)
        {
            _service = service;
        }
        public Contact GetEmployeeById(string employeeId)
        {
            Entity employee = _service.Retrieve("contact", new Guid(employeeId), new ColumnSet(true));
            return (Contact)employee;
        }

        public List<Contact> GetAllEmployees()
        {
            QueryExpression query = new("contact")
            {
                ColumnSet = new ColumnSet(true),
            };

            EntityCollection result = _service.RetrieveMultiple(query);

            List<Contact> contacts = [];

            foreach (var contact in result.Entities)
            {
                contacts.Add((Contact)contact);
            }

            return contacts;
        }
    }
}
