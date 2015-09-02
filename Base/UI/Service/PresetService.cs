using Base.Ambient;
using Base.DAL;
using Base.QueryableExtensions;
using Base.Service;
using Base.UI.Service;
using Framework.Wrappers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Base.UI
{
    public class PresetService : BaseObjectService<PresetRegistor>, IPresetService
    {
        private const string PresetKey = "{38BAAAAE-9981-4489-9C39-DBD0DDC02316}";

        private static readonly object _sessionLock = new object();

        private readonly ISessionWrapper _session;
        private readonly IPresetFactory _presetFactory;
        private readonly IUiFasade _uiFasade;
        private readonly IUnitOfWorkFactory _unitOfWorkFactory;

        public PresetService(IBaseObjectServiceFacade facade, ISessionWrapper session, IPresetFactory presetFactory, IUiFasade uiFasade, IUnitOfWorkFactory unitOfWorkFactory)
            : base(facade)
        {
            _session = session;
            _presetFactory = presetFactory;
            _uiFasade = uiFasade;
            _unitOfWorkFactory = unitOfWorkFactory;
        }

        private string GetRegistorKey(string mnemonic, string ownerMnemonic)
        {
            return String.Format("{0}:{1}:{2}", mnemonic, ownerMnemonic, AppContext.SecurityUser.GetKey());
        }

        private Dictionary<string, Preset> GetCache()
        {
            if (_session[PresetKey] == null)
            {
                _session[PresetKey] = new Dictionary<string, Preset>();
            }

            return _session[PresetKey] as Dictionary<string, Preset>;

        }

        private Preset GetCachePreset(string ownerMnemonic)
        {
            lock (_sessionLock)
            {
                var presets = this.GetCache();

                return presets.ContainsKey(ownerMnemonic) ? presets[ownerMnemonic] : null;
            }
        }

        private void SaveCachePreset(Preset preset)
        {
            var presets = this.GetCache();

            string key = preset.OwnerMnemonic;

            if (!presets.ContainsKey(key))
                presets.Add(key, preset);
            else
                presets[key] = preset;
        }

        private void RemoveCachePreset(string ownerMnemonic)
        {
            var presets = this.GetCache();

            if (presets.ContainsKey(ownerMnemonic))
                presets.Remove(ownerMnemonic);
        }

        public Preset GetPreset(IUnitOfWork unitOfWork, string mnemonic, string ownerMnemonic)
        {
            var preset = this.GetCachePreset(ownerMnemonic);

            if (preset != null) return preset;

            string key = this.GetRegistorKey(mnemonic, ownerMnemonic);

            unitOfWork = unitOfWork ?? _unitOfWorkFactory.CreateSystem();

            var presetDb = unitOfWork.GetRepository<PresetRegistor>().All().FirstOrDefault(x => x.Key == key);

            if (presetDb != null && presetDb.Preset != null)
            {
                preset = presetDb.Preset.Init(_uiFasade);
                preset.Modified = true;

            }
            else
            {
                preset = this.GetDefaultPreset(unitOfWork, mnemonic, ownerMnemonic);
            }

            this.SaveCachePreset(preset);

            return preset;
        }

        public Preset GetPreset(string mnemonic, string ownerMnemonic)
        {
            return GetPreset(null, mnemonic, ownerMnemonic);
        }

        public T GetPreset<T>(IUnitOfWork unitOfWork, string ownerMnemonic) where T : Preset
        {
            return GetPreset(unitOfWork, _uiFasade.GetViewModelConfig(typeof(T)).Mnemonic, ownerMnemonic) as T;
        }

        public T GetPreset<T>(string ownerMnemonic) where T : Preset
        {
            return GetPreset<T>(null, ownerMnemonic);
        }

        public async Task<Preset> GetPresetAsync(IUnitOfWork unitOfWork, string mnemonic, string ownerMnemonic)
        {
            var preset = this.GetCachePreset(ownerMnemonic);

            if (preset != null) return preset;

            string key = this.GetRegistorKey(mnemonic, ownerMnemonic);

            unitOfWork = unitOfWork ?? _unitOfWorkFactory.CreateSystem();

            var presetDb = await unitOfWork.GetRepository<PresetRegistor>().All().Where(x => x.Key == key).FirstOrDefaultAsync().ConfigureAwait(false);

            if (presetDb != null && presetDb.Preset != null)
            {
                preset = presetDb.Preset.Init(_uiFasade);

                preset.Modified = true;
            }
            else
            {
                preset = this.GetDefaultPreset(unitOfWork, mnemonic, ownerMnemonic);
            }

            this.SaveCachePreset(preset);

            return preset;
        }

        public Task<Preset> GetPresetAsync(string mnemonic, string ownerMnemonic)
        {
            return GetPresetAsync(null, mnemonic, ownerMnemonic);
        }

        public async Task<T> GetPresetAsync<T>(IUnitOfWork unitOfWork, string ownerMnemonic) where T : Preset
        {
            return await this.GetPresetAsync(unitOfWork, _uiFasade.GetViewModelConfig(typeof(T)).Mnemonic, ownerMnemonic) as T;
        }

        public Task<T> GetPresetAsync<T>(string ownerMnemonic) where T : Preset
        {
            return GetPresetAsync<T>(null, ownerMnemonic);
        }

        public async Task SavePresetAsync(IUnitOfWork unitOfWork, Preset preset)
        {
            string key = this.GetRegistorKey(preset.Mnemonic, preset.OwnerMnemonic);

            var presetDb = await GetAll(unitOfWork)
                .Where(x => x.Key == key)
                .FirstOrDefaultAsync()
                .ConfigureAwait(false);

            if (!preset.IsInit)
            {
                preset.Init(_uiFasade);
            }

            if (presetDb == null)
            {
                presetDb = new PresetRegistor()
                {
                    Key = this.GetRegistorKey(preset.Mnemonic, preset.OwnerMnemonic),
                    Preset = preset,
                };

                this.Create(unitOfWork, presetDb);
            }
            else
            {
                presetDb.Preset = preset;

                this.Update(unitOfWork, presetDb);
            }

            await unitOfWork.SaveChangesAsync().ConfigureAwait(false);

            this.RemoveCachePreset(preset.OwnerMnemonic);
        }

        public async Task DeletePresetAsync(IUnitOfWork unitOfWork, Preset preset)
        {
            string key = this.GetRegistorKey(preset.Mnemonic, preset.OwnerMnemonic);

            var presetDb = await unitOfWork.GetRepository<PresetRegistor>().All()
                .Where(x => x.Key == key)
                .FirstOrDefaultAsync()
                .ConfigureAwait(false);

            if (presetDb != null)
            {
                unitOfWork.GetRepository<PresetRegistor>().Delete(presetDb);

                await unitOfWork.SaveChangesAsync().ConfigureAwait(false);
            }

            this.RemoveCachePreset(preset.OwnerMnemonic);
        }


        public Preset GetDefaultPreset(IUnitOfWork unitOfWork, string mnemonic, string ownerMnemonic)
        {
            var preset = _presetFactory.CreatePreset(unitOfWork, mnemonic, ownerMnemonic);

            preset.Init(_uiFasade);

            return preset;
        }

        public T GetDefaultPreset<T>(IUnitOfWork unitOfWork, string ownerMnemonic) where T : Preset
        {
            return this.GetDefaultPreset(unitOfWork, _uiFasade.GetViewModelConfig(typeof(T)).Mnemonic, ownerMnemonic) as T;
        }

        public void SessionClear()
        {
            if (_session != null)
            {
                _session.Remove(PresetKey);
            }
        }
    }
}
