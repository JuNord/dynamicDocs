using System.Collections.Generic;
using DynamicDocsWPF.Model.Base;

namespace DynamicDocsWPF.Model.Process
{
    public class Process : NamedTag
    {
        private readonly List<ProcessStep> _steps;
        private readonly List<ArchivePermissionElement> _permissions;
        
        public Process(string name, string description) : base(null, name, description)
        {
            _steps = new List<ProcessStep>();
            _permissions = new List<ArchivePermissionElement>();
        }

        public void AddStep(ProcessStep step)
        {
            if(step != null)
                _steps.Add(step);
        }

        public void AddPermission(ArchivePermissionElement element)
        {
            if(element != null)
                _permissions.Add(element);
        }

        public int ProcessStepCount => _steps?.Count ?? 0;
        
        public ProcessStep GetStepAtIndex(int index) => _steps?[index];

        public string GetElementValue(string name)
        {
            foreach (var processStep in _steps)
            {
                for (var i = 0; i < processStep.DialogCount; i++)
                {
                    var dialog = processStep.GetDialogAtIndex(i);
                    for (var j = 0; j < dialog.ElementCount; j++)
                    {
                        var element = dialog.GetElementByName(name);

                        if (element != null) return element.GetFormattedValue();
                    }
                }
            }

            return null;
        }
    }
}