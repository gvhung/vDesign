using Base.UI.Service;
using Newtonsoft.Json;
using System;

namespace Base.UI
{
    [Serializable]
    public abstract class Preset : BaseObject
    {
        [NonSerialized]
        private ViewModelConfig _config;

        public string Mnemonic { get; set; }
        public string OwnerMnemonic { get; set; }
        
        [JsonIgnore]
        public ViewModelConfig OwnerConfig { get { return _config; } }
        [JsonIgnore]
        public bool Modified { get; set; }
        [JsonIgnore]
        public bool IsInit { get; private set; }

        public virtual Preset Init(IUiFasade uiFasade)
        {
            _config = uiFasade.GetViewModelConfig(OwnerMnemonic);
            IsInit = true;

            return this;
        }
    }
}
