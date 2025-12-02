using OxyPlot;
using OxyPlot.Series;
using ShadowScope.Resources.Code;
using System.ComponentModel;
using System.Windows;


namespace ShadowScope
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window, INotifyPropertyChanged
    {
        private double _progressValue;
        public event PropertyChangedEventHandler PropertyChanged;
        public double ProgressValue
        {
            get => _progressValue;
            set
            {
                if (_progressValue != value)
                {
                    _progressValue = value;
                    PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(ProgressValue)));
                }
            }
        }
        public MainWindow()
        {
            InitializeComponent();
            ExtraInitialize();
        }

        /// <summary>
        /// Дополнительная инициализация компонентов окна
        /// </summary>
        private void ExtraInitialize()
        {
            comboBox.ItemsSource = new string[] { "Равномерное", "Нормальное", "Рэлея" };
        }

        /// <summary>
        /// Нажатие кнопки
        /// </summary>
        private async void Button_Click(object sender, RoutedEventArgs e)
        {
            // 1. Считывание и валидация параметров
            LightPlane.Thickness = ValidateInput(0.0001, 1000, textBox_Толщина.Text);
            LightPlane.Angle = ValidateInput(0, 180, textBox_Угол.Text);
            LightPlane.DistanceToScreen = ValidateInput(0, 10000, textBox_Расстояние_до_экрана.Text);

            Balls.Radius = ValidateInput(0.0001, 1000, textBox_Диаметр.Text);
            Balls.Speed = ValidateInput(0.0001, 3000000, textBox_Скорость.Text);
            Balls.Count = (int)ValidateInput(1, 1000000, textBox_Количество.Text);

            Physics.Distribution_Type = comboBox.SelectedIndex switch
            {
                0 => DistributionType.Uniform,
                1 => DistributionType.Normal,
                2 => DistributionType.Rayleigh,
                _ => DistributionType.Uniform
            };

            // 2. Сброс прогресса
            ProgressValue = 0;

            // 3. Инициализация физики и плоскости
            Physics.InitializePhysics();
            LightPlane.CalculatePosition();
            Physics.ResetPhysics();

            int totalSteps = (int)Physics.Time;

            // 4. Запуск расчёта физики в фоне
            await Task.Run(() =>
            {
                Physics.Start(progress =>
                {
                    // Обновляем ProgressBar не каждый шаг, а каждые 1000 итераций для плавности
                    if (progress % Math.Max(1, totalSteps / 1000) == 0)
                        Dispatcher.Invoke(() => ProgressValue = progress);
                });
            });

            // 5. Построение графика после завершения расчетов
            DrawShadowGraph();
        }
        /// <summary>
        /// Проверяет, что указанная строка ввода представляет числовое значение в заданном диапазоне.
        /// </summary>
        /// <remarks>Если ввод равен null, пустой, не является допустимым числом или выходит за пределы указанного диапазона,
        /// пользователю отображается сообщение об ошибке, и возвращается <see cref="double.NaN"/>.</remarks>
        /// <param name="min">Минимально допустимое значение для ввода. Разобранное число должно быть больше или равно этому значению.</param>
        /// <param name="max">Максимально допустимое значение для ввода. Разобранное число должно быть меньше или равно этому значению.</param>
        /// <param name="input">Строка ввода для проверки. Должна представлять числовое значение в указанном диапазоне.</param>
        /// <returns>Разобранное значение типа double, если ввод корректен и находится в указанном диапазоне; в противном случае <see cref="double.NaN"/>.</returns>
        private double ValidateInput(double min, double max, string input)
        {
            if (input == null || input == "")
            {
                MessageBox.Show("Пожалуйста, введите значение.", "Ошибка!", MessageBoxButton.OK, MessageBoxImage.Error);
                return double.NaN;
            }
            if (double.TryParse(input, out double value))
            {
                if (value < min || value > max)
                {
                    MessageBox.Show($"Значение должно быть в диапазоне от {min} до {max}.", "Ошибка!", MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }
            else
            {
                MessageBox.Show("Пожалуйста, введите корректное числовое значение.", "Ошибка!", MessageBoxButton.OK, MessageBoxImage.Error);
            }
            return value;
        }

        private void DrawShadowGraph()
        {
            var plotModel = new PlotModel { Title = "Площадь тени" };
            var series = new LineSeries
            {
                Title = "Тень",
                StrokeThickness = 2,
                LineStyle = LineStyle.Solid,
                MarkerType = MarkerType.None
                
            };

            for (int i = 0; i < Physics.SumArea.Length; i++)
                series.Points.Add(new DataPoint(i, Physics.SumArea[i]));

            plotModel.Series.Add(series);

            plotModel.Axes.Add(new OxyPlot.Axes.LinearAxis
            {
                Position = OxyPlot.Axes.AxisPosition.Bottom,
                Title = "Время"
            });
            plotModel.Axes.Add(new OxyPlot.Axes.LinearAxis
            {
                Position = OxyPlot.Axes.AxisPosition.Left,
                Title = "Площадь тени"
            });

            shadowPlot.Model = plotModel;
        }

    }
}