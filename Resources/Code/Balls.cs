namespace ShadowScope.Resources.Code
{
    /// <summary>
    /// Представляет структуру для управления свойствами шаров.
    /// </summary>
    internal static class Balls
    {
        // Свойства шаров
        private static double radius;  // Радиус шара
        private static double speed;   // Скорость шара
        private static int _count;     // Количество шаров

        /// <summary>
        /// Возвращает или задает, радиус шара.
        /// </summary>
        /// <remarks>Радиус должен быть больше нуля. Есть валидация.</remarks>
        public static double Radius
        {
            get { return radius; }
            set
            {
                if (value > 0)
                    radius = value;
            }
        }

        /// <summary>
        /// Представляет собой скорость шара.
        /// </summary>
        /// <remarks>Скорость должна быть больше нуля. Есть валидация.</remarks>
        public static double Speed
        {
            get { return speed; }
            set
            {
                if (value > 0)
                    speed = value;
            }
        }

        /// <summary>
        /// Представляет собой количество шаров.
        /// </summary>
        /// <remarks>Количество шаров должно быть не отрицательным. Есть валидация.</remarks>
        public static int Count
        {
            get
            {
                return _count;
            }
            set
            {
                if (value >= 0)
                    _count = value;
                else throw new ArgumentException("Число шаров должно быть не отрицательным.");
            }
        }

        public static Ball[] Array { get; private set; }    // Массив шаров
        private static Vec2[] Position_Generator;   // Позиции генератора шаров

        [ThreadStatic]
        private static Random threadRnd;    // Локальный экземпляр Random для каждого потока
        private static Random GlobalRnd = new();

        /// <summary>
        /// Метод инициализации физических параметров генератора шаров.
        /// </summary>
        public static void InitializeGenerator()
        {
            double length = Math.Sqrt(Count * 10 + 1);
            Position_Generator =
            [
                new Vec2(0,0),
                new Vec2(0,length),
                new Vec2(length,length),
                new Vec2(length,0)
            ];
            Array = new Ball[Balls.Count];
        }

        /// <summary>
        /// Метод генерации начальных позиций шаров в зависимости от выбранного типа распределения
        /// </summary>
        /// <param name="type">Тип распределения</param>
        public static void SpawnBalls(DistributionType type)
        {
            for (int i = 0; i < Balls.Count; i++)
            {
                double rx = Sample(type);   // 0..1
                double ry = Sample(type);   // 0..1

                double x = Lerp(Position_Generator[0].X, Position_Generator[2].X, rx);  // Линейная интерполяция по X
                double y = Lerp(Position_Generator[3].Y, Position_Generator[2].Y, ry);  // Линейная интерполяция по Y

                Array[i] = new Ball(Balls.Radius, Balls.Speed, new Vec2(x, y));
            }
        }

        /// <summary>
        /// Метод выборки случайного числа в диапазоне [0,1] в зависимости от типа распределения
        /// </summary>
        /// <param name="type">Тип распределения</param>
        /// <returns>Число в диапазоне [0,1] </returns>
        private static double Sample(DistributionType type)
        {
            return type switch
            {
                DistributionType.Uniform => GetRnd().NextDouble(),
                DistributionType.Normal => Normal(),
                DistributionType.Rayleigh => Rayleigh(),
                _ => GetRnd().NextDouble()
            };
        }

        /// <summary>
        /// Метод для получения экземпляра Random, уникального для каждого потока
        /// </summary>
        /// <returns>Экземпляр Random</returns>
        private static Random GetRnd()
        {
            return threadRnd ??= new Random(GlobalRnd.Next());
        }

        /// <summary>
        /// Метод для генерации случайного числа с нормальным распределением, усеченного до диапазона [0,1]
        /// </summary>
        /// <returns>Число в диапазоне [0,1]</returns>
        private static double Normal()
        {
            var rnd = GetRnd();
            double u1 = 1.0 - rnd.NextDouble();
            double u2 = 1.0 - rnd.NextDouble();
            double normal = Math.Sqrt(-2.0 * Math.Log(u1)) * Math.Cos(2 * Math.PI * u2);
            return Math.Clamp(0.5 + normal * 0.15, 0.0, 1.0);
        }

        /// <summary>
        /// Метод для генерации случайного числа с распределением Релея, усеченного до диапазона [0,1]
        /// </summary>
        /// <returns>Число в диапазоне [0,1]</returns>
        private static double Rayleigh()
        {
            var rnd = GetRnd();
            double u = rnd.NextDouble();
            double sigma = 0.3;
            return Math.Clamp(sigma * Math.Sqrt(-2.0 * Math.Log(1 - u)), 0.0, 1.0);
        }

        /// <summary>
        /// Метод линейной интерполяции между двумя значениями a и b с параметром t
        /// </summary>
        /// <param name="a">Минимум</param>
        /// <param name="b">Максимум</param>
        /// <param name="t">Коэфициент</param>
        /// <returns></returns>
        private static double Lerp(double a, double b, double t) => a + (b - a) * t;
    }
}
