using Base.Service;
using System;
using System.Collections.Generic;

namespace Base.UI
{
    public interface IViewModelConfigLoader : IService
    {
        List<ViewModelConfig> Load(Func<Type, ViewModelConfig> defViewModelConfig);
    }
}
