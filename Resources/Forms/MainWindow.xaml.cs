using Microsoft.Win32;
using OxyPlot;
using OxyPlot.Axes;
using OxyPlot.Series;
using ShadowScope.Resources.Code;
using System.ComponentModel;
using System.IO;
using System.Text.Json;
using System.Windows;

namespace ShadowScope
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window, INotifyPropertyChanged
    {
        private double _progressValue;  // Значение прогресса для ProgressBar
        public event PropertyChangedEventHandler PropertyChanged;   // Событие для уведомления об изменении свойства

        /// <summary>
        /// Представляет собой значение прогресса для ProgressBar.
        /// </summary>
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

        public MainWindow() // Конструктор главного окна
        {
            InitializeComponent();
            DataContext = this;
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
            GlobalCheck(); // Глобальная проверка и установка параметров

            // Инициализация физики и плоскости
            Physics.InitializePhysics();
            LightPlane.CalculatePosition();
            Physics.ResetPhysics();

            // Проверка времени симуляции
            if (Physics.Time > 10000)
            {
                var result = MessageBox.Show(
                    $"Время симуляции очень велико ({Physics.Time:0}).\n" +
                    "Расчёт может занять продолжительное время.\n\n" +
                    "Продолжить?",
                    "Предупреждение",
                    MessageBoxButton.YesNo,
                    MessageBoxImage.Warning
                );

                if (result == MessageBoxResult.No)
                    return; // Прерываем выполнение
            }

            int totalSteps = (int)Physics.Time;

            // Запуск расчёта физики в фоне(асинхронно)
            await Task.Run(() =>
            {
                Physics.Start(progress =>
                {
                    if (progress % Math.Max(1, totalSteps / 1000) == 0)
                        Dispatcher.Invoke(() => ProgressValue = progress);
                });
            });

            DrawShadowGraph();  // Отрисовка графика тени
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
            if (string.IsNullOrWhiteSpace(input))
            {
                MessageBox.Show("Пожалуйста, введите значение.", "Ошибка!", MessageBoxButton.OK, MessageBoxImage.Error);
                return double.NaN;
            }

            input = input.Replace('.', ',');

            if (!double.TryParse(input, out double value))
            {
                MessageBox.Show("Пожалуйста, введите корректное числовое значение.", "Ошибка!", MessageBoxButton.OK, MessageBoxImage.Error);
                return double.NaN;
            }

            if (value < min || value > max)
            {
                MessageBox.Show($"Значение должно быть в диапазоне от {min} до {max}.", "Ошибка!", MessageBoxButton.OK, MessageBoxImage.Error);
                return double.NaN;
            }

            return value;
        }


        /// <summary>
        /// Метод для отрисовки графика площади тени.
        /// </summary>
        /// <remarks>Используется библиотека OxyPlot для построения графика.</remarks>
        private void DrawShadowGraph()
        {
            var plotModel = new PlotModel { Title = "Площадь тени" };   // Создание модели графика с заголовком
            var series = new LineSeries     // Создание серии данных для графика
            {
                Title = "Тень",
                StrokeThickness = 2,
                LineStyle = LineStyle.Solid,
                MarkerType = MarkerType.None,
                Color = OxyColors.Blue
            };

            for (int i = 0; i < Physics.SumArea.Length; i++)
                series.Points.Add(new DataPoint(i + Math.Round(LightPlane.DistanceToScreen/Balls.Speed, 3), Math.Round(Physics.SumArea[i], 3)));    // Добавление точек данных в серию

            plotModel.Series.Add(series);   // Добавление серии в модель графика

            // Настройка осей графика
            plotModel.Axes.Add(new LinearAxis
            {
                Position = AxisPosition.Bottom,
                Title = "Время, с"
            });

            if (CheckBox.IsChecked == false)
            {
                plotModel.Axes.Add(new LinearAxis   // Линейная ось Y
                {
                    Position = AxisPosition.Left,
                    Title = "Площадь тени м²"
                });
            }
            else
            {
                plotModel.Axes.Add(new LogarithmicAxis  // Логарифмическая ось Y
                {
                    Position = AxisPosition.Left,
                    Title = "Площадь тени (log), м²"
                });
            }
            shadowPlot.Model = plotModel;   // Установка модели графика в элемент управления
            ProgressValue = 100; // Установка прогресса на 100% после завершения отрисовки графика
        }

        private void CheckBox_Checked(object sender, RoutedEventArgs e)
        {
            GlobalCheck();
            DrawShadowGraph();
        }

        private void CheckBox_Unchecked(object sender, RoutedEventArgs e)
        {
            CheckBox_Checked(sender, e);
        }

        private void GlobalCheck()
        {
            // Считывание и валидация параметров
            double thickness = ValidateInput(0.0001, 1000000, textBox_Толщина.Text);
            double angle = ValidateInput(-90, 90, textBox_Угол.Text);
            double dist = ValidateInput(0, 100000, textBox_Расстояние_до_экрана.Text);

            double radius = ValidateInput(0.0001, 1000, textBox_Диаметр.Text);
            double speed = ValidateInput(0.0001, 3000000, textBox_Скорость.Text);
            double count = ValidateInput(1, 1000000, textBox_Количество.Text);

            if (double.IsNaN(thickness) ||
                double.IsNaN(angle) ||
                double.IsNaN(dist) ||
                double.IsNaN(radius) ||
                double.IsNaN(speed) ||
                double.IsNaN(count))
            {
                MessageBox.Show("Проверьте корректность введённых данных.",
                                "Ошибка!",
                                MessageBoxButton.OK,
                                MessageBoxImage.Error);
                return;
            }

            // Всё валидно — записываем
            LightPlane.Thickness = thickness;
            LightPlane.Angle = angle;
            LightPlane.DistanceToScreen = dist;

            Balls.Radius = radius;
            Balls.Speed = speed;
            Balls.Count = (int)count;

            Physics.Distribution_Type = comboBox.SelectedIndex switch
            {
                0 => DistributionType.Uniform,
                1 => DistributionType.Normal,
                2 => DistributionType.Rayleigh,
                _ => DistributionType.Uniform
            };
        }
    }
}