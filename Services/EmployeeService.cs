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

        public List<Contact> SearchEmployees(string employeeName)
        {
            QueryExpression query = new("contact")
            {
                ColumnSet = new ColumnSet(true),
                Criteria = new FilterExpression()
            };

            if (!string.IsNullOrEmpty(employeeName))
            {
                query.Criteria.AddCondition("fullname", ConditionOperator.Like, $"%{employeeName}%");
            }

            EntityCollection result = _service.RetrieveMultiple(query);

            List<Contact> contacts = [];

            foreach (var contact in result.Entities)
            {
                contacts.Add((Contact)contact);
            }

            return contacts;
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

        public List<Contact> GetSortedEmployees(string sortCode)
        {
            List<Contact> contacts = [];

            QueryExpression query = new("contact")
            {
                ColumnSet = new ColumnSet(true),
            };

            switch (sortCode)
            {
                case "eaz":
                    query.AddOrder("fullname", OrderType.Ascending);
                    break;
                case "eza":
                    query.AddOrder("fullname", OrderType.Descending);
                    break;
                case "hno":
                    query.AddOrder("hiring_hiredate", OrderType.Ascending);
                    break;
                case "hon":
                    query.AddOrder("hiring_hiredate", OrderType.Descending);
                    break;
                default: return contacts;
            }

            EntityCollection result = _service.RetrieveMultiple(query);

            foreach (var contact in result.Entities)
            {
                contacts.Add((Contact)contact);
            }

            return contacts;
        }
    }
}
