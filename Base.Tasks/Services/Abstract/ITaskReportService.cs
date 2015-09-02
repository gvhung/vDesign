using Base.Service;
using Base.Task.Entities;

namespace Base.Task.Services
{
    public interface ITaskReportService : IBaseCategorizedItemService<TaskReport>, IReadOnly
    {   
    }
}
