using MSAuth.Models;

namespace MSAuth.Interfaces
{
    public interface ITaskService
    {
        public List<CrmTask> GetTasksByEmployeeId(string employeeId);
    }
}
