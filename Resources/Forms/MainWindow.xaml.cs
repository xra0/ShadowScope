using ShadowScope.Resources.Code;
using System.Windows;

namespace ShadowScope
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
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
        private void Button_Click(object sender, RoutedEventArgs e)
        {
            Physics.Thickness = ValidateInput(0.0001, 1000, textBox_Толщина.Text);
            Physics.Angle = ValidateInput(0, 180, textBox_Угол.Text);
            Physics.Radius = ValidateInput(0.0001, 1000, textBox_Диаметр.Text);
            Physics.DistanceToScreen = ValidateInput(0, 10000, textBox_Расстояние_до_экрана.Text);
            Physics.Speed = ValidateInput(0.0001, 3000000, textBox_Скорость.Text);
            Physics.Count = (int)ValidateInput(1, 1000000, textBox_Количество.Text);
            Physics.Distro = comboBox.SelectedIndex switch
            {
                0 => DistributionType.Uniform,
                1 => DistributionType.Normal,
                2 => DistributionType.Rayleigh,
                _ => DistributionType.Uniform
            };
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
    }
}