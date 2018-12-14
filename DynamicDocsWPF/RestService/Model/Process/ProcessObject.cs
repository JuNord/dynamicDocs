using System.Collections.Generic;
using RestService.Model.Base;

namespace RestService.Model.Process
{
    public class ProcessObject : NamedTag
    {
        private readonly List<ArchivePermissionElement> _archivePermissions;
        private readonly List<ProcessStep> _steps;

        public CustomEnumerable<ProcessStep> Steps { get; }
        public CustomEnumerable<ArchivePermissionElement> ArchivePermissions { get; }

        public ProcessObject(string name, string description) : base(null, name, description)
        {
            _steps = new List<ProcessStep>();
            Steps = new CustomEnumerable<ProcessStep>(_steps);
            _archivePermissions = new List<ArchivePermissionElement>();
            ArchivePermissions = new CustomEnumerable<ArchivePermissionElement>(_archivePermissions);
        }

        public int CurrentStep { get; set; }

        public int StepCount => _steps?.Count ?? 0;

        public void AddStep(ProcessStep step)
        {
            if (step != null)
                _steps.Add(step);
        }

        public void AddPermission(ArchivePermissionElement element)
        {
            if (element != null)
                _archivePermissions.Add(element);
        }

        public ProcessStep GetStepAtIndex(int index)
        {
            if (index < _steps.Count)
                return _steps?[index];
            else return null;
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