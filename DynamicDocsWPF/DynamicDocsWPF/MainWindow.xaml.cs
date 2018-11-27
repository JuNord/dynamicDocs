using System;
using System.Runtime.Remoting.Messaging;
using System.Text.RegularExpressions;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Xml;
using DynamicDocsWPF.HelperClasses;
using DynamicDocsWPF.Model;
using DynamicDocsWPF.Model.InputElements;
using DynamicDocsWPF.Model.Process;
using RestService;
using WebServer.Model;

namespace DynamicDocsWPF
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow
    {
        private Process _process;
        private ProcessStep _processStep;
        private int _currentDialog;

        private string Message
        {
            get
            {
                if (DateTime.Now.Hour < 12) return "Guten Morgen!";
                else if (DateTime.Now.Hour < 17) return "Guten Tag!";
                else if (DateTime.Now.Hour < 7 || DateTime.Now.Hour >= 17) return "Guten Abend!";
                else return "Hallo";
            }
        }
        
        public MainWindow()
        {
            InitializeComponent();
            /*_process = XMLHelper.ReadXML(@"C:\Users\Sebastian.Bauer\RiderProjects\dynamicDocs\DynamicDocsWPF\XmlProcessor\XMLFile1.xml");
            _process.CurrentStep = 1;
            var overview = new ValidationOverview(_process);
            overview.ShowDialog();*/
            
            var helper = new NetworkHelper("http://localhost:8000/Service");
            helper.PostProcessTemplate(new ProcessTemplate()
            {
                Process_ID = "blaprocess",
                FilePath = "C:/bla",
                Description = "VIEL VIEL BLA"
            });
            
        }

        private void HandleProcessStep(ProcessStep processStep)
        {
            const int currentDialog = 0;
            if (processStep.DialogCount > 0)
            {
                ViewCreator.FillViewHolder(ViewHolder, processStep.GetDialogAtIndex(currentDialog));
            }
            
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
                        element.BaseControl.BorderBrush =  new SolidColorBrush(Color.FromArgb(170,255,50,50));
                        element.BaseControl.BorderThickness = new Thickness(2);   
                        return;
                    }
                    else if (!element.IsValidForControl())
                    {
                        InfoBlock.Text = element.ControlErrorMsg;
                        element.BaseControl.BorderBrush =  new SolidColorBrush(Color.FromArgb(170,255,50,50));
                        element.BaseControl.BorderThickness = new Thickness(2);
                        return;
                    }
                    else
                    {
                        InfoBlock.Text = Message;
                        element.BaseControl.BorderBrush = new SolidColorBrush(Colors.Gray);
                        element.BaseControl.BorderThickness = new Thickness(1);
                    }
                }
                else
                {
                    InfoBlock.Text = "Bitte füllen Sie alle Muss Felder aus.";
                    element.BaseControl.BorderBrush =  new SolidColorBrush(Color.FromArgb(170,255,50,50));
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
            string op = "";
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
            if (numberRegex.IsMatch(split[1]))
            {
                secondValue = double.Parse(split[1]);
            }

            switch (op)
            {
                case "<": return new Func<bool>(() => firstValue < secondValue);
                case ">": return new Func<bool>(() => firstValue > secondValue);
                case "<=": return new Func<bool>(() => firstValue <= secondValue);
                case ">=": return new Func<bool>(() => firstValue >= secondValue);
                case "==": return new Func<bool>(() => Math.Abs(firstValue - secondValue) < 0.000000001);
                case "!=": return new Func<bool>(() => Math.Abs(firstValue - secondValue) > 0.000000001);
                default: return null;
            }

            return null;
        }
    }
}