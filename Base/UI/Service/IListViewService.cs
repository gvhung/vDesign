using System;
using System.Collections.Generic;

namespace Base.UI.Service
{
    public interface IListViewService
    {
        List<ColumnViewModel> GetColumns(string mnemonic);
        List<ColumnViewModel> GetColumns(Type type);
        List<ColumnViewModel> GetColumns(ViewModelConfig viewModelConfig);
    }
}
