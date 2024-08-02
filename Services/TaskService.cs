using Microsoft.Xrm.Sdk.Query;
using Microsoft.Xrm.Sdk;
using MSAuth.Interfaces;
using MSAuth.Models;
using Microsoft.PowerPlatform.Dataverse.Client;

namespace MSAuth.Services
{
    public class TaskService : ITaskService
    {
        private readonly ServiceClient _service;

        public TaskService(ServiceClient service)
        {
            _service = service;
        }
        public List<CrmTask> GetTasksByEmployeeId(string employeeId)
        {
            QueryExpression query = new("hiring_crmlogs")
            {
                ColumnSet = new ColumnSet(true),
                Criteria = new FilterExpression()
            };

            query.Criteria.AddCondition("cr267_employee", ConditionOperator.Equal, employeeId);

            EntityCollection result = _service.RetrieveMultiple(query);

            List<CrmTask> tasks = [];

            foreach (var task in result.Entities)
            {
                tasks.Add((CrmTask)task);
            }

            return tasks;
        }
    }
}
