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
            // Считывание и валидация параметров
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
            // Сброс прогресса
            ProgressValue = 0;

            Physics.InitializePhysics();
            LightPlane.CalculatePosition();
            Physics.ResetPhysics();
            // Запуск физики в фоне
            await Task.Run(() =>
            {
                Physics.Start(value =>
                {
                    // Обновление ProgressBar в UI-потоке
                    Dispatcher.Invoke(() => ProgressValue = value);
                });
            });

            // После завершения расчетов отрисовать график
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
            // Создаём модель графика
            var plotModel = new PlotModel { Title = "Площадь тени" };

            // Создаём серию точек
            var series = new LineSeries
            {
                Title = "Тень",
                StrokeThickness = 2,
                LineStyle = LineStyle.Solid,
                MarkerType = MarkerType.None // <-- никаких точек
            };

            // Словарь: ключ = радиус, значение = площадь тени
            Dictionary<double, double> shadowDict = Physics.SumArea;

            foreach (var kvp in shadowDict)
                series.Points.Add(new DataPoint(kvp.Key, kvp.Value));

            plotModel.Series.Add(series);

            // Названия осей
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