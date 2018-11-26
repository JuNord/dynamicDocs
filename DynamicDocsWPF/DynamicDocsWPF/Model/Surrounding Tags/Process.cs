using System;
using System.Collections.Generic;
using DynamicDocsWPF.Model.Base_Classes;
using DynamicDocsWPF.Model.InputElements;

namespace DynamicDocsWPF.Model.Surrounding_Tags
{
    public class Process : NamedTag
    {
        private readonly List<ProcessStep> _steps;

        public Process() : base(null)
        {
            _steps = new List<ProcessStep>();
        }

        public void AddStep(ProcessStep step)
        {
            if(step != null)
                _steps.Add(step);
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

                        if (element != null) return element.ValueToString();
                    }
                }
            }

            return null;
        }
    }
}