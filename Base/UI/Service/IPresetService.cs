using Base.DAL;
using System.Threading.Tasks;

namespace Base.UI
{
    public interface IPresetService
    {
        Preset GetPreset(IUnitOfWork unitOfWork, string mnemonic, string ownerMnemonic);
        Preset GetPreset(string mnemonic, string ownerMnemonic);
        T GetPreset<T>(IUnitOfWork unitOfWork, string ownerMnemonic) where T : Preset;
        T GetPreset<T>(string ownerMnemonic) where T : Preset;
        Task<Preset> GetPresetAsync(IUnitOfWork unitOfWork, string mnemonic, string ownerMnemonic);
        Task<Preset> GetPresetAsync(string mnemonic, string ownerMnemonic);
        Task<T> GetPresetAsync<T>(IUnitOfWork unitOfWork, string ownerMnemonic) where T : Preset;
        Task<T> GetPresetAsync<T>(string ownerMnemonic) where T : Preset;
        Task SavePresetAsync(IUnitOfWork unitOfWork, Preset preset);
        Task DeletePresetAsync(IUnitOfWork unitOfWork, Preset preset);
        Preset GetDefaultPreset(IUnitOfWork unitOfWork, string mnemonic, string ownerMnemonic);
        T GetDefaultPreset<T>(IUnitOfWork unitOfWork, string ownerMnemonic) where T : Preset;
        void SessionClear();
    }
}
