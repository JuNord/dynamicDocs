using System.Collections.Generic;
using System.Linq;
using System.Windows.Controls;
using System.Windows.Documents;
using DynamicDocsWPF.Model.Base_Classes;

namespace DynamicDocsWPF.Model.Surrounding_Tags
{
    public class Dialog : Tag
    {
        private readonly List<BaseInputElement> _elements;

        public Dialog(Tag parent) : base(parent)
        {
            _elements = new List<BaseInputElement>();
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

        public int GetElementCount() => _elements?.Count ?? 0;
    }
}