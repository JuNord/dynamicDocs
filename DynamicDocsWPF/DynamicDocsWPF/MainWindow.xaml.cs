using System;
using System.Runtime.Remoting.Messaging;
using System.Text.RegularExpressions;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using DynamicDocsWPF.Model.InputElements;
using DynamicDocsWPF.Model.Surrounding_Tags;
using DynamicDocsWPF.UIGeneration;
using RestService;

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
            Test();
        }

        private void Test()
        {
            _process = new Process()
            {
                Name = "vacation",
                Description = "Urlaubsantrag"
            };
            _processStep = new ProcessStep(_process);
            
            
            
            var dialog = new Dialog(_processStep);

            dialog.AddElement(new TeacherDropdown(dialog, true)
                {
                    Description = "Lehrer/in"
                });
            dialog.AddElement(new ClassDropDown(dialog, true)
            {
                Description = "Klasse"
            });
            
            _processStep.AddDialog(dialog);
            dialog = new Dialog(_processStep);

            dialog.AddElement(new DateDropdown(dialog, true)
            {
                Description = "Datum"
            });
            dialog.AddElement(new NumberInputBox(dialog,true)
            {
                Description = "Unterrichtsstunden"
            });
            
            _processStep.AddDialog(dialog);
            dialog = new Dialog(_processStep);
            
            dialog.AddElement(new TextInputBox(dialog,true)
            {
                Description = "Neuer Unterrichtsort"
            });
            dialog.AddElement(new TextInputBox(dialog,true)
            {
                Description = "Ort des Unterrichtsbeginns bei 1. Stunde"
            });
            
            _processStep.AddDialog(dialog);
            dialog = new Dialog(_processStep);
            
            dialog.AddElement(new TextInputBox(dialog,true)
            {
                Description = "Begründung / Fächerbezug"
            });
            
            _processStep.AddDialog(dialog);
            
            ViewCreator.FillViewHolder(ViewHolder, _processStep.GetDialogAtIndex(0));
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
                    else if (!element.CheckValidForControl())
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