using System;
using System.Runtime.CompilerServices;
using System.Text.RegularExpressions;
using System.Windows;
using System.Windows.Media;
using DynamicDocsWPF.HelperClasses;
using RestService;
using RestService.Model.Database;
using RestService.Model.Process;

namespace DynamicDocsWPF.Windows
{
    public partial class CreateProcessInstance : Window
    {
        private int _currentDialog;
        private int _instanceId = -1;
        private Process _process;
        private readonly NetworkHelper _networkHelper;
        private ProcessStep _processStep;
        
        public CreateProcessInstance(Process process, NetworkHelper networkHelper)
        {
            InitializeComponent();
            _process = process;
            _networkHelper = networkHelper;
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
            string[] split = new string[0];
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

            if (_currentDialog+1 < _processStep.DialogCount)
            {
                _currentDialog++;
                
                ViewCreator.FillViewHolder(ViewHolder, _processStep.GetDialogAtIndex(_currentDialog));
            }
            else
            {
                var sendPopup = new InfoPopup(MessageBoxButton.YesNo, "Sollen die eingegebenen Daten abgeschickt werden?");

                sendPopup.ShowDialog();

                if (sendPopup.DialogResult == true)
                {
                    SendData();
                }
                else
                {
                    var ensurePopup = new InfoPopup(MessageBoxButton.YesNo, "Sind sie sicher ?");

                    ensurePopup.ShowDialog();
                    
                    if (ensurePopup.DialogResult == false) SendData();
                }

                Close();
            }

            if (_currentDialog == _processStep.DialogCount - 1)
                BtnNext.Content = "Senden";
        }

        private void SendData()
        {
            var reply = _networkHelper.CreateProcessInstance(_process.Name, _networkHelper.User.Email);

            _instanceId = reply.InstanceId;

            if (reply.UploadResult == UploadResult.SUCCESS)
            {
                for (var i = 0; i < _processStep.DialogCount; i++)
                {
                    var dialog = _processStep.GetDialogAtIndex(i);
                    for (var j = 0; j < dialog.ElementCount; j++)
                    {
                        var element = dialog.GetElementAtIndex(0);
                        var entry = new Entry()
                        {
                            Process_ID = _instanceId,
                            PermissionLevel = 1,
                            FieldName = element.Name,
                            Data = element.GetFormattedValue(),
                            DataType = element.DataType.ToString()
                        };

                        _networkHelper.CreateEntry(entry);
                    }
                }
            }
        }
    }
}
