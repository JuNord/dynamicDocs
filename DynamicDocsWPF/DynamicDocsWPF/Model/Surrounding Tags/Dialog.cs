using System.Collections.Generic;
using System.Windows.Controls;
using System.Windows.Documents;
using DynamicDocsWPF.Model.Base_Classes;

namespace DynamicDocsWPF.Model.Surrounding_Tags
{
    public class Dialog
    {
        private readonly List<BaseInputElement> _elements;

        public Dialog(List<BaseInputElement> elements)
        {
            _elements = elements;
        }

        public void AddElement(BaseInputElement element)
        {
            _elements.Add(element);
        }

        public BaseInputElement GetElementAtIndex(int index) => _elements?[index];

        public BaseInputElement GetElementByName(string name)
        {
            foreach (var element in _elements)
            {
                if (element.Name.Equals(name)) return element;
            }

            return null;
        }
    }
}