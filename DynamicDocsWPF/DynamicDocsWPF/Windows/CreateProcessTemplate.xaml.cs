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
    public partial class CreateProcessTemplate
    {
        private bool _isOkay;
        private ProcessObject _processObject;
        private readonly NetworkHelper _networkHelper;
        private string _filePath;
        public CreateProcessTemplate(NetworkHelper networkHelper)
        {
            _networkHelper = networkHelper;
            InitializeComponent();
            
            
        }

        private void CreateProcessTemplate_OnClick(object sender, RoutedEventArgs e)
        {
            if (_isOkay)
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
                            InfoPopup.ShowOk(
                                $"Eine Vorlage \"{element.Id}\" bestand bereits und konnte nicht überschrieben werden. Tipp: Ändern sie den Namen in <receipt...>");
                            Close();
                            return;
                        case UploadResult.FAILED_FILEEXISTS:
                            InfoPopup.ShowOk($"Eine Vorlage \"{element.Id}\" bestand bereits und konnte nicht überschrieben werden. Tipp: Ändern sie den Namen in <receipt...>");
                            Close();
                            return;
                        case UploadResult.FAILED_FILE_OR_TYPE_INVALID:
                            InfoPopup.ShowOk("Der Server hat die Vorlagedatei als fehlerhaft gemeldet. Bitte überprüfen Sie ihre Vorlage.");
                            Close();
                            return;
                        case UploadResult.FAILED_OTHER:
                            InfoPopup.ShowOk($"Ups da ist wohl etwas beim Upload der Vorlage \"{element.Id}\" schief gelaufen. Bitte wenden Sie sich an einen Administrator.");
                            Close();
                            return;
                        case UploadResult.NO_PERMISSION:
                            InfoPopup.ShowOk("Es tut uns leid, doch sie besitzen nicht die notwendigen Berechtigungen.");
                            Close();
                            return;
                        case UploadResult.INVALID_LOGIN:
                            InfoPopup.ShowOk("Irgendetwas scheint mit ihrem Konto nicht zu stimmen. Bitte starten sie das Programm neu.");
                            Application.Current.Shutdown();
                            return;
                    }
                }
                
                var processUploadResult = _networkHelper.UploadProcessTemplate(_filePath, true);
                switch (processUploadResult)
                {
                    case UploadResult.SUCCESS:
                        InfoPopup.ShowOk("Vorlage erfolgreich angelegt.");
                        Close();
                        break;
                    case UploadResult.FAILED_ID_EXISTS:
                        InfoPopup.ShowOk("Ein Prozess mit diesem technischen Namen bestand bereits und konnte nicht überschrieben werden. Tipp: Ändern sie den Namen in <process...>");
                        Close();
                        return;
                    case UploadResult.FAILED_FILE_OR_TYPE_INVALID:
                        InfoPopup.ShowOk("Der Server hat die Prozessdatei als fehlerhaft gemeldet. Bitte überarbeiten Sie den Prozess.");
                        Close();
                        return;
                    case UploadResult.FAILED_OTHER:
                        InfoPopup.ShowOk("Ups da ist wohl etwas schief gelaufen. Bitte wenden Sie sich an einen Administrator.");
                        Close();
                        return;
                    case UploadResult.NO_PERMISSION:
                        InfoPopup.ShowOk("Es tut uns leid, doch sie besitzen nicht die notwendigen Berechtigungen.");
                        Close();
                        return;
                    case UploadResult.INVALID_LOGIN:
                        InfoPopup.ShowOk("Irgendetwas scheint mit ihrem Konto nicht zu stimmen. Bitte starten sie das Programm neu.");
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
                                var info = InfoPopup.ShowYesNo($"Der Prozess \"{_processObject.Name}\", existiert bereits. Möchten Sie ihn ersetzen?");
 
                                if (info == false)
                                {
                                    InfoPopup.ShowOk( "Bitte überarbeiten Sie den Prozess.");
                                    Close();
                                    return;
                                }   
                            }

                            InfoText.Text = "Super! Die Datei scheint korrekt zu sein.";
                            _filePath = dialog.FileName;
                            _isOkay = true;
                            
                            break;
                        case XmlState.MISSINGATTRIBUTE:
                            InfoPopup.ShowOk( "Einem der Tags scheint ein Attribut zu fehlen. Bitte prüfen Sie ihre Datei.");
                            break;
                        case XmlState.MISSINGPARENTTAG:
                            InfoPopup.ShowOk( "Einer der Tags befindet sich nicht in seinem Parenttag. Bitte prüfen Sie ihre Datei.");
                            break;
                        case XmlState.INVALID:
                            InfoPopup.ShowOk( "Die Datei weist einen nicht eindeutigen Fehler auf. Fehlen Klammern, Tags oder Anführungszeichen? Bitte prüfen Sie ihre Datei.");
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
                            list.Add(CheckReceipt(receipt));
                        }
                    
                    if(validation.Declined != null)
                        foreach (var receipt in validation.Declined.Receipts)
                        {
                            list.Add(CheckReceipt(receipt));
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
                        InfoPopup.ShowOk( $"Der Prozess erfordert eine Vorlage \"{receipt.DraftName}\". Bitte wählen Sie eine Datei aus.");
                        var dialog = new OpenFileDialog();
                        dialog.Filter = "Draft Files (*.docx)|*.docx";
                        dialog.ShowDialog();

                        if (File.Exists(dialog.FileName))
                        {                          
                            return new DocTemplate(){Id = receipt.DraftName, FilePath = dialog.FileName};
                        }
                        else
                        {
                            InfoPopup.ShowOk( "Ups. Da ist wohl etwas schief gelaufen. Die Datei konnte nicht gefunden werden.");
                            Close();
                            return null;
                        }
                    }
                    else
                    {
                        var info = InfoPopup.ShowYesNo($"Der Prozess erfordert eine Vorlage \"{receipt.DraftName}\", die bereits auf dem Server existiert. Möchten Sie sie ersetzen?");
                        
                        if (info == true)
                        {
                            var dialog = new OpenFileDialog {Filter = "Draft Files (*.docx)|*.docx"};
                            dialog.ShowDialog();

                            if (File.Exists(dialog.FileName))
                            {
                                return new DocTemplate(){Id = receipt.DraftName, FilePath = dialog.FileName};
                            }
                            else
                            {
                                InfoPopup.ShowOk( "Ups. Da ist wohl etwas schief gelaufen. Die Datei konnte nicht gefunden werden.");
                                Close();
                                return null;
                            }
                        }
                        else
                        {
                            InfoPopup.ShowOk( "Bitte überarbeiten Sie den Prozess.");
                            Close();
                            return null;
                        }
                    }
        }
    }
}
