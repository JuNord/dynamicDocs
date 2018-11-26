using System.Collections.Generic;
using DynamicDocsWPF.Model.Base_Classes;

namespace DynamicDocsWPF.Model.Surrounding_Tags
{
    public class ProcessStep : NamedTag
    {
        private readonly List<Dialog> _dialogs;

        public ProcessStep(Tag parent) : base(parent)
        {
            _dialogs = new List<Dialog>();
        }

        public void AddDialog(Dialog dialog)
        {
            _dialogs.Add(dialog);
        }

        public int DialogCount => _dialogs?.Count??0;
        
        public Dialog GetDialogAtIndex(int index) => _dialogs?[index];
    }
}