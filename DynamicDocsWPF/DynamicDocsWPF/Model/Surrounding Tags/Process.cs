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

        public ProcessStep GetStepAtIndex(int index) => _steps?[index];
    }
}