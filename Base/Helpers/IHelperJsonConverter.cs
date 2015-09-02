using System;

namespace Base.Helpers
{
    public interface IHelperJsonConverter
    {
        string SerializeObject(BaseObject obj, bool completeGraph = false);
        BaseObject DeserializeObject(string value, Type type);
        T DeserializeObject<T>(string value) where T : BaseObject;
    }
}
