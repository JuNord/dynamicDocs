using System.Collections.Generic;

namespace DynamicDocsWPF.Model.Surrounding_Tags
{
    public class ProcessStep
    {
        private readonly List<Dialog> _dialogs;

        public ProcessStep(List<Dialog> dialogs)
        {
            _dialogs = dialogs;
        }

        public void AddDialog(Dialog dialog)
        {
            _dialogs.Add(dialog);
        }

        public Dialog GetDialogAtIndex(int index) => _dialogs?[index];
    }
}