using System;
using System.Diagnostics;

namespace ShadowScope.Resources.Code
{
    /// <summary>
    /// Класс физики
    /// </summary>
    internal static class Physics
    {
        // TODO: Оптимизация физики для больших количеств шаров (100k+)
        // TODO: Параллельные вычисления
        // Свойства системы
        /// <summary>
        /// Задает тип распределения шаров
        /// </summary>
        public static DistributionType Distribution_Type { get; internal set; }
        /// <summary>
        /// Малый шаг времени для расчета физики
        /// </summary>
        public const double DTime = 1;
        /// <summary>
        /// Получает время моделирования
        /// </summary>
        public static double Time { get; internal set; }
        /// <summary>
        /// Задает площадь в экране на каждом шаге моделирования
        /// </summary>
        public static Dictionary<double, double> SumArea { get; internal set; } // Время - Площадь
        /// <summary>
        /// Представляет массив шаров
        /// </summary>
        public static List<Ball> Balls_Array { get; internal set; }
        /// <summary>
        /// Определяет позицию генерации шаров
        /// </summary>
        private static Point[] Position_Generator { get; set; }
        private static Random? Random;

        // Методы физики
        /// <summary>
        /// Метод инициализации физики
        /// </summary>
        /// <param name="position_Generator">4 Координаты генератора шаров.</param>
        public static void InitializePhysics()
        {
            Random = new Random();
            Position_Generator = new Point[4] { new Point(0, 0), new Point(0, 1000), new Point(Balls.Count * 100, 1000), new Point(Balls.Count * 100, 0) };
            Balls_Array = new List<Ball>();
            Time = Balls.Count * 100 + LightPlane.DistanceToScreen + LightPlane.Thickness + 10;   // Время равно расстоянию от генератора до экрана + расстояние от экрана до тени + запас
            SumArea = new Dictionary<double, double>();
        }
        public static void Start(Action<double> reportProgress)
        {
            double progress = 0;
            SpawnBalls(Distribution_Type);
            for (double i = 0; i < Time; i+= DTime)
            {
                double currentArea = 0.0;
                foreach (var ball in Balls_Array)
                {
                    // Проверяем, пересек ли шар плоскость света
                    if (ball.IntersectsPlaneX())
                    {
                        // Добавляем площадь шара к текущей площади
                        currentArea += ball.AreaDt;
                    }
                    // Двигаем шар
                    ball.MoveBall(DTime);
                }
                // Сохраняем площадь на текущий момент времени
                SumArea[i] = currentArea;
                // Отчет о прогрессе
                progress += 1;
                reportProgress?.Invoke(progress);
            }
        }
        /// <summary>
        /// Метод сброса физической системы
        /// </summary>
        public static void ResetPhysics()
        {
            Time = Balls.Count * 100 + LightPlane.DistanceToScreen + LightPlane.Thickness + 10;   // Время равно расстоянию от генератора до экрана + расстояние от экрана до тени + запас
            SumArea.Clear();
            Balls_Array.Clear();
        }
        /// <summary>
        /// Метод генерации шаров
        /// </summary>
        public static void SpawnBalls(DistributionType type)
        {
            Balls_Array.Clear();

            for (int i = 0; i < Balls.Count; i++)
            {
                double rx = Sample(type);  // 0..1
                double ry = Sample(type);  // 0..1

                // Преобразуем в нужный диапазон
                double x = Lerp(Position_Generator[0].X, Position_Generator[2].X, rx);
                double y = Lerp(Position_Generator[3].Y, Position_Generator[2].Y, ry);

                Balls_Array.Add(new Ball(Balls.Radius, Balls.Speed, new Point((int)x, (int)y)));
            }
        }
        /// <summary>
        /// Метод линейной интерполяции
        /// </summary>
        /// <param name="min">Минимум</param>
        /// <param name="max">Максимум</param>
        /// <param name="t">Коэфициент</param>
        /// <returns></returns>
        private static double Lerp(double min, double max, double t)
        {
            return min + (max - min) * t;
        }
        /// <summary>
        /// Метод выборки распределения
        /// </summary>
        /// <param name="type">Тип распределения</param>
        /// <returns>Число от 0.0 до 1.0</returns>
        private static double Sample(DistributionType type)
        {
            return type switch
            {
                DistributionType.Uniform => UniformDistribution(),
                DistributionType.Normal => NormalDistribution(),
                DistributionType.Rayleigh => RayleighDistribution(),

                _ => UniformDistribution()
            };
        }

        // Методы распределений
        /// <summary>
        /// Метод равномерного распределения
        /// </summary>
        /// <returns>Возвращает значение в диапазоне от 0 до 1</returns>
        public static double UniformDistribution()
        {
            return Random.NextDouble();
        }
        /// <summary>
        /// Метод нормального распределения
        /// </summary>
        /// <returns>Возвращает значение в диапазоне от 0 до 1</returns>
        public static double NormalDistribution()
        {
            double u1 = 1.0 - Random.NextDouble();
            double u2 = 1.0 - Random.NextDouble();
            double normal = Math.Sqrt(-2.0 * Math.Log(u1)) * Math.Cos(2 * Math.PI * u2);

            // Масштабируем в диапазон 0..1
            double t = 0.5 + normal * 0.15;

            // Ограничиваем
            return Math.Clamp(t, 0.0, 1.0);
        }
        /// <summary>
        /// Метод распределения Рэлея
        /// </summary>
        /// <returns>Возвращает значение в диапазоне от 0 до 1</returns>
        public static double RayleighDistribution()
        {
            double u = Random.NextDouble();
            double sigma = 0.3;

            double r = sigma * Math.Sqrt(-2.0 * Math.Log(1 - u));

            // нормализация в 0..1
            return Math.Clamp(r, 0.0, 1.0);
        }
    }
}
