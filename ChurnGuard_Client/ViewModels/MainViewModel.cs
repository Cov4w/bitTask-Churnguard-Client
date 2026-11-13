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
        public TestFigure testFigure { get; set; } = new TestFigure();

        public event PropertyChangedEventHandler? PropertyChanged;

        #region 콤보박스 값 설정
        public List<double> logistic { get; } = new()
        {
            0.01, 0.1, 1, 10
        };
        public List<double> SGD_alpha { get; } = new()
        {
            0.0001, 0.001, 0.01
        };
        public List<int> dt_depth { get; } = new()
        {
            3, 5, 7, 10
        };
        public List<int> rf_n_estimators { get; } = new()
        {
            100, 200, 300
        };
        public List<double> gb_learning_rate { get; } = new()
        {
            0.01, 0.1, 0.2
        };
        #endregion
        public ObservableCollection<ModelMetric> PerformanceResults { get; set; } = new();
        public ICommand PredictCommand { get; }
        public ICommand ImprovementModel { get; }
        public ICommand FinalReportCommand { get; }

        public MainViewModel()
        {
            PredictCommand = new RelayCommand(async _ => await getBasicScore());
            ImprovementModel = new RelayCommand(async _ => await getImprovementScore());
            FinalReportCommand = new RelayCommand(async _ => await getFinalReport());
        }

        private string reportText = string.Empty;
        public string ReportText
        {
            get => reportText;
            set
            {
                reportText = value;
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(ReportText)));
            }
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

        private async Task getImprovementScore()
        {
            try
            {
                MessageBox.Show("입력된 수치의 테스트 결과를 요청합니다..");
                var response = await api.TestImprovement(testFigure);
                
                PerformanceResults.Clear();
                foreach(var item in response)
                {
                    PerformanceResults.Add(item);
                }
            }
            catch(Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        private async Task getFinalReport()
        {
            try
            {
                MessageBox.Show("GridSearchCV 탐색 및 최종 모델 선정 보고서를 요청합니다..\n(교차 검증 수행으로 몇 초 정도 소요될 수 있습니다)");

                var response = await api.LoadFinalReport();

                if (response != null && response.AllGridResults != null)
                {
                    // [핵심] 기존 DataGrid의 데이터를 비우고 GridSearchCV 최적 결과 5개를 추가
                    PerformanceResults.Clear();
                    foreach (var item in response.AllGridResults)
                    {
                        PerformanceResults.Add(item);
                    }

                    // 줄글로 된 최종 선정 보고서 출력 (메시지 박스 및 프로퍼티 업데이트)
                    ReportText = response.SummaryReport;
                    MessageBox.Show(response.SummaryReport, "최종 모델 선정 보고서");
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
