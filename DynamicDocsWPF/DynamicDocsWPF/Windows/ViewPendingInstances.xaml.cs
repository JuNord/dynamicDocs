using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Management.Instrumentation;
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
        private int _currentStepIndex = 0;
        
        private ProcessInstance SelectedInstance { get; set; }

        
        public ViewPendingInstances(NetworkHelper networkHelper)
        {
            InitializeComponent();
            _networkHelper = networkHelper;
            Refresh();
        }

        public void Refresh()
        {
            var list = TryGetResponsibilities();
            if (InstanceList?.ItemsSource != null)
            {
                if (list.SequenceEqual((List<ProcessInstance>)InstanceList.ItemsSource)) return;
            }

            InstanceList.ItemsSource = list;
        }

        private List<ProcessInstance> TryGetResponsibilities()
        {
            try
            {
                var responsibilities = _networkHelper.GetResponsibilities();
                var instances = new List<ProcessInstance>();
                
                foreach (var responsibility in responsibilities)
                {
                    var instance = _networkHelper.GetProcessInstanceById(responsibility.InstanceId);
                    if (instance != null)
                    {
                        instances.Add(instance);
                    }
                }
                
                return instances;
            }
            catch (WebException)
            {
                InfoPopup.ShowOk(
                        "Leider konnten die laufenden Prozesse nicht vom Server bezogen werden. Bitte melden Sie sich bei einem Administrator.");
            }

            return null;
        }
        
        private void Next_Click(object sender, RoutedEventArgs e)
        {
            if (TryShowNextDialog(_entries)) return;

            if (_currentStepIndex + 1 <= SelectedInstance.CurrentStep)
            {
                _currentStepIndex++;
                _processSteps.MoveNext();
            }
            else if (_currentStepIndex + 1 > SelectedInstance.CurrentStep)
            {
                if (_processSteps.Current.DialogCount > 0)
                {
                    SendData();
                }
                if (_processSteps.Current.ValidationCount > 0)
                {
                    var initialPopup = InfoPopup.ShowOk(
                        "Möchten Sie diesen Schritt genehmigen? Weitere Instanzen werden gegebenenfalls über die Genehmigung in Kenntnis gesetzt.");

                    var validationElement = _processSteps.Current.GetValidationAtIndex(0);
                    if (initialPopup == true)
                    {
                        var acceptPopup = InfoPopup.ShowOk(
                            "Möchten Sie wirklich zustimmen? Dieser Schritt kann nicht rückgängig gemacht werden! Wählen Sie nein um die Daten erneut zu prüfen.");

                        if (acceptPopup)
                        {
                            var validation = _processSteps.Current.GetValidationAtIndex(0);
                            _networkHelper.PostProcessUpdate(SelectedInstance.Id, false, validation.Locks);

                            Refresh();
                        }
                    }
                    else
                    {
                        var declinePopup = InfoPopup.ShowOk(
                            "Möchten Sie wirklich ablehnen? Dieser Schritt kann nicht rückgängig gemacht werden! Wählen Sie nein um die Daten erneut zu prüfen.");

                        if (declinePopup)
                        {
                            var validation = _processSteps.Current.GetValidationAtIndex(0);
                            _networkHelper.PostProcessUpdate(SelectedInstance.Id, true, validation.Locks);

                            if (validationElement?.Declined != null)
                            {
                                foreach (var receipt in validationElement.Declined.Receipts)
                                {
                                    var fileName = $"{receipt.DraftName}_{DateTime.Now.ToShortDateString()}.docx";
                                    var documentTemplate = _networkHelper.GetDocTemplate(receipt.DraftName);
                                    File.WriteAllBytes(fileName, Encoding.Default.GetBytes(documentTemplate.Content));

                                    var replacements = new List<KeyValuePair<string, string>>();

                                    foreach (var entry in _entries)
                                    {
                                        replacements.Add(
                                            new KeyValuePair<string, string>($"[{entry.FieldName}]", entry.Data));
                                    }

                                    WordReceiptHelper.OpenDocument(fileName, replacements.ToArray());

                                }
                            }

                            Refresh();
                        }
                    }

                    return;
                }
            }

            _dialogs = _processSteps.Current.Dialogs;
            TryShowNextDialog(_entries);
        }
        
        private bool SendData()
        {
            for (var i = 0; i < _processSteps.Current.DialogCount; i++)
            {
                var dialog = _processSteps.Current.GetDialogAtIndex(i);
                for (var j = 0; j < dialog.ElementCount; j++)
                {
                    var element = dialog.GetElementAtIndex(j);
                    var entry = new Entry()
                    {
                        InstanceId = SelectedInstance.Id,
                        FieldName = element.Name,
                        Data = element.GetFormattedValue()
                    };

                    _networkHelper.CreateEntry(entry);
                }
            }

            return true;          
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
            try
            {
                ContentSection.Visibility =
                    InstanceList.SelectedIndex == -1 ? Visibility.Collapsed : Visibility.Visible;
                if (InstanceList.SelectedIndex != -1)
                {                 
                    SelectedInstance =
                        _networkHelper.GetProcessInstanceById(((ProcessInstance) InstanceList.SelectedItem).Id);
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
            }
            catch (NullReferenceException)
            {
                InfoPopup.ShowOk(
                    "Leider konnte der gewählte Prozess nicht vom Server bezogen werden. Bitte melden Sie sich bei einem Administrator.");
            }
            catch (Exception)
            {
                InfoPopup.ShowOk(
                    "Leider konnte der gewählte Prozess nicht vom Server bezogen werden. Bitte melden Sie sich bei einem Administrator.");
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
                InfoPopup.ShowOk(
                    $"The process contained an element called \"{uiElement.Name}\" that couldnt be found.");
            }
        }
    }
}
