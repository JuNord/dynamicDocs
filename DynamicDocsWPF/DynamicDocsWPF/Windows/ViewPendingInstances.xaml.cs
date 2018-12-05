using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using DynamicDocsWPF.HelperClasses;
using RestService;
using RestService.Model.Database;
using RestService.Model.Input;
using RestService.Model.Process;

namespace DynamicDocsWPF.Windows
{
    public partial class ViewPendingInstances
    {
        private readonly NetworkHelper _networkHelper;
        private ProcessObject _currentProcessObject;
        private CustomEnumerable<ProcessStep> _processSteps;
        private CustomEnumerable<Dialog> _dialogs;
        private List<Entry> _entries;
        
        private ProcessInstance SelectedInstance { get; set; }

        
        public ViewPendingInstances(NetworkHelper networkHelper)
        {
            InitializeComponent();
            _networkHelper = networkHelper;
            InstanceList.ItemsSource = TryGetResponsibilities();
        }

        private List<PendingInstance> TryGetResponsibilities()
        {
            try
            {
                var responsibilities = _networkHelper.GetResponsibilities();

                if (responsibilities == null)
                {
                    new InfoPopup(MessageBoxButton.OK,
                            "Leider konnten die laufenden Prozesse nicht vom Server bezogen werden. Bitte melden Sie sich bei einem Administrator.")
                        .ShowDialog();

                }

                return responsibilities;
            }
            catch (WebException)
            {
                new InfoPopup(MessageBoxButton.OK,
                        "Leider konnten die laufenden Prozesse nicht vom Server bezogen werden. Bitte melden Sie sich bei einem Administrator.")
                    .ShowDialog();
            }

            return null;
        }
        
        private void Next_Click(object sender, RoutedEventArgs e)
        {
            if (TryShowNextDialog(_entries)) return;
            if (!(_processSteps?.MoveNext() ?? false))
            {
                var initialPopup = new InfoPopup(MessageBoxButton.YesNo,
                    "Möchten Sie diesen Schritt genehmigen? Weitere Instanzen werden gegebenenfalls über die Genehmigung in Kenntnis gesetzt.");

                var validationElement = _processSteps.Current.GetValidationAtIndex(0);
                if (initialPopup.ShowDialog() == true)
                {
                    var acceptPopup = new InfoPopup(MessageBoxButton.YesNo,
                        "Möchten Sie wirklich zustimmen? Dieser Schritt kann nicht rückgängig gemacht werden! Wählen Sie nein um die Daten erneut zu prüfen.");

                    if (acceptPopup.ShowDialog() == true)
                    {
                        var validation = _processSteps.Current.GetValidationAtIndex(0);
                        _networkHelper.PostProcessUpdate(SelectedInstance.Id, false, validation.Locks);
                        
                        InstanceList.ItemsSource = _networkHelper.GetResponsibilities();
                    }
                }
                else
                {
                    var declinePopup = new InfoPopup(MessageBoxButton.YesNo,
                        "Möchten Sie wirklich ablehnen? Dieser Schritt kann nicht rückgängig gemacht werden! Wählen Sie nein um die Daten erneut zu prüfen.");

                    if (declinePopup.ShowDialog() == true)
                    {
                        var validation = _processSteps.Current.GetValidationAtIndex(0);
                        _networkHelper.PostProcessUpdate(SelectedInstance.Id, true, validation.Locks);

                        if (validationElement?.Declined != null)
                        {
                            foreach (var receipt in validationElement.Declined.Receipts)
                            {
                                var fileName = $"{receipt.DraftName}_{DateTime.Now.ToShortDateString()}.docx";
                                var documentTemplate = _networkHelper.GetDocTemplate(receipt.DraftName);
                                File.WriteAllBytes(fileName ,Encoding.Default.GetBytes(documentTemplate.Content));

                                var replacements = new List<KeyValuePair<string, string>>();

                                foreach (var entry in _entries)
                                {
                                    replacements.Add(new KeyValuePair<string, string>($"[{entry.FieldName}]", entry.Data));
                                }
                                WordReceiptHelper.OpenDocument(fileName, replacements.ToArray());
                                
                            }
                        }
                        InstanceList.ItemsSource = TryGetResponsibilities();
                    }
                }
                return;
            };
            
            _dialogs = _processSteps.Current.Dialogs;
            TryShowNextDialog(_entries);
        }

        private void Back_Click(object sender, RoutedEventArgs e)
        {
            if (TryShowLastDialog(_entries)) return;
            if (((string)BtnNext.Content).Equals("Änderungen Speichern"))
            {
                BtnNext.Content = "Weiter";
            }
            
            _dialogs = _processSteps.Current.Dialogs;
            TryShowLastDialog(_entries);
        }

        private bool TryShowNextDialog(List<Entry> entries)
        {
            if (!(_dialogs?.MoveNext() ?? false)) return false;
            ShowCurrentDialog(entries);
            return true;
        }
        
        private bool TryShowLastDialog(List<Entry> entries)
        {
            if (!(_dialogs?.MoveBack() ?? false)) return false;
            ShowCurrentDialog(entries);
            return true;
        }
        
        private void ShowCurrentDialog(List<Entry> entries)
        {
            FillElements(entries, _dialogs.Current.Elements);
            ViewHolder.Content = _dialogs.Current.GetStackPanel();
        }
        
        private void FillElements(List<Entry> entries, CustomEnumerable<BaseInputElement> elements)
        {
            foreach (var uiElement in elements)
            {
                FillUiElement(entries, uiElement);
            }
        }

        private void InstanceList_OnSelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            Overlay.Visibility = InstanceList.SelectedIndex == -1 ? Visibility.Visible : Visibility.Collapsed;
            
            SelectedInstance = _networkHelper.GetProcessInstanceById(((PendingInstance) InstanceList.SelectedItem).InstanceId);
            _entries = _networkHelper.GetEntries(SelectedInstance.Id);
            
            var processText = _networkHelper.GetProcessTemplate(SelectedInstance.TemplateId);
            _currentProcessObject = XmlHelper.ReadXmlFromString(processText);
            _processSteps = _currentProcessObject.Steps;

            if (_processSteps?.MoveNext() ?? false)
            {
                _dialogs = _processSteps.Current.Dialogs;
                TryShowNextDialog(_entries);
            }
            
        }

        private void FillUiElement(List<Entry> entries, BaseInputElement uiElement)
        {
            try
            {
                var result = entries.First(entry => entry.FieldName.Equals(uiElement.Name));
                uiElement.SetValueFromString(result.Data);
                uiElement.SetEnabled(!SelectedInstance.Locked);
            }
            catch (Exception)
            {
                new InfoPopup(MessageBoxButton.OK,
                        $"The process contained an element called \"{uiElement.Name}\" that couldnt be found.")
                    .ShowDialog();
            }
        }
    }
}
