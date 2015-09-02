using Base.DAL;
using System;
using System.Collections.Generic;

namespace Base.UI.Service
{
    public interface IDetailViewService
    {
        EditorViewModel GetEditorViewModel(string mnemonic, string member);
        List<EditorViewModel> GetEditors(string mnemonic);
        List<EditorViewModel> GetEditors(Type type);
        List<EditorViewModel> GetEditors(ViewModelConfig config);
        CommonEditorViewModel GetCommonEditor(string mnemonic);
        CommonEditorViewModel GetCommonEditor(IUnitOfWork unitOfWork, string mnemonic, BaseObject obj);
    }
}
