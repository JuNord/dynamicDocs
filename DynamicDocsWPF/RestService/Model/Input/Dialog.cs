using System.Collections.Generic;
using RestService.Model.Base;

namespace RestService.Model.Input
{
    public class Dialog : Tag
    {
        private readonly List<BaseInputElement> _elements;

        public IEnumerable<BaseInputElement> Elements => _elements;
        
        public Dialog(Tag parent) : base(parent)
        {
            _elements = new List<BaseInputElement>();
        }

        public int ElementCount => _elements?.Count ?? 0;

        public void AddElement(BaseInputElement element)
        {
            _elements.Add(element);
        }

        public BaseInputElement GetElementAtIndex(int index)
        {
            return _elements?[index];
        }

        public BaseInputElement GetElementByName(string name)
        {
            foreach (var element in _elements)
                if (element.Name.Equals(name))
                    return element;

            return null;
        }
    }
}