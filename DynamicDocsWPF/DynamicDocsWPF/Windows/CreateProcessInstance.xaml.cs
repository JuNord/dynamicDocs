using System;
using System.Text.RegularExpressions;
using System.Windows;
using System.Windows.Media;
using DynamicDocsWPF.HelperClasses;
using RestService.Model.Process;

namespace DynamicDocsWPF.Windows
{
    public partial class CreateProcessInstance : Window
    {
        private int _currentDialog;
        private Process _process;
        private ProcessStep _processStep;
        
        public CreateProcessInstance(Process process)
        {
            InitializeComponent();
            _process = process;
            _processStep = process.GetStepAtIndex(0);
            if (_processStep != null)
            {
                HandleProcessStep(_processStep);
            }
            
        }
        
        private string Message
        {
            get
            {
                if (DateTime.Now.Hour < 12) return "Guten Morgen!";
                if (DateTime.Now.Hour < 17) return "Guten Tag!";
                if (DateTime.Now.Hour < 7 || DateTime.Now.Hour >= 17) return "Guten Abend!";
                return "Hallo";
            }
        }
        
        private void HandleProcessStep(ProcessStep processStep)
        {
            const int currentDialog = 0;
            if (processStep.DialogCount > 0)
                ViewCreator.FillViewHolder(ViewHolder, processStep.GetDialogAtIndex(currentDialog));
        }
        
        private Func<bool> StringToCondition(Process process, string condition)
        {
            string[] split;
            var op = "";
            if (condition.Contains("<"))
            {
                op = "<";
                split = condition.Split(op[0]);
            }
            else if (condition.Contains(">"))
            {
                op = ">";
                split = condition.Split(op[0]);
            }
            else if (condition.Contains("<="))
            {
                op = "<=";
                split = condition.Split(op[0]);
            }
            else if (condition.Contains(">="))
            {
                op = ">=";
                split = condition.Split(op[0]);
            }
            else if (condition.Contains("=="))
            {
                op = "==";
                split = condition.Split(op[0]);
            }
            else if (condition.Contains("!="))
            {
                op = "!=";
                split = condition.Split(op[0]);
            }
            else
            {
                split = new string[0];
            }

            if (split.Length != 2) return null;

            var numberRegex = new Regex("^\\d{1,*}$");
            var linkRegex = new Regex("^\\[(.*?)\\]$");
            var firstValue = 0.0;
            var secondValue = 0.0;

            if (numberRegex.IsMatch(split[0]))
            {
                firstValue = double.Parse(split[0]);
            }
            else if (linkRegex.IsMatch(split[0]))
            {
                var linkText = split[0].Substring(1, split[0].Length - 2);
            }

            if (numberRegex.IsMatch(split[1])) secondValue = double.Parse(split[1]);

            switch (op)
            {
                case "<": return () => firstValue < secondValue;
                case ">": return () => firstValue > secondValue;
                case "<=": return () => firstValue <= secondValue;
                case ">=": return () => firstValue >= secondValue;
                case "==": return () => Math.Abs(firstValue - secondValue) < 0.000000001;
                case "!=": return () => Math.Abs(firstValue - secondValue) > 0.000000001;
                default: return null;
            }

            return null;
        }

        private void CreateInstance_Btn_Next_OnClick(object sender, RoutedEventArgs e)
        {
            for (var i = 0; i < _processStep.GetDialogAtIndex(_currentDialog)?.ElementCount; i++)
            {
                var element = _processStep.GetDialogAtIndex(_currentDialog).GetElementAtIndex(i);
                if (element.IsValidForObligatory())
                {
                    if (!element.IsValidForProcess())
                    {
                        InfoBlock.Text = element.ProcessErrorMsg;
                        element.BaseControl.BorderBrush = new SolidColorBrush(Color.FromArgb(170, 255, 50, 50));
                        element.BaseControl.BorderThickness = new Thickness(2);
                        return;
                    }

                    if (!element.IsValidForControl())
                    {
                        InfoBlock.Text = element.ControlErrorMsg;
                        element.BaseControl.BorderBrush = new SolidColorBrush(Color.FromArgb(170, 255, 50, 50));
                        element.BaseControl.BorderThickness = new Thickness(2);
                        return;
                    }

                    InfoBlock.Text = Message;
                    element.BaseControl.BorderBrush = new SolidColorBrush(Colors.Gray);
                    element.BaseControl.BorderThickness = new Thickness(1);
                }
                else
                {
                    InfoBlock.Text = "Bitte füllen Sie alle Muss Felder aus.";
                    element.BaseControl.BorderBrush = new SolidColorBrush(Color.FromArgb(170, 255, 50, 50));
                    element.BaseControl.BorderThickness = new Thickness(2);
                    return;
                }
            }

            _currentDialog++;
            ViewCreator.FillViewHolder(ViewHolder, _processStep.GetDialogAtIndex(_currentDialog));
        }
    }
}
