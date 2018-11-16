using System.Windows.Controls;

namespace DynamicDocsWPF.Model.Base_Classes
{
    public abstract class InputElement<TInput, TValue> : Unnamed where TInput : Control 
    {
        public TValue Value { get; set; }
        public string Check { get; set; }
        public TInput UiElement { get; set; }
        
        
    }
}