using System;
using System.Collections.Generic;

namespace Base.UI.Service
{
    public interface IUiFasade
    {
        ViewModelConfig GetViewModelConfig(string mnemonic);
        ViewModelConfig GetViewModelConfig(Type type);

        List<ColumnViewModel> GetColumns(string mnemonic);
        List<ColumnViewModel> GetColumns(Type type);
        List<ColumnViewModel> GetColumns(ViewModelConfig viewModelConfig);

        List<EditorViewModel> GetEditors(string mnemonic);
        List<EditorViewModel> GetEditors(Type type);
        List<EditorViewModel> GetEditors(ViewModelConfig config);
    }
}
