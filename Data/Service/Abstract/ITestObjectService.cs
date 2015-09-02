using Base.BusinessProcesses.Entities;
using Base.Service;
using Data.Entities;

namespace Data.Service.Abstract
{
    public interface ITestObjectService : IBaseObjectService<TestObject>, IWFObjectService
    {
    }

    public interface ITestObjectEntryService : IBaseObjectService<TestObjectEntry>
    {
    }

    public interface ITestObjectNestedEntryService : IBaseObjectService<TestObjectNestedEntry>
    {
    }
}
