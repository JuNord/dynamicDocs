using System;
using System.Collections.Generic;
using System.Windows;
using DynamicDocsWPF.HelperClasses;
using RestService;
using RestService.Model.Database;
using RestService.Model.Input;
using RestService.Model.Process;

namespace DynamicDocsWPF.Windows
{
    public partial class ValidationOverview : Window
    {
        private readonly int _instanceId;
        private ProcessObject _processObject;
        private readonly NetworkHelper _networkHelper;
        private int _stepIndex;
        private CustomEnumerable<Dialog> _dialogEnumerable;
        private readonly CustomEnumerable<ProcessStep> _processStepEnumerator;
        
        public ValidationOverview(int instanceId, ProcessObject processObject, NetworkHelper networkHelper)
        {
            _instanceId = instanceId;
            _processObject = processObject;
            _networkHelper = networkHelper;
            InitializeComponent();

            _processStepEnumerator = processObject.Steps;
            _processStepEnumerator.MoveNext();
            _dialogEnumerable = _processStepEnumerator.Current?.Dialogs;
            _dialogEnumerable?.MoveNext();
            ViewCreator.FillViewHolder(ViewHolder, _dialogEnumerable?.Current);
        }
        
        private void Next_OnClick(object sender, RoutedEventArgs e)
        {
            _stepIndex++;
            if (!_dialogEnumerable.MoveNext())
            {
                if (!_processStepEnumerator.MoveNext() || _stepIndex > _processObject.CurrentStep)
                {
                    var popup = new ValidationPopup();
                    popup.ShowDialog();
                    switch (popup.DialogResult)
                    {
                        case true:
                            _networkHelper.PostProcessUpdate(_instanceId, false, false);
                            break;
                        default:
                            _networkHelper.PostProcessUpdate(_instanceId, true, false);
                            break;
                    }             
                    Close();
                    return;
                }
                else
                {
                    _dialogEnumerable = _processStepEnumerator.Current?.Dialogs;
                }
            }
            else
            {
                ViewCreator.FillViewHolder(ViewHolder, _dialogEnumerable.Current);
            }
        }

        private void Back_OnClick(object sender, RoutedEventArgs e)
        {
            _stepIndex--;
            if (!_dialogEnumerable.MoveBack())
            {
                if (!_processStepEnumerator.MoveBack() || _stepIndex > _processObject.CurrentStep)
                {
                    var popup = new ValidationPopup();
                    popup.ShowDialog();
                    switch (popup.DialogResult)
                    {
                        case true:
                            _networkHelper.PostProcessUpdate(_instanceId, false, false);
                            break;
                        default:
                            _networkHelper.PostProcessUpdate(_instanceId, true, false);
                            break;
                    }             
                    Close();
                    return;
                }
                else
                {
                    _dialogEnumerable = _processStepEnumerator.Current?.Dialogs;
                }
            }
            else
            {
                ViewCreator.FillViewHolder(ViewHolder, _dialogEnumerable.Current);
            }
        }
    }
}
