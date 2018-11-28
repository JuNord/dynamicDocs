using System;
using System.Windows.Controls;
using RestService.Model.Base;

namespace RestService.Model.Input
{
    public abstract class BaseInputElement : NamedTag
    {
        protected BaseInputElement(Tag parent, string name, string description, bool obligatory, Control control) :
            base(parent, name, description)
        {
            BaseControl = control;
            Obligatory = obligatory;
            Fill();
        }

        public Control BaseControl { get; }
        public bool Obligatory { get; }
        public string ProcessErrorMsg { get; set; }
        public string ControlErrorMsg { get; protected set; }

        public Func<string> GetFormattedValue { get; set; }
        public Func<bool> ProcessValidityCheck { private get; set; }
        protected Func<bool> ControlValidityCheck { private get; set; }
        protected Func<bool> ObligatoryCheck { private get; set; }

        /// <summary>
        ///     Clears the underlying UI Control, according to its type
        /// </summary>
        public abstract void Clear();

        /// <summary>
        ///     Fills the underlying UI Control with startvalues
        /// </summary>
        public abstract void Fill();

        /// <summary>
        ///     Returns whether a controls content is fulfilling additional conditions defined in the XML
        /// </summary>
        /// <returns></returns>
        public bool IsValidForProcess()
        {
            return ProcessValidityCheck?.Invoke() ?? true;
        }

        /// <summary>
        ///     Returns whether the value of the underlying UI Control, fulfills certain InputElement specific conditions.
        ///     e.g. If the TextBox of a NumberInputBox only includes digits
        /// </summary>
        /// <returns></returns>
        public bool IsValidForControl()
        {
            return ControlValidityCheck?.Invoke() ?? true;
        }

        /// <summary>
        ///     Returns whether a control fulfills any conditions about its obligatority
        /// </summary>
        /// <returns></returns>
        public bool IsValidForObligatory()
        {
            return !Obligatory || (ObligatoryCheck?.Invoke() ?? true);
        }
    }
}