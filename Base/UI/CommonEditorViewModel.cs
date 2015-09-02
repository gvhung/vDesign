using Framework;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Base.UI
{
    public class CommonEditorViewModel
    {
        private List<TabVm> _tabs;
        private int _ver = 0;

        public CommonEditorViewModel(ViewModelConfig viewModelConfig, List<EditorViewModel> editors)
        {
            ViewModelConfig = viewModelConfig;
            Editors = editors;
        }

        public ViewModelConfig ViewModelConfig { get; private set; }

        public List<EditorViewModel> Editors { get; private set; }

        public List<TabVm> Tabs
        {
            get
            {
                return _tabs ?? (_tabs = Editors
                    .Where(e => e.Visible)
                    .GroupBy(x => x.TabName).OrderBy(x => x.Key)
                    .Select(x => new TabVm(x.Key, editors: x)).ToList());
            }
        }

        public void ApplySetting(IDetailViewSetting detailViewSetting)
        {
            _ver++;
            
            detailViewSetting.Apply(this);

            var editors = new List<EditorViewModel>();

            foreach (var editor in Editors.Select(ed=> (_ver == 0 ? ObjectHelper.CreateAndCopyObject<EditorViewModel>(ed) : ed)))
            {
                detailViewSetting.Apply(editor);
                editors.Add(editor);
            }

            _tabs = null;

            Editors = editors;
        }
    }

    public class TabVm
    {
        public TabVm(string tabName, IGrouping<string, EditorViewModel> editors)
        {
            TabID = Guid.NewGuid().ToString("N");
            TabName = tabName;
            Editors = editors;
        }

        public string TabID { get; private set; }
        public string TabName { get;  private set; }
        public IGrouping<string, EditorViewModel> Editors { get; private set; }
    }
}