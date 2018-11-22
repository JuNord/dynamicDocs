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
        private Dialog _dialog;
        
        public MainWindow()
        {
            InitializeComponent();
            //Test();
        }

        private void Test()
        {
            _process = new Process()
            {
                Name = "vacation",
                Description = "Urlaubsantrag"
            };
            _processStep = new ProcessStep(_process);
            _dialog = new Dialog(_processStep);

            var teacherDropdown = new TeacherDropdown(_dialog, true)
            {
                Description = "Lehrer"
            };
            teacherDropdown.IsValidForProcess = () => teacherDropdown.GetValue().Equals("Ulrich Böhmer");
            teacherDropdown.ProcessErrorMsg = "Der Wert darf nur Uli sein";
            
            _dialog.AddElement(teacherDropdown);
            _dialog.AddElement(new StudentDropdown(_dialog)
            {
                Description = "Schüler"
            });
            _dialog.AddElement(new NumberInputBox(_dialog)
            {
                Description = "Urlaubstage"
            });
            
            ViewCreator.FillViewHolder(ViewHolder, _dialog);
        }

        private void ButtonBase_OnClick(object sender, RoutedEventArgs e)
        {
            for (var i = 0; i < _dialog?.GetElementCount(); i++)
            {
                var element = _dialog.GetElementAtIndex(i);
                if (element.CheckObligatory())
                {
                    if (!element.CheckValidForProcess())
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
                        InfoBlock.Text = "Hallo!";
                        element.BaseControl.BorderBrush = new SolidColorBrush(Colors.Gray);
                        element.BaseControl.BorderThickness = new Thickness(1);
                    }
                }
                else
                {
                    InfoBlock.Text = "Bitte füllen sie alle Muss-Felder aus.";
                    element.BaseControl.BorderBrush =  new SolidColorBrush(Color.FromArgb(170,255,50,50));
                    element.BaseControl.BorderThickness = new Thickness(2);
                    return;
                    
                }
            }
        }
    }
}