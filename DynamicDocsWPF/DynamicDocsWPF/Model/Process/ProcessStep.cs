using System.Collections.Generic;
using DynamicDocsWPF.Model.Base;
using DynamicDocsWPF.Model.InputElements;

namespace DynamicDocsWPF.Model.Process
{
    public class ProcessStep : NamedTag
    {
        public string Target { get; set; }
        private readonly List<Dialog> _dialogs;
        
        public ProcessStep(Tag parent, string name, string description, string target) : base(parent, name, description)
        {
            _dialogs = new List<Dialog>();
            Target = target;
        }

        public void AddDialog(Dialog dialog)
        {
            _dialogs.Add(dialog);
        }

        public int DialogCount => _dialogs?.Count??0;
        
        public Dialog GetDialogAtIndex(int index) => _dialogs?[index];
    }
}