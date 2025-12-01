using ShadowModel.Resources.Code;

namespace ShadowScope.Resources.Code
{
    /// <summary>
    /// Класс физики
    /// </summary>
    internal class Physics
    {
        // Свойства шара
        /// <summary>
        /// Задает количество шаров
        /// </summary>
        public static int Count { get; internal set; }

        // Свойства системы
        /// <summary>
        /// Задает тип распределения шаров
        /// </summary>
        public static DistributionType Distro { get; internal set; }
        /// <summary>
        /// Малый шаг времени для расчета физики
        /// </summary>
        public const double DTime = 0.01;
        /// <summary>
        /// Получает время моделирования
        /// </summary>
        public static double Time { get; internal set; }
        /// <summary>
        /// Задает площадь в экране на каждом шаге моделирования
        /// </summary>
        public static Dictionary<double, double> SumArea { get; internal set; }
        /// <summary>
        /// Определяет позицию генерации шаров
        /// </summary>
        private static Point[] Position { get; set; }
    }
}
