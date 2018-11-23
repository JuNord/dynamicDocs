using System.Collections.Generic;
using DynamicDocsWPF.Model.Base_Classes;

namespace DynamicDocsWPF.Model.Surrounding_Tags
{
    public class ProcessStep : Tag
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

        public Dialog GetDialogAtIndex(int index) => _dialogs?[index];
    }
}