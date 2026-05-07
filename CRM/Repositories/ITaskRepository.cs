namespace CRM.Repositories
{
    public interface ITaskRepository
    {
        Task<bool> CreateTask(string data);
        Task<string> GetListTasks(int id, string data);
        Task<string> GetTask(int id);
        Task<string> GetTaskListByEmployee(int id);
    }
}
