using System.Collections.Generic;
using System.Text.RegularExpressions;
using System.Windows.Controls;
using RestService.Model.Base;
using RestService.Model.Process;

namespace RestService.Model.Input
{
    public class Dialog : NamedTag
    {
        private readonly List<BaseInputElement> _elements;
        private StackPanel _stackPanel;

        public Dialog(Tag parent, string name, string description) : base(parent, name , description)
        {
            _elements = new List<BaseInputElement>();
            Elements = new CustomEnumerable<BaseInputElement>(_elements);
        }

        public CustomEnumerable<BaseInputElement> Elements { get; }

        public int ElementCount => _elements?.Count ?? 0;

        public StackPanel GetStackPanel()
        {
            if (_stackPanel == null) _stackPanel = StackPanelFactory.CreatePanel(this);

            return _stackPanel;
        }

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

        public void PerformCalculations()
        {
            foreach (var element in _elements)
                if (!string.IsNullOrWhiteSpace(element.Calculation))
                {
                    var calculation = element.Calculation;
                    var split = new string[0];
                    var op = ' ';
                    if (calculation.Contains("+"))
                        op = '+';
                    else if (calculation.Contains("-"))
                        op = '-';
                    else if (calculation.Contains("*"))
                        op = '*';
                    else if (calculation.Contains("/"))
                        op = '/';
                    else return;

                    split = calculation.Split(op);


                    if (split.Length != 2) return;

                    var numberRegex = new Regex("^\\d{1,*}$");
                    var linkRegex = new Regex("^\\[(.*?)\\]$");
                    var firstValue = "";
                    var secondValue = "";

                    if (numberRegex.IsMatch(split[0]))
                    {
                        firstValue = split[0];
                    }
                    else if (linkRegex.IsMatch(split[0]))
                    {
                        var linkText = split[0].Substring(1, split[0].Length - 2);

                        var processObject = (ProcessObject) ((ProcessStep) Parent).Parent;
                        firstValue = processObject.GetElementValue(linkText);
                    }

                    if (numberRegex.IsMatch(split[1]))
                    {
                        secondValue = split[1];
                    }
                    else if (linkRegex.IsMatch(split[1]))
                    {
                        var linkText = split[1].Substring(1, split[1].Length - 2);

                        var processObject = (ProcessObject) ((ProcessStep) Parent).Parent;
                        secondValue = processObject.GetElementValue(linkText);
                    }

                    element.Calculate(firstValue, secondValue, op);
                }
        }
    }
}