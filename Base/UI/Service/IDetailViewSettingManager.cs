using Base.DAL;
using System.Collections.Generic;

namespace Base.UI.Service
{
    public interface IDetailViewSettingManager
    {
        IEnumerable<DetailViewSetting> GetSettings(IUnitOfWork unitOfWork, string mnemonic, BaseObject obj);
    }
}
