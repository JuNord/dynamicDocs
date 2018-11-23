using System;
using System.Windows.Controls;

namespace DynamicDocsWPF.Model.Base_Classes
{
    public abstract class BaseInputElement : NamedTag
    {
        public Control BaseControl { get; }
        public bool Obligatory { get; private set; }
        public string ProcessErrorMsg { get; set; }
        public string ControlErrorMsg { get; protected set; }
        
        public Func<bool> IsValidForProcess { private get; set; }
        public Func<bool> FulfillsObligatory { private get; set; }

        
        protected BaseInputElement(Tag parent, bool obligatory, Control control) : base(parent)
        {
            BaseControl = control;
            Obligatory = obligatory;
        }
        
        /// <summary>
        /// Returns whether the value of the underlying UI Control, fulfills certain InputElement specific conditions.
        /// e.g. If the TextBox of a NumberInputBox only includes digits
        /// </summary>
        /// <returns></returns>
        public abstract bool CheckValidForControl();
        
        
        
        /// <summary>
        /// Clears the underlying UI Control, according to its type
        /// </summary>
        public abstract void Clear();
        
        /// <summary>
        /// Returns whether a controls content is fulfilling additional conditions defined in the XML
        /// </summary>
        /// <returns></returns>
        public bool CheckValidForProcess() => IsValidForProcess?.Invoke() ?? true;
        
        /// <summary>
        /// Returns whether a control fulfills any conditions about its obligatority
        /// </summary>
        /// <returns></returns>
        public bool CheckObligatory() => FulfillsObligatory?.Invoke() ?? true;
    }
}