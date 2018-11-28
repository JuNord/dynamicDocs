using System;
using System.Text.RegularExpressions;
using System.Windows;
using System.Windows.Media;
using DynamicDocsWPF.HelperClasses;
using RestService;
using RestService.Model.Database;
using RestService.Model.Process;

namespace DynamicDocsWPF
{
    /// <summary>
    ///     Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow
    {
        private int _currentDialog;
        private Process _process;
        private ProcessStep _processStep;

        public MainWindow()
        {
            InitializeComponent();
            var login = new Login();
            login.ShowDialog();
            var helper = new NetworkHelper("http://localhost:8000/Service", new User(login.Email, login.Password));
            var result = helper.UploadProcessTemplate("", true);
            HandleUploadResult(result);
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

        private void HandleUploadResult(UploadResult result)
        {
            switch (result)
            {
                case UploadResult.FAILED_FILEEXISTS:
                    MessageBox.Show(this, "Filename already taken.");
                    break;
                case UploadResult.FAILED_ID_EXISTS:
                    MessageBox.Show(this, "ID already taken.");
                    break;
                case UploadResult.FAILED_OTHER:
                    MessageBox.Show(this, "Something went wrong.");
                    break;
                case UploadResult.INVALID_LOGIN:
                    MessageBox.Show(this, "Username or password was wrong.");
                    break;
                case UploadResult.NO_PERMISSION:
                    MessageBox.Show(this, "You are not permitted.");
                    break;
            }
        }

        private void HandleProcessStep(ProcessStep processStep)
        {
            const int currentDialog = 0;
            if (processStep.DialogCount > 0)
                ViewCreator.FillViewHolder(ViewHolder, processStep.GetDialogAtIndex(currentDialog));
        }


        private void ButtonBase_OnClick(object sender, RoutedEventArgs e)
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
    }
}