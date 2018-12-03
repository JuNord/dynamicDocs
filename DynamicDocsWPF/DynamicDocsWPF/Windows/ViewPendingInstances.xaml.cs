using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using DynamicDocsWPF.HelperClasses;
using RestService;
using RestService.Model.Database;
using RestService.Model.Input;
using RestService.Model.Process;

namespace DynamicDocsWPF.Windows
{
    public partial class ViewPendingInstances : Window
    {
        private readonly NetworkHelper _networkHelper;
        private ProcessObject _currentProcessObject;
        private CustomEnumerable<ProcessStep> _processSteps;
        private CustomEnumerable<Dialog> _dialogs;
        private List<Entry> _entries;
        
        private ProcessInstance SelectedInstance => ((ProcessInstance) InstanceList.SelectedItem);

        public ViewPendingInstances(NetworkHelper networkHelper)
        {
            _networkHelper = networkHelper;
            InitializeComponent();
            InstanceList.ItemsSource = _networkHelper.GetResponsibilities();
        }

        private void ViewAllInstances_Btn_Next_OnClick(object sender, RoutedEventArgs e)
        {
            if (TryShowNextDialog(_entries)) return;
            if (!(_processSteps?.MoveNext() ?? false))
            {
                BtnNext.Content = "Änderungen Speichern";
                return;
            };
            
            _dialogs = _processSteps.Current.Dialogs;
            TryShowNextDialog(_entries);
        }

        private void ViewAllInstances_Btn_Back_OnClick(object sender, RoutedEventArgs e)
        {
            if (TryShowLastDialog(_entries)) return;
            if (((string)BtnNext.Content).Equals("Änderungen Speichern"))
            {
                BtnNext.Content = "Weiter";
            }
            
            _dialogs = _processSteps.Current.Dialogs;
            TryShowNextDialog(_entries);
        }

        private bool TryShowNextDialog(List<Entry> entries)
        {
            if (!(_dialogs?.MoveNext() ?? false)) return false;
            ShowCurrentDialog(entries);
            return true;
        }
        
        private bool TryShowLastDialog(List<Entry> entries)
        {
            if (!(_dialogs?.MoveNext() ?? false)) return false;
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
            _entries = _networkHelper.GetEntries(SelectedInstance.Id);
            
            var processText = _networkHelper.GetProcessTemplate(SelectedInstance.TemplateId);
            _currentProcessObject = XmlHelper.ReadXMLFromString(processText);
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
