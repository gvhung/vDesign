using System.ComponentModel;

namespace Base.OpenID
{
    public enum ServiceType
    {
        Unknown = -1,
        Google = 0,
        Facebook = 10,
        Yandex = 20,
        [Description("ЕСИА")]
        Esia = 30
    }
}
