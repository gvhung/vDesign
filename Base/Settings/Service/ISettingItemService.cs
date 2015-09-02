using Base.Service;
using System;

namespace Base.Settings
{
    public interface ISettingItemService : IBaseCategorizedItemService<SettingItem>
    {
        object GetValue(Guid key, object bydef);
    }
}
