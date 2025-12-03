using AsadorMoron.ViewModels.Base;
using System;
using System.Threading.Tasks;

namespace AsadorMoron.Interfaces
{
    public interface INavigationService
    {
        Task InitializeAsync();

        Task NavigateToAsync<TViewModel>() where TViewModel : ViewModelBase;

        Task NavigateToAsync<TViewModel>(object parameter) where TViewModel : ViewModelBase;

        Task NavigateToAsync(Type viewModelType);

        Task NavigateToAsyncWithoutMenu<TViewModel>() where TViewModel : ViewModelBase;

        Task NavigateToAsyncWithoutMenu<TViewModel>(object parameter) where TViewModel : ViewModelBase;

        Task NavigateToAsyncWithoutMenu(Type viewModelType);

        Task NavigateToAsyncMenu<TViewModel>() where TViewModel : ViewModelBase;

        Task NavigateToAsyncMenu<TViewModel>(object parameter) where TViewModel : ViewModelBase;

        Task NavigateToAsyncMenu(Type viewModelType);

        Task NavigateToAsyncMenu(Type viewModelType, object parameter);

        void InsertPageBefore<T>();

        Task NavigateBackAsync();

        Task LogOutAppAsync<TViewModel>(object parameter) where TViewModel : ViewModelBase;

        void LogOutApp(Type viewModelType, object parameter);
    }
}
