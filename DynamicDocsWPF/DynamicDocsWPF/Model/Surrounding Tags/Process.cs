using System.Collections.Generic;
using DynamicDocsWPF.Model.InputElements;

namespace DynamicDocsWPF.Model.Surrounding_Tags
{
    public class Process
    {
        private readonly List<ProcessStep> _steps;

        public Process(List<ProcessStep> steps)
        {
            _steps = steps;
        }

        public void AddStep(ProcessStep step)
        {
            if(step != null)
                _steps.Add(step);
        }

        public ProcessStep GetStepAtIndex(int index) => _steps?[index];
    }
}