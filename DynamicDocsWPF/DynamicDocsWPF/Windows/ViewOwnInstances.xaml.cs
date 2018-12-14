using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Shapes;
using DynamicDocsWPF.HelperClasses;
using Microsoft.Office.Interop.Word;
using RestService;
using RestService.Model.Database;
using RestService.Model.Input;
using RestService.Model.Process;
using Dialog = RestService.Model.Input.Dialog;
using Window = System.Windows.Window;

namespace DynamicDocsWPF.Windows
{
    public partial class ViewOwnInstances :UserControl
    {
        private readonly MainWindow _mainWindow;
        private readonly NetworkHelper _networkHelper;
        private ProcessObject _currentProcessObject;
        private CustomEnumerable<Dialog> _dialogs;
        private List<Entry> _entries;
        
        private ProcessInstance SelectedInstance => ((ProcessInstance) InstanceList.SelectedItem);

        public ViewOwnInstances(MainWindow mainWindow, NetworkHelper networkHelper)
        {
            InitializeComponent();
            _mainWindow = mainWindow;
            _networkHelper = networkHelper;
            Refresh();
        }

        public void Refresh()
        {
            var list = TryGetInstances();
            if (list != null)
            {
                if (InstanceList?.ItemsSource != null)
                {
                    if (list.SequenceEqual((List<ProcessInstance>) InstanceList.ItemsSource)) return;
                }

                var toShow = new List<ProcessInstance>(
                    Running.IsChecked == true
                        ? list.Where(e => e.Archived == false)
                        : list.Where(e => e.Archived == true));
                InstanceList.ItemsSource = toShow;
            }
        }
        
        private List<ProcessInstance> TryGetInstances()
        {
            try
            {
                var processInstances = _networkHelper?.GetProcessInstances();

                return processInstances;
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
            if (_dialogs.Current.Elements.Any(baseInputElement => !IsInputValueValid(baseInputElement)))
                return;
            if (TryShowNextDialog(_entries)) return;

            if (!SelectedInstance.Locked)
            {
                var sendPopup = InfoPopup.ShowYesNo("Haben Sie Änderungen vorgenommen die gespeichert werden sollen?");

                if (!sendPopup) return;
                
                SendData();
                InfoPopup.ShowOk("Änderungen gespeichert.");
                Refresh();
                LoadAndShowSelection();
            }
        }

        private void Back_Click(object sender, RoutedEventArgs e)
        {
            if (((string)BtnNext.Content).Equals("Änderungen Speichern"))
            {
                BtnNext.Content = "Weiter";
            }

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
        
        private void NewInstance_Click(object sender, RoutedEventArgs e)
        {
            ProcessSelect processSelect = null;
            try
            {
                processSelect = new ProcessSelect(_networkHelper);

                if (processSelect.ShowDialog() == false) return;
                
                var file = _networkHelper.GetProcessTemplate(processSelect.SelectedProcessTemplate.Id);
                var process = XmlHelper.ReadXmlFromString(file);
                var newInstance = new CreateProcessInstance(process, _networkHelper);
                newInstance.ShowDialog();
                Refresh();
            }
            catch (WebException)
            {
                InfoPopup.ShowOk("Der Server ist derzeit nicht erreichbar.");
                processSelect?.Close();
            }
        }

        private void InstanceList_OnSelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            LoadAndShowSelection();
        }

        private bool IsInputValueValid(BaseInputElement baseInputElement)
        {
            if (baseInputElement.FulfillsObligatoryConditions())
            {
                if (!baseInputElement.FulfillsProcessConditions())
                {
                   _mainWindow.DisplayInfo(baseInputElement.ProcessErrorMsg);
                    baseInputElement.BaseControl.BorderBrush = new SolidColorBrush(Color.FromArgb(170, 255, 50, 50));
                    baseInputElement.BaseControl.BorderThickness = new Thickness(2);
                    return false;
                }

                if (!baseInputElement.FulfillsControlConditions())
                {
                    _mainWindow.DisplayInfo(baseInputElement.ControlErrorMsg);
                    baseInputElement.BaseControl.BorderBrush = new SolidColorBrush(Color.FromArgb(170, 255, 50, 50));
                    baseInputElement.BaseControl.BorderThickness = new Thickness(2);
                    return false;
                }

                _mainWindow.DisplayInfo(MainWindow.MoTD);
                baseInputElement.BaseControl.BorderBrush = new SolidColorBrush(Colors.Gray);
                baseInputElement.BaseControl.BorderThickness = new Thickness(1);
                return true;

            }
            else
            {
                _mainWindow.DisplayInfo("Bitte füllen Sie alle Muss Felder aus.");
                baseInputElement.BaseControl.BorderBrush = new SolidColorBrush(Color.FromArgb(170, 255, 50, 50));
                baseInputElement.BaseControl.BorderThickness = new Thickness(2);
                return false;
            }
        }
        
        private void LoadAndShowSelection()
        {
            ContentSection.Visibility = InstanceList.SelectedIndex == -1 ? Visibility.Collapsed : Visibility.Visible;

            if (SelectedInstance != null)
            {
                var processText = _networkHelper.GetProcessTemplate(SelectedInstance.TemplateId);
                _currentProcessObject = XmlHelper.ReadXmlFromString(processText);
                _dialogs = _currentProcessObject.GetStepAtIndex(0).Dialogs;
                _entries = _networkHelper.GetEntries(SelectedInstance.Id);
                TryShowNextDialog(_entries);

                ProgressPanel.Children.Clear();
                for (int i = 0; i < _currentProcessObject.StepCount; i++)
                {
                    Color color = Colors.Transparent;
                    int width = 10;

                    if (i < SelectedInstance.CurrentStep)
                    {
                        color = Colors.Green;
                    }
                    else if (i == SelectedInstance.CurrentStep)
                    {
                        color = SelectedInstance.Declined ? Colors.Red : Colors.White;
                        width = 15;
                    }
                    else if (i > SelectedInstance.CurrentStep)
                    {
                        color = Colors.LightGray;
                    }


                    
                    ProgressPanel.Children.Add(new Ellipse()
                    {
                        Width = width,
                        Height = width,
                        Fill = new SolidColorBrush(color),
                        Margin = new Thickness(10, 0, 10, 0)
                    });
                  
                    if (SelectedInstance.CurrentStep < _currentProcessObject.StepCount)
                    {
                        StepDescription.Text = _currentProcessObject.GetStepAtIndex(SelectedInstance.CurrentStep)
                            ?.Description;

                        if (SelectedInstance.Declined)
                            StepDescription.Text += " - Abgelehnt";
                    }
                    else StepDescription.Text = "Genehmigt.";
                }
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
                InfoPopup.ShowOk($"The process contained an element called \"{uiElement.Name}\" that couldnt be found.");
            }
        }
        
        private void SendData()
        {
            try
            {
                _dialogs.Reset();
                foreach (var dialog in _dialogs)
                {
                    foreach (var element in dialog.Elements)
                    {
                        var entry = _entries.FirstOrDefault(e => e.FieldName == element.Name);

                        if (entry != null)
                        {
                            var dataOld = entry.Data;
                            if (dataOld.Equals(element.GetFormattedValue())) continue;
                            entry.Data = element.GetFormattedValue();
                            _networkHelper.PostEntryUpdate(entry);
                        }
                        else
                        {
                            entry = new Entry()
                            {
                                InstanceId = SelectedInstance.Id,
                                FieldName = element.Name,
                                Data = element.GetFormattedValue()
                            };
                            _networkHelper.CreateEntry(entry);
                        }

                    }
                }             
            }
            catch (NullReferenceException e)
            {
                InfoPopup.ShowOk("Ups, da ist wohl etwas schief gelaufen. Bitte wenden Sie sich an einen Administrator.");
            }
        }

        private void Archived_OnChecked(object sender, RoutedEventArgs e)
        {
            Refresh();
        }

        private void Running_OnChecked(object sender, RoutedEventArgs e)
        {
            Refresh();
        }

        private void ButtonBase_OnClick(object sender, RoutedEventArgs e)
        {
            if (SelectedInstance != null)
            {
                _networkHelper.PostProcessUpdate(SelectedInstance.Id, true, true);
                Refresh();
            }
        }
    }
}
