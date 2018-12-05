using System;
using System.Collections.Generic;
using System.IO;
using System.Windows;
using System.Xml;
using DynamicDocsWPF.HelperClasses;
using Microsoft.Office.Interop.Word;
using Microsoft.Win32;
using RestService;
using RestService.Model.Database;
using RestService.Model.Process;
using Application = System.Windows.Application;
using Window = System.Windows.Window;

namespace DynamicDocsWPF.Windows
{
    public partial class CreateProcessTemplate : Window
    {
        private bool _isOkay = false;
        private ProcessObject _processObject;
        private NetworkHelper _networkHelper;
        private string _filePath = null;
        public CreateProcessTemplate(NetworkHelper networkHelper)
        {
            _networkHelper = networkHelper;
            InitializeComponent();
            
            
        }

        private void CreateProcessTemplate_OnClick(object sender, RoutedEventArgs e)
        {
            if (_isOkay == true)
            {
                var list = CheckDependencies();
                
                if (list == null) return;
                
                foreach (var element in list)
                {
                    var docUploadResult = _networkHelper.UploadDocTemplate(element.Id, element.FilePath, true);

                    switch (docUploadResult)
                    {
                        case UploadResult.SUCCESS:
                            break;
                        case UploadResult.FAILED_ID_EXISTS:
                            new InfoPopup(MessageBoxButton.OK,$"Eine Vorlage \"{element.Id}\" bestand bereits und konnte nicht überschrieben werden. Tipp: Ändern sie den Namen in <receipt...>").ShowDialog();
                            Close();
                            return;
                        case UploadResult.FAILED_FILEEXISTS:
                            new InfoPopup(MessageBoxButton.OK,$"Eine Vorlage \"{element.Id}\" bestand bereits und konnte nicht überschrieben werden. Tipp: Ändern sie den Namen in <receipt...>").ShowDialog();
                            Close();
                            return;
                        case UploadResult.FAILED_FILE_OR_TYPE_INVALID:
                            new InfoPopup(MessageBoxButton.OK,"Der Server hat die Vorlagedatei als fehlerhaft gemeldet. Bitte überprüfen Sie ihre Vorlage.").ShowDialog();
                            Close();
                            return;
                        case UploadResult.FAILED_OTHER:
                            new InfoPopup(MessageBoxButton.OK,$"Ups da ist wohl etwas beim Upload der Vorlage \"{element.Id}\" schief gelaufen. Bitte wenden Sie sich an einen Administrator.").ShowDialog();
                            Close();
                            return;
                        case UploadResult.NO_PERMISSION:
                            new InfoPopup(MessageBoxButton.OK,"Es tut uns leid, doch sie besitzen nicht die notwendigen Berechtigungen.").ShowDialog();
                            Close();
                            return;
                        case UploadResult.INVALID_LOGIN:
                            new InfoPopup(MessageBoxButton.OK,"Irgendetwas scheint mit ihrem Konto nicht zu stimmen. Bitte starten sie das Programm neu.").ShowDialog();
                            Application.Current.Shutdown();
                            return;
                    }
                }
                
                var processUploadResult = _networkHelper.UploadProcessTemplate(_filePath, true);
                switch (processUploadResult)
                {
                    case UploadResult.SUCCESS:
                        new InfoPopup(MessageBoxButton.OK,"Vorlage erfolgreich angelegt.").ShowDialog();
                        Close();
                        break;
                    case UploadResult.FAILED_ID_EXISTS:
                        new InfoPopup(MessageBoxButton.OK,"Ein Prozess mit diesem technischen Namen bestand bereits und konnte nicht überschrieben werden. Tipp: Ändern sie den Namen in <process...>").ShowDialog();
                        Close();
                        return;
                    case UploadResult.FAILED_FILE_OR_TYPE_INVALID:
                        new InfoPopup(MessageBoxButton.OK,"Der Server hat die Prozessdatei als fehlerhaft gemeldet. Bitte überarbeiten Sie den Prozess.").ShowDialog();
                        Close();
                        return;
                    case UploadResult.FAILED_OTHER:
                        new InfoPopup(MessageBoxButton.OK,"Ups da ist wohl etwas schief gelaufen. Bitte wenden Sie sich an einen Administrator.").ShowDialog();
                        Close();
                        return;
                    case UploadResult.NO_PERMISSION:
                        new InfoPopup(MessageBoxButton.OK,"Es tut uns leid, doch sie besitzen nicht die notwendigen Berechtigungen.").ShowDialog();
                        Close();
                        return;
                    case UploadResult.INVALID_LOGIN:
                        new InfoPopup(MessageBoxButton.OK,"Irgendetwas scheint mit ihrem Konto nicht zu stimmen. Bitte starten sie das Programm neu.").ShowDialog();
                        Application.Current.Shutdown();
                        return;
                    default:
                        throw new ArgumentOutOfRangeException();
                }
            }
        }

        private void SelectFileOnClick(object sender, RoutedEventArgs e)
        {
            var dialog = new OpenFileDialog();
            dialog.Filter = "Process Files (*.xml)|*.xml";
            dialog.ShowDialog();

            if (!string.IsNullOrWhiteSpace(dialog.FileName))
            {
                try
                {
                    var xmlState = XmlHelper.TryReadXmlFromPath(dialog.FileName, out _processObject);

                    switch (xmlState)
                    {
                        case XmlState.VALID:
                            var processTemplate = _networkHelper.GetProcessTemplate(_processObject.Name);

                            if (processTemplate != null)
                            {
                                var info = new InfoPopup(MessageBoxButton.YesNo, $"Der Prozess \"{_processObject.Name}\", existiert bereits. Möchten Sie ihn ersetzen?");
                                info.ShowDialog();
                                if (DialogResult == false)
                                {
                                    new InfoPopup(MessageBoxButton.OK, "Bitte überarbeiten Sie den Prozess.").ShowDialog();
                                    Close();
                                    return;
                                }   
                            }

                            InfoText.Text = "Super! Die Datei scheint korrekt zu sein.";
                            _filePath = dialog.FileName;
                            _isOkay = true;
                            
                            break;
                        case XmlState.MISSINGATTRIBUTE:
                            new InfoPopup(MessageBoxButton.OK, "Einem der Tags scheint ein Attribut zu fehlen. Bitte prüfen Sie ihre Datei.").ShowDialog();
                            break;
                        case XmlState.MISSINGPARENTTAG:
                            new InfoPopup(MessageBoxButton.OK, "Einer der Tags befindet sich nicht in seinem Parenttag. Bitte prüfen Sie ihre Datei.").ShowDialog();
                            break;
                        case XmlState.INVALID:
                            new InfoPopup(MessageBoxButton.OK, "Die Datei weist einen nicht eindeutigen Fehler auf. Fehlen Klammern, Tags oder Anführungszeichen? Bitte prüfen Sie ihre Datei.").ShowDialog();
                            break;
                        default:
                            throw new ArgumentOutOfRangeException();
                    }
                    
                }
                catch (ArgumentOutOfRangeException e3)
                {
                    var value = e3.ActualValue.ToString();
                    InfoText.Text = $"Die XML beinhaltet einen unbekannten Tag \"{value}\"";
                }
            }
        }

        private List<DocTemplate> CheckDependencies()
        {
            var list = new List<DocTemplate>();
            var stepEnum = _processObject.Steps;
            
            while(stepEnum.MoveNext())
            {
                foreach(var receipt in stepEnum.Current.Receipts)
                {
                    CheckReceipt(receipt);
                }

                var validation = stepEnum.Current.GetValidationAtIndex(0);

                if (validation != null)
                {
                    if(validation.Accepted != null)
                        foreach (var receipt in validation.Accepted.Receipts)
                        {
                            CheckReceipt(receipt);
                        }
                    
                    if(validation.Accepted != null)
                        foreach (var receipt in validation.Declined.Receipts)
                        {
                            CheckReceipt(receipt);
                        }
                }
            }

            return list;
        }

        private DocTemplate CheckReceipt(ReceiptElement receipt)
        {
            var onlineTemplate = _networkHelper.GetDocTemplate(receipt.DraftName);
                    if (onlineTemplate == null)
                    {
                        new InfoPopup(MessageBoxButton.OK, $"Der Prozess erfordert eine Vorlage \"{receipt.DraftName}\". Bitte wählen Sie eine Datei aus.").ShowDialog();
                        var dialog = new OpenFileDialog();
                        dialog.Filter = "Draft Files (*.docx)|*.docx";
                        dialog.ShowDialog();

                        if (File.Exists(dialog.FileName))
                        {                          
                            return new DocTemplate(){Id = receipt.DraftName, FilePath = dialog.FileName};
                        }
                        else
                        {
                            new InfoPopup(MessageBoxButton.OK, "Ups. Da ist wohl etwas schief gelaufen. Die Datei konnte nicht gefunden werden.").ShowDialog();
                            Close();
                            return null;
                        }
                    }
                    else
                    {
                        var info = new InfoPopup(MessageBoxButton.YesNo, $"Der Prozess erfordert eine Vorlage \"{receipt.DraftName}\", die bereits auf dem Server existiert. Möchten Sie sie ersetzen?");
                        info.ShowDialog();
                        if (info.DialogResult == true)
                        {
                            var dialog = new OpenFileDialog();
                            dialog.Filter = "Draft Files (*.docx)|*.docx";
                            dialog.ShowDialog();

                            if (File.Exists(dialog.FileName))
                            {
                                return new DocTemplate(){Id = receipt.DraftName, FilePath = dialog.FileName};
                            }
                            else
                            {
                                new InfoPopup(MessageBoxButton.OK, "Ups. Da ist wohl etwas schief gelaufen. Die Datei konnte nicht gefunden werden.").ShowDialog();
                                Close();
                                return null;
                            }
                        }
                        else
                        {
                            new InfoPopup(MessageBoxButton.OK, "Bitte überarbeiten Sie den Prozess.").ShowDialog();
                            Close();
                            return null;
                        }
                    }
        }
    }
}
