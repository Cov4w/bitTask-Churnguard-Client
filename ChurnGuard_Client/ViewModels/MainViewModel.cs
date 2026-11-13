using ChurnGuard_Client.Models;
using ChurnGuard_Client.Services;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Windows;
using System.Windows.Input;

namespace ChurnGuard_Client.ViewModels
{
    internal class MainViewModel : INotifyPropertyChanged
    {
        private readonly ApiService api = new();

        public event PropertyChangedEventHandler? PropertyChanged;

        public ObservableCollection<ModelMetric> PerformanceResults { get; set; } = new();

        public ICommand PredictCommand { get; }

        public MainViewModel()
        {
            PredictCommand = new RelayCommand(async _ => await getBasicScore());
        }

        private async Task getBasicScore()
        {
            try
            {
                MessageBox.Show("모델별 성능 비교 결과를 요청합니다..");
                var response = await api.LoadBasicPerformance();
                if (response !=null)
                {
                    PerformanceResults.Clear();
                    foreach (var item in response)
                    {
                        PerformanceResults.Add(item);
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        public class RelayCommand : ICommand
        {
            private readonly Action<object?> _execute;
            private readonly Predicate<object?>? _canExecute;

            public RelayCommand(Action<object?> execute, Predicate<object?>? canExecute = null)
            {
                _execute = execute;
                _canExecute = canExecute;
            }

            public bool CanExecute(object? parameter) => _canExecute?.Invoke(parameter) ?? true;
            public void Execute(object? parameter) => _execute(parameter);
            public event EventHandler? CanExecuteChanged
            {
                add => CommandManager.RequerySuggested += value;
                remove => CommandManager.RequerySuggested -= value;
            }
        }
    }
}
