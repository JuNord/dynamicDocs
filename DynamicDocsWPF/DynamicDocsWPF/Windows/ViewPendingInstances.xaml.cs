using System;
using System.Collections.Generic;
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
    /// <summary>
    /// A window with a list of instances that need to be approved aswell as controls to handle them
    /// </summary>
    public partial class ViewPendingInstances
    {
        private readonly NetworkHelper _networkHelper;
        
        private ProcessObject _curProcObj;
        private CustomEnumerable<Dialog> _dialogs;
        private CustomEnumerable<ProcessStep> _steps;
        private List<Entry> _entries;
        private int _curStepIndex;
        
        private ProcessInstance SelectedInstance { get; set; }
        
        public ViewPendingInstances(NetworkHelper networkHelper)
        {
            InitializeComponent();
            _networkHelper = networkHelper;
            Refresh();
        }

        /// <summary>
        /// Fill the list of Instances with instances the logged in User is responsible for
        /// </summary>
        public void Refresh()
        {
            var list = GetResponsibilities();
            if (InstanceList?.ItemsSource != null)
                if (list.SequenceEqual((List<ProcessInstance>) InstanceList.ItemsSource))
                    return;

            if (InstanceList != null) 
                InstanceList.ItemsSource = list;
        }

        /// <summary>
        /// Retrieve the list of Instances, by receiving Pending Instances and matching them to their corresponding Process Instances
        /// </summary>
        /// <returns></returns>
        private List<ProcessInstance> GetResponsibilities()
        {
            if (_networkHelper != null)
            {
                try
                {
                    if (Running.IsChecked == false) return _networkHelper.GetArchived();

                    var responsibilities = _networkHelper.GetResponsibilities();
                    //Retrieve a list of pending instances from the server
                    var instances = new List<ProcessInstance>();

                    //Retrieve the process instance object of every pending instance
                    foreach (var responsibility in responsibilities)
                    {
                        var instance = _networkHelper.GetProcessInstanceById(responsibility.InstanceId);
                        if (instance != null) instances.Add(instance);
                    }

                    return instances;
                }

                catch (WebException)
                {
                    InfoPopup.ShowOk(StringResources.LoadPendingError);
                }
            }

            return null;
        }

        /// <summary>
        /// Handles clicks on the next button.
        /// <para>
        /// Loads the next dialogs content from the server into the UI or requests to accept or decline a request,
        /// if on the last dialog.
        /// </para>
        /// </summary>
        private void Next_Click(object sender, RoutedEventArgs e)
        {
            //If there's a next dialog in the current step show it and return
            if (TryShowNextDialog(_entries)) return;

            //If the last dialog is reached and there is another process-step, load its first dialog
            if (_curStepIndex + 1 <= SelectedInstance.CurrentStepIndex)
            {
                _curStepIndex++;
                _steps.MoveNext();
                _dialogs = _steps.Current?.Dialogs;
                TryShowNextDialog(_entries);
            }
            //If we reached the last dialog AND step, handle validations and additional input by the validator
            else if (_curStepIndex + 1 > SelectedInstance.CurrentStepIndex)
            {
                //If in the current step we have any input forms, send their input
                if ((_steps.Current?.DialogCount??0) > 0) SendData();

                //If theres no validation tags these values are automatically accepted after reading
                if ((_steps.Current?.ValidationCount ?? 0) <= 0)
                {
                    _networkHelper.PostProcessUpdate(SelectedInstance.Id, false, false);
                    InfoPopup.ShowOk("Sie haben die Daten gelesen und somit bestätigt. Sie haben nun keinen Zugriff mehr.");
                    return;
                }
                
                //Ask the user if he accepts or declines the submitted data, reassure his answer and send an update to the server if necessary
                var validates = InfoPopup.ShowYesNo(StringResources.WantsToValidate, "Genehmigen", "Ablehnen");
                if (InfoPopup.ShowYesNo(StringResources.ValidationSure) == false) return;

                if (_steps.Current != null)
                {
                    var validationElement = _steps.Current.GetValidationAtIndex(0);
                    
                    _networkHelper.PostProcessUpdate(SelectedInstance.Id, !validates, validationElement.Locks);

                    HandleReceipts(validates
                        ? validationElement.Accepted?.Receipts
                        : validationElement.Declined?.Receipts);
                }

                Refresh();
            } 
        }
        
        /// <summary>
        /// Handles clicks on the Back Button, trying to show the last dialog or elsewhise loading the last process-step.
        /// </summary>
        private void Back_Click(object sender, RoutedEventArgs e)
        {
            if (TryShowLastDialog(_entries)) return;
            if (((string) BtnNext.Content).Equals("Änderungen Speichern")) BtnNext.Content = "Weiter";

            if (_steps.Current != null) 
                _dialogs = _steps.Current.Dialogs;
            TryShowLastDialog(_entries);
        }

        /// <summary>
        /// Fill out a list of receipts (*.docx)  and save them to the disk
        /// </summary>
        /// <param name="receipts"></param>
        private void HandleReceipts(IEnumerable<ReceiptElement> receipts)
        {
            if (receipts == null) return;
            
            foreach (var receipt in receipts)
                HandleReceipt(receipt);
        }
        
        /// <summary>
        /// Retrieve the receipt from the Server, fill it with values for the current receipt and save it to the disk
        /// </summary>
        /// <param name="receipt"></param>
        private void HandleReceipt(ReceiptElement receipt)
        {
            var fileName = $"{receipt.DraftName}_{DateTime.Now.ToShortDateString()}.docx";
            var documentTemplate = _networkHelper.GetDocTemplate(receipt.DraftName);
            File.WriteAllBytes(fileName, Encoding.Default.GetBytes(documentTemplate.Content));

            var replacements = new List<KeyValuePair<string, string>>();

            foreach (var entry in _entries)
                replacements.Add(
                    new KeyValuePair<string, string>($"[{entry.FieldName}]", entry.Data));

            WordReceiptHelper.OpenDocument(fileName, replacements.ToArray());
        }
        
        /// <summary>
        /// Submits any Data in the current process-step to the server
        /// </summary>
        /// <returns></returns>
        private void SendData()
        {
            if (_steps.Current == null) return;
            
            for (var i = 0; i < _steps.Current.DialogCount; i++)
            {
                var dialog = _steps.Current.GetDialogAtIndex(i);
                for (var j = 0; j < dialog.ElementCount; j++)
                {
                    var element = dialog.GetElementAtIndex(j);
                    var entry = new Entry
                    {
                        InstanceId = SelectedInstance.Id,
                        FieldName = element.Name,
                        Data = element.GetFormattedValue()
                    };

                    _networkHelper.CreateEntry(entry);
                }
            }
        }
        
        /// <summary>
        /// Returns whether showing the next dialog was a success
        /// </summary>
        /// <returns></returns>
        private bool TryShowNextDialog(IReadOnlyCollection<Entry> entries)
        {
            if (!_dialogs?.MoveNext() ?? true) return false;
            ShowCurrentDialog(entries);
            return true;
        }

        /// <summary>
        /// Returns whether showing the last dialog was a success
        /// </summary>
        /// <returns></returns>
        private bool TryShowLastDialog(IReadOnlyCollection<Entry> entries)
        {
            if (!_dialogs?.MoveBack() ?? true) return false;
            ShowCurrentDialog(entries);
            return true;
        }

        /// <summary>
        /// Displays the current dialog with its entries (retrieved from the server) in the UI
        /// </summary>
        /// <param name="entries"></param>
        private void ShowCurrentDialog(IReadOnlyCollection<Entry> entries)
        {
            if (_dialogs.Current == null) return;
            
            foreach (var uiElement in _dialogs.Current.Elements)
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
                        $"Für das Element \"{uiElement.Name}\" konnte kein Wert gefunden werden. Das Formular ist möglicherweise beschädigt.");
                }
            }
            
            ViewHolder.Content = _dialogs.Current.GetStackPanel();
        }

        /// <summary>
        /// Handless selection changes on the instance list, loading a new processobject from the server,
        /// and showing the first dialog.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
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
                    _curProcObj = XmlHelper.ReadXmlFromString(processText);
                    _steps = _curProcObj.Steps;
                    if (_steps?.MoveNext() ?? false)
                    {
                        _dialogs = _steps.Current?.Dialogs;
                        TryShowNextDialog(_entries);
                    }
                }
            }
            catch (Exception)
            {
                InfoPopup.ShowOk(StringResources.LoadPendingError);
            }
        }

        private void Archived_OnChecked(object sender, RoutedEventArgs e)
        {
            InstanceList.ItemsSource = null;
            Refresh();
        }

        private void Running_OnChecked(object sender, RoutedEventArgs e)
        {
            InstanceList.ItemsSource = null;
            Refresh();
        }
    }
}