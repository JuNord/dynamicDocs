using System;
using System.IO;
using System.Windows;
using System.Xml;
using DynamicDocsWPF.HelperClasses;
using Microsoft.Win32;
using RestService;
using RestService.Model.Process;

namespace DynamicDocsWPF.Windows
{
    public partial class CreateProcessTemplate : Window
    {
        private bool _isOkay = false;
        private Process _process;
        private NetworkHelper _networkHelper;
        public CreateProcessTemplate(NetworkHelper networkHelper)
        {
            _networkHelper = networkHelper;
            InitializeComponent();
        }

        private void CreateProcessTemplate_OnClick(object sender, RoutedEventArgs e)
        {
            if (_isOkay == true)
            {
                CheckDependencies();
            }
        }

        private void SelectFileOnClick(object sender, RoutedEventArgs e)
        {
            var dialog = new OpenFileDialog();
            dialog.Filter = "Process Files (*.xml)|*.xml";
            dialog.ShowDialog();

            if (!dialog.FileName.Equals(""))
            {
                // Assuming you have one file that you care about, pass it off to whatever
                // handling code you have defined.
                try
                {
                    _process = XmlHelper.ReadXMLFromPath(dialog.FileName);
                    InfoText.Text = "Super! Die Datei scheint korrekt zu sein.";
                    _networkHelper.UploadProcessTemplate(dialog.FileName, true);
                    _isOkay = true;
                }
                catch (XmlException e2)
                {
                    InfoText.Text = "Die XML ist fehlerhaft. Sind alle Tags geschlossen?";
                }
                catch (ArgumentOutOfRangeException e3)
                {
                    var value = e3.ActualValue as string;
                    InfoText.Text = $"Die XML beinhaltet einen unbekannten Tag \"{value}\"";
                }
            }
        }

        private void CheckDependencies()
        {
            for (int i = 0; i < _process.ProcessStepCount; i++)
            {
                var step = _process.GetStepAtIndex(i);
                for (int j = 0; j < step.ReceiptCount; j++)
                {
                    var receipt = step.GetReceiptAtIndex(j);
                    
                    MessageBox.Show("Die Prozessdatei verweist auf ein Draft. Bitte wählen Sie die zugehörige Datei aus.");
                    var dialog = new OpenFileDialog();
                    dialog.Filter = "Draft Files (*.docx)|*.docx";
                    dialog.ShowDialog();

                    if (File.Exists(dialog.FileName))
                    {
                        var result = _networkHelper.UploadDocTemplate(receipt.DraftName, Path.GetFileName(dialog.FileName), false);

                        if (result == UploadResult.FAILED_FILEEXISTS)
                            MessageBox.Show("The draftfile already exists.");
                    }
                }
            }
        }
    }
}
