namespace Base.BusinessProcesses.Services.Abstract
{
    public interface IWorkflowCacheService
    {
        string Get(string key);

        string Get(string key, string wfID);

        void Add(string key, string json);

        void Add(string key, string json, int wfID);

        void Clear(string key);
    }
}
