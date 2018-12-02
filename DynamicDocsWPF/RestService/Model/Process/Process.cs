using System.Collections.Generic;
using RestService.Model.Base;

namespace RestService.Model.Process
{
    public class Process : NamedTag
    {
        private readonly List<ArchivePermissionElement> _permissions;
        private readonly List<ProcessStep> _steps;

        public IEnumerable<ProcessStep> Steps => _steps;

        public Process(string name, string description) : base(null, name, description)
        {
            _steps = new List<ProcessStep>();
            _permissions = new List<ArchivePermissionElement>();
        }

        public int CurrentStep { get; set; }

        public int ProcessStepCount => _steps?.Count ?? 0;

        public void AddStep(ProcessStep step)
        {
            if (step != null)
                _steps.Add(step);
        }

        public void AddPermission(ArchivePermissionElement element)
        {
            if (element != null)
                _permissions.Add(element);
        }

        public ProcessStep GetStepAtIndex(int index)
        {
            return _steps?[index];
        }
        

        public string GetElementValue(string name)
        {
            foreach (var processStep in _steps)
                for (var i = 0; i < processStep.DialogCount; i++)
                {
                    var dialog = processStep.GetDialogAtIndex(i);
                    for (var j = 0; j < dialog.ElementCount; j++)
                    {
                        var element = dialog.GetElementByName(name);

                        if (element != null) return element.GetFormattedValue();
                    }
                }

            return null;
        }
    }
}