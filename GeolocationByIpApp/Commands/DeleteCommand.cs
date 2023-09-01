using System;
using System.Windows.Input;
using GeolocationApp.Model;
using GeolocationApp.ViewModel;
using static GeolocationApp.Model.Enums;


namespace GeolocationApp.Commands
{
    public class DeleteCommand : ICommand
    {
        public DeleteCommand(GeolocationViewModel vm)
        {
            CommandIpViewModel = vm;
        }

        public GeolocationViewModel CommandIpViewModel { get; set; }

        public event EventHandler CanExecuteChanged
        {
            add => CommandManager.RequerySuggested += value;
            remove => CommandManager.RequerySuggested -= value;
        }

        public bool CanExecute(object parameter)
        {
            return  CommandIpViewModel.HealthStatesModel.AreAllServicesSuccessful() &&
                    CommandIpViewModel.IpDomainSearchModel.EntryDataType != EntryType.None &&
                   !CommandIpViewModel.ListNotInApi.Contains(CommandIpViewModel.IpDomainSearchModel.Id) &&
                   !string.IsNullOrEmpty(CommandIpViewModel.IpadressModel.Id);
        }

        public void Execute(object parameter)
        {
            CommandIpViewModel.DeleteModels();
        }
    }
}