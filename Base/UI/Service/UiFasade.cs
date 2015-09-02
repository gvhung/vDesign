using System;
using System.Collections.Generic;

namespace Base.UI.Service
{
    public class UiFasade: IUiFasade
    {
        private readonly IViewModelConfigService _viewModelConfigService;
        private readonly IListViewService _listViewService;
        private readonly IDetailViewService _detailViewService;

        public UiFasade(IViewModelConfigService viewModelConfigService, IListViewService listViewService, IDetailViewService detailViewService)
        {
            _viewModelConfigService = viewModelConfigService;
            _listViewService = listViewService;
            _detailViewService = detailViewService;
        }

        public ViewModelConfig GetViewModelConfig(string mnemonic)
        {
            return _viewModelConfigService.Get(mnemonic);
        }

        public ViewModelConfig GetViewModelConfig(Type type)
        {
            return _viewModelConfigService.Get(type);
        }

        public List<ColumnViewModel> GetColumns(string mnemonic)
        {
            return _listViewService.GetColumns(mnemonic);
        }

        public List<ColumnViewModel> GetColumns(Type type)
        {
            return _listViewService.GetColumns(type);
        }
        
        public List<ColumnViewModel> GetColumns(ViewModelConfig viewModelConfig)
        {
            return _listViewService.GetColumns(viewModelConfig);
        }

        public List<EditorViewModel> GetEditors(string mnemonic)
        {
            return _detailViewService.GetEditors(mnemonic);
        }

        public List<EditorViewModel> GetEditors(Type type)
        {
            return _detailViewService.GetEditors(type);
        }

        public List<EditorViewModel> GetEditors(ViewModelConfig config)
        {
            return _detailViewService.GetEditors(config);
        }
    }
}
