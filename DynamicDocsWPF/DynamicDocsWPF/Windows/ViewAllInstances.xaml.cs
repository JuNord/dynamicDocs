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
    public partial class ViewAllInstances : Window
    {
        private readonly NetworkHelper _networkHelper;
        private List<ProcessInstance> _instances;
        private ProcessObject _currentProcessObject;
        private ProcessStep _firstProcessStep;
        private IEnumerator<Dialog> _dialogEnumerator;
        private List<Entry> _entries;
        
        private ProcessInstance SelectedInstance => ((ProcessInstance) InstanceList.SelectedItem);

        public ViewAllInstances(NetworkHelper networkHelper)
        {
            _networkHelper = networkHelper;
            InitializeComponent();
            _instances = _networkHelper.GetProcessInstances();
            InstanceList.ItemsSource = _instances;
        }

        private void ViewAllInstances_Btn_Next_OnClick(object sender, RoutedEventArgs e)
        {
            if (_dialogEnumerator?.MoveNext() ?? false)  
            {
                foreach (var uiElement in _dialogEnumerator.Current.Elements)
                {
                    FillUiElement(uiElement);
                }
                ViewCreator.FillViewHolder(ViewHolder, _dialogEnumerator.Current);
            }
        }

        private void InstanceList_OnSelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            _entries = _networkHelper.GetEntries(SelectedInstance.Id);
            
            var processText = _networkHelper.GetProcessTemplate(SelectedInstance.TemplateId);
            _currentProcessObject = XmlHelper.ReadXMLFromString(processText);
            _firstProcessStep = _currentProcessObject.GetStepAtIndex(0);
            
            _dialogEnumerator = _firstProcessStep?.Dialogs.GetEnumerator();

            if(_dialogEnumerator?.MoveNext()??false){
                foreach (var uiElement in _dialogEnumerator.Current.Elements)
                {
                    FillUiElement(uiElement);
                }
            }

            ViewCreator.FillViewHolder(ViewHolder, _dialogEnumerator?.Current);
        }

        private void FillUiElement(BaseInputElement uiElement)
        {
            try
            {
                var result = _entries.First(entry => entry.FieldName.Equals(uiElement.Name));
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
