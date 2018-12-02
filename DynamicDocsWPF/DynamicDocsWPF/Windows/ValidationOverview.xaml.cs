using System;
using System.Collections.Generic;
using System.Windows;
using DynamicDocsWPF.HelperClasses;
using RestService.Model.Database;
using RestService.Model.Input;
using RestService.Model.Process;
using Process = RestService.Model.Process.Process;

namespace DynamicDocsWPF.Windows
{
    public partial class ValidationOverview : Window
    {
        private readonly int _instanceId;
        private Process _process;
        private readonly NetworkHelper _networkHelper;
        private int _stepIndex;
        private IEnumerator<Dialog> _dialogEnumerator;
        private readonly IEnumerator<ProcessStep> _processStepEnumerator;
        
        public ValidationOverview(int instanceId, Process process, NetworkHelper networkHelper)
        {
            _instanceId = instanceId;
            _process = process;
            _networkHelper = networkHelper;
            InitializeComponent();

            _processStepEnumerator = process.Steps.GetEnumerator();
            _processStepEnumerator.MoveNext();
            _dialogEnumerator = _processStepEnumerator.Current?.Dialogs.GetEnumerator();
            _dialogEnumerator?.MoveNext();
            ViewCreator.FillViewHolder(ViewHolder, _dialogEnumerator?.Current);
        }
        
        private void ButtonBase_OnClick(object sender, RoutedEventArgs e)
        {
            _stepIndex++;
            if (!_dialogEnumerator.MoveNext())
            {
                if (!_processStepEnumerator.MoveNext() || _stepIndex > _process.CurrentStep)
                {
                    var popup = new ValidationPopup();
                    popup.ShowDialog();
                    switch (popup.DialogResult)
                    {
                        case true:
                            _networkHelper.PostProcessUpdate(_instanceId, false);
                            break;
                        default:
                            _networkHelper.PostProcessUpdate(_instanceId, true);
                            break;
                    }             
                    Close();
                    return;
                }
                else
                {
                    _dialogEnumerator = _processStepEnumerator.Current?.Dialogs.GetEnumerator();
                }
            }
            else
            {
                ViewCreator.FillViewHolder(ViewHolder, _dialogEnumerator.Current);
            }
        }

    }
}
