using System;
using System.Windows;
using DynamicDocsWPF.HelperClasses;
using RestService.Model.Database;
using Process = RestService.Model.Process.Process;

namespace DynamicDocsWPF.Windows
{
    public partial class ValidationOverview : Window
    {
        private Process _process;
        private User _user;
        private int dialogIndex;
        private int stepIndex;
        
        public ValidationOverview(Process process, User user)
        {
            _process = process;
            _user = user;
            InitializeComponent();

           
            var dialog = process.GetStepAtIndex(0).GetDialogAtIndex(0);
            ViewCreator.FillViewHolder(ViewHolder, dialog);
        }
        
        private void ButtonBase_OnClick(object sender, RoutedEventArgs e)
        {
            dialogIndex++;
            if (dialogIndex >= _process.GetStepAtIndex(stepIndex).DialogCount)
            {
                dialogIndex = 0;
                stepIndex++;
                if (stepIndex >= Math.Min(_process.ProcessStepCount, _process.CurrentStep))
                {
                    var popup = new ValidationPopup();
                    popup.ShowDialog();
                    if (popup.DialogResult??false)
                    {
                        var networkHelper = new NetworkHelper("http://localhost:8000/", _user);
                      
                    }
                    else
                    {
                        
                    }
                   
                    Close();
                    return;
                }
            }
            if(_process.GetStepAtIndex(stepIndex).DialogCount > 0)
                ViewCreator.FillViewHolder(ViewHolder, _process.GetStepAtIndex(stepIndex).GetDialogAtIndex(dialogIndex));
        }

    }
}
