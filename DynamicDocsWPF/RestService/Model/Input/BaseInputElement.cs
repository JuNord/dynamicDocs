using System;
using System.Windows.Controls;
using RestService.Model.Base;

namespace RestService.Model.Input
{
    public abstract class BaseInputElement : NamedTag
    {
        protected BaseInputElement(Tag parent, string name, string description, bool obligatory, string calculation,
            Control control) :
            base(parent, name, description)
        {
            BaseControl = control;
            Obligatory = obligatory;
            Calculation = calculation;
            if (!string.IsNullOrWhiteSpace(calculation)) BaseControl.IsEnabled = false;
            SetStartValue();
        }

        public Control BaseControl { get; }
        public bool Obligatory { get; }
        public string ProcessErrorMsg { get; set; }
        public string ControlErrorMsg { get; protected set; }
        public string Calculation { get; }

        public DataType DataType { get; set; }
        public Func<bool> ProcessValidityCheck { private get; set; }
        protected Func<bool> ControlValidityCheck { private get; set; }

        /// <summary>
        ///     Clears the underlying UI Control, according to its type
        /// </summary>
        public abstract void Clear();

        /// <summary>
        ///     Fills the underlying UI Control with startvalues
        /// </summary>
        public abstract void SetStartValue();

        /// <summary>
        ///     Returns whether a controls content is fulfilling additional conditions defined in the XML
        /// </summary>
        /// <returns></returns>
        public bool FulfillsProcessConditions()
        {
            return ProcessValidityCheck?.Invoke() ?? true;
        }

        /// <summary>
        ///     Returns whether the value of the underlying UI Control, fulfills certain InputElement specific conditions.
        ///     e.g. If the TextBox of a NumberInputBox only includes digits
        /// </summary>
        /// <returns></returns>
        public bool FulfillsControlConditions()
        {
            return ControlValidityCheck?.Invoke() ?? true;
        }

        /// <summary>
        ///     Returns whether a control fulfills any conditions about its obligatority
        /// </summary>
        /// <returns></returns>
        public bool FulfillsObligatoryConditions()
        {
            return !Obligatory || ObligatoryCheck();
        }

        /// <summary>
        ///     Specifies how the underlying UI Controls value can be Parsed from a string
        /// </summary>
        /// <param name="value"></param>
        public abstract void SetValueFromString(string value);

        /// <summary>
        /// </summary>
        /// <param name="value1"></param>
        /// <param name="value2"></param>
        /// <param name="operand"></param>
        /// <returns></returns>
        public abstract bool Calculate(string value1, string value2, char operand);

        /// <summary>
        ///     Returns the underlying UI Controls value formatted as a string
        /// </summary>
        /// <returns></returns>
        public abstract string GetFormattedValue();

        /// <summary>
        /// Sets the enabled Value of the BaseControl
        /// </summary>
        /// <param name="enabled"></param>
        public void SetEnabled(bool enabled)
        {
            BaseControl.IsEnabled = enabled;
        }

        public abstract bool ObligatoryCheck();
    }
}