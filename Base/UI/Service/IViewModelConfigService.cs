using Base.Service;
using System;
using System.Collections.Generic;

namespace Base.UI
{
    public interface IViewModelConfigService : IService
    {
        List<ViewModelConfig> GetAll();
        ViewModelConfig Get(string mnemonic);
        ViewModelConfig Get(Type type);
        Dictionary<Type, string> GetAllObjects();
        Dictionary<Type, string> GetAllObjects(Func<ViewModelConfig, bool> predicate);
        Dictionary<string, string> GetAllMnemonics(Func<ViewModelConfig, bool> predicate);
    }
}
