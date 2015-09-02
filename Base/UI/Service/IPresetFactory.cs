using Base.DAL;

namespace Base.UI
{
    public interface IPresetFactory
    {
        Preset CreatePreset(IUnitOfWork unitOfWork, string mnemonic, string ownerMnemonic);
        Preset CreatePreset<T>(IUnitOfWork unitOfWork, string mnemonic, string ownerMnemonic) where T : Preset;
    }
}
