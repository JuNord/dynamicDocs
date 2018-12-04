using System;
using System.Linq;
using System.Text.RegularExpressions;
using System.Windows;
using System.Windows.Media;
using DynamicDocsWPF.HelperClasses;
using RestService;
using RestService.Model.Database;
using RestService.Model.Input;
using RestService.Model.Process;

namespace DynamicDocsWPF.Windows
{
    public partial class CreateProcessInstance : Window
    {
        private int _instanceId = -1;
        private readonly ProcessObject _processObject;
        private readonly NetworkHelper _networkHelper;
        private readonly ProcessStep _processStep;
        private readonly CustomEnumerable<Dialog> _dialogEnumerable;
        
        private static string MoTD
        {
            get
            {
                if (DateTime.Now.Hour < 12) return "Guten Morgen!";
                if (DateTime.Now.Hour < 17) return "Guten Tag!";
                if (DateTime.Now.Hour < 7 || DateTime.Now.Hour >= 17) return "Guten Abend!";
                return "Hallo";
            }
        }

        public CreateProcessInstance(ProcessObject processObject, NetworkHelper networkHelper)
        {
            InitializeComponent();
            _processObject = processObject;
            _networkHelper = networkHelper;
            _processStep = processObject.GetStepAtIndex(0);

            if (_processStep != null)
            {
                _dialogEnumerable = _processStep?.Dialogs;

                if (_dialogEnumerable != null)
                {
                    _dialogEnumerable?.MoveNext();
                    ViewHolder.Content = _dialogEnumerable.Current.GetStackPanel();
                }
            }
        }

        private Func<double> StringToCalculation(ProcessObject processObject, string calculation)
        {
            string[] split = new string[0];
            var op = "";
            if (calculation.Contains("+")) 
                op = "+";
            else if (calculation.Contains("-"))
                op = "-";
            else if (calculation.Contains("*"))
                op = "*";
            else if (calculation.Contains("/"))
                op = "/";
            else return null;

            split = calculation.Split(op[0]);
            
            
            if (split.Length != 2) return null;

            var numberRegex = new Regex("^\\d{1,*}$");
            var linkRegex = new Regex("^\\[(.*?)\\]$");
            var firstValue = 0.0;
            var secondValue = 0.0;

            if (numberRegex.IsMatch(split[0]))
                firstValue = double.Parse(split[0]);
            
            else if (linkRegex.IsMatch(split[0]))
            {
                var linkText = split[0].Substring(1, split[0].Length - 2);
                
                
            }

            if (numberRegex.IsMatch(split[1])) 
                secondValue = double.Parse(split[1]);

            switch (op)
            {
                case "+": return () => firstValue + secondValue;
                case "-": return () => firstValue - secondValue;
                case "*": return () => firstValue * secondValue;
                case "/": return () => firstValue / secondValue;
                default: return null;
            }
        }
        
        private Func<bool> StringToCondition(ProcessObject processObject, string condition)
        {
            var split = new string[0];
            var op = "";
            if (condition.Contains("<"))
                op = "<";
            else if (condition.Contains(">"))
                op = ">";
            else if (condition.Contains("<="))
                op = "<=";
            else if (condition.Contains(">="))
                op = ">=";
            else if (condition.Contains("=="))
                op = "==";
            else if (condition.Contains("!="))
                op = "!=";
            else return null;

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
        }

        private void Next_OnClick(object sender, RoutedEventArgs e)
        {
            if (_dialogEnumerable.Current?.Elements == null)
                return;
            if (_dialogEnumerable.Current.Elements.Any(baseInputElement => !IsInputValueValid(baseInputElement)))
                return;

            if (_dialogEnumerable.MoveNext())
            {
                ViewHolder.Content = _dialogEnumerable.Current.GetStackPanel();
            }
            else
            {
                if (!string.IsNullOrWhiteSpace(Subject.Text))
                {
                    var sendPopup = new InfoPopup(MessageBoxButton.YesNo,
                        "Sollen die eingegebenen Daten abgeschickt werden?");

                    if (sendPopup.ShowDialog() == true)
                        SendData();
                }
                else InfoBlock.Text = "Bitte geben Sie einen Betreff an.";
            }
        }
        
        private void Back_OnClick(object sender, RoutedEventArgs e)
        {
            if (_dialogEnumerable.MoveBack())
            {
                ViewHolder.Content = _dialogEnumerable.Current.GetStackPanel();
            }
        }

        private bool IsInputValueValid(BaseInputElement baseInputElement)
        {
            if (baseInputElement.FulfillsObligatoryConditions())
            {
                if (!baseInputElement.FulfillsProcessConditions())
                {
                    InfoBlock.Text = baseInputElement.ProcessErrorMsg;
                    baseInputElement.BaseControl.BorderBrush = new SolidColorBrush(Color.FromArgb(170, 255, 50, 50));
                    baseInputElement.BaseControl.BorderThickness = new Thickness(2);
                    return false;
                }

                if (!baseInputElement.FulfillsControlConditions())
                {
                    InfoBlock.Text = baseInputElement.ControlErrorMsg;
                    baseInputElement.BaseControl.BorderBrush = new SolidColorBrush(Color.FromArgb(170, 255, 50, 50));
                    baseInputElement.BaseControl.BorderThickness = new Thickness(2);
                    return false;
                }

                InfoBlock.Text = MoTD;
                baseInputElement.BaseControl.BorderBrush = new SolidColorBrush(Colors.Gray);
                baseInputElement.BaseControl.BorderThickness = new Thickness(1);
                return true;

            }
            else
            {
                InfoBlock.Text = "Bitte füllen Sie alle Muss Felder aus.";
                baseInputElement.BaseControl.BorderBrush = new SolidColorBrush(Color.FromArgb(170, 255, 50, 50));
                baseInputElement.BaseControl.BorderThickness = new Thickness(2);
                return false;
            }
        }

        private bool SendData()
        {
            var reply = _networkHelper.CreateProcessInstance(_processObject.Name, _networkHelper.User.Email, Subject.Text);

            _instanceId = reply.InstanceId;

            if (reply.UploadResult == UploadResult.SUCCESS)
            {
                for (var i = 0; i < _processStep.DialogCount; i++)
                {
                    var dialog = _processStep.GetDialogAtIndex(i);
                    for (var j = 0; j < dialog.ElementCount; j++)
                    {
                        var element = dialog.GetElementAtIndex(j);
                        var entry = new Entry()
                        {
                            InstanceId = _instanceId,
                            FieldName = element.Name,
                            Data = element.GetFormattedValue()
                        };

                        _networkHelper.CreateEntry(entry);
                    }
                }

                return true;
            }

            if (reply.UploadResult == UploadResult.FAILED_OTHER)
            {
                new InfoPopup(MessageBoxButton.OK,
                        "Ups, da ist wohl etwas schief gelaufen. Bitte wenden Sie sich an einen Administrator")
                    .ShowDialog();
            }

            return false;
        }
    }
}
