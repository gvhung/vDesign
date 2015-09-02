
namespace Base.Settings.SettingValues
{
    public interface ISettingValue
    {
        string Type { get; }
        object Value { get;  }
    }
}
