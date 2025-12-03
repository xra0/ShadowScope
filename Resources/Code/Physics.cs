namespace ShadowScope.Resources.Code
{
    /// <summary>
    /// Класс, отвечающий за физику движения шаров и их взаимодействие с плоскостью света.
    /// </summary>
    internal static class Physics
    {
        public static DistributionType Distribution_Type { get; internal set; } // Тип распределения для генерации позиций шаров
        public static double DTime;  // Шаг времени для симуляции

        /// <summary>
        /// Общее время симуляции
        /// </summary>
        public static double Time { get; internal set; }   
        
        /// <summary>
        /// Массив для хранения суммарной площади шаров, пересекающих плоскость света на каждом шаге времени
        /// </summary>
        public static double[] SumArea { get; private set; }

        /// <summary>
        /// Массив шаров, участвующих в симуляции
        /// </summary>
        public static Ball[] Balls_Array { get; private set; }

        /// <summary>
        /// Позиции для генерации шаров
        /// </summary>
        private static Vec2[] Position_Generator;

        [ThreadStatic]
        private static Random threadRnd;    // Локальный экземпляр Random для каждого потока
        private static Random GlobalRnd = new();

        /// <summary>
        /// Метод инициализации физических параметров симуляции
        /// </summary>
        public static void InitializePhysics()
        {
            Position_Generator = new[]
            {
                new Vec2(0,0),
                new Vec2(0,1000),
                new Vec2(Balls.Count * 100,1000),
                new Vec2(Balls.Count * 100,0)
            };

            Time = (Balls.Count * 100 + LightPlane.DistanceToScreen + LightPlane.Thickness + 10)/Balls.Speed;     // Время численно равно расстоянию, которое должен пройти самый дальний шар плюс запас
            SumArea = new double[(int)Time + 1];
            Balls_Array = new Ball[Balls.Count];

            DTime = 1;
        }

        /// <summary>
        /// Метод запуска симуляции физики движения шаров
        /// </summary>
        /// <param name="reportProgress">Делегат для отчета о прогрессе симуляции </param>
        public static void Start(Action<double> reportProgress)
        {
            SpawnBalls(Distribution_Type);  // Генерация начальных позиций шаров

            int totalSteps = (int)(Time / DTime);   // Общее количество шагов времени
            double progress = 0;    // Переменная для отслеживания прогресса

            for (int step = 0; step < totalSteps; step++)
            {
                double t = step * DTime;    // Текущее время симуляции
                double areaAccumulator = 0; // Аккумулятор для суммарной площади на текущем шаге

                // Параллельный цикл для обновления состояния каждого шара и вычисления площади пересечения с плоскостью света
                Parallel.For(0, Balls_Array.Length, () => 0.0,  
                    (i, state, local) =>
                    {
                        ref Ball b = ref Balls_Array[i];

                        if (b.IntersectsPlaneX())   // Если шар пересекает плоскость света
                            local += b.AreaDt;

                        b.MoveBall(DTime);  // Обновление позиции шара
                        return local;
                    },
                    local => InterlockedAdd(ref areaAccumulator, local) // Добавление локальной суммы площади в глобальный аккумулятор
                );

                SumArea[step] = areaAccumulator;    // Сохранение суммарной площади для текущего шага времени

                progress+= 0.25; // Обновление прогресса
                reportProgress?.Invoke(progress);   // Вызов делегата для отчета о прогрессе
            }
        }

        /// <summary>
        /// Метод для атомарного сложения double значений
        /// </summary>
        /// <param name="location">Расположение для сложения </param>
        /// <param name="value">Значение</param>
        private static void InterlockedAdd(ref double location, double value)
        {
            double initial, computed;
            do
            {
                initial = location;
                computed = initial + value;
            }
            while (Interlocked.CompareExchange(ref location, computed, initial) != initial);    // Повторяем, пока не удастся успешно обновить значение
        }

        /// <summary>
        /// Метод для сброса физических параметров симуляции
        /// </summary>
        public static void ResetPhysics()
        {
            Array.Clear(SumArea, 0, SumArea.Length);
        }

        /// <summary>
        /// Метод генерации начальных позиций шаров в зависимости от выбранного типа распределения
        /// </summary>
        /// <param name="type">Тип распределения</param>
        private static void SpawnBalls(DistributionType type)
        {
            for (int i = 0; i < Balls.Count; i++)
            {
                double rx = Sample(type);   // 0..1
                double ry = Sample(type);   // 0..1

                double x = Lerp(Position_Generator[0].X, Position_Generator[2].X, rx);  // Линейная интерполяция по X
                double y = Lerp(Position_Generator[3].Y, Position_Generator[2].Y, ry);  // Линейная интерполяция по Y

                Balls_Array[i] = new Ball(Balls.Radius, Balls.Speed, new Vec2(x, y));
            }
        }

        /// <summary>
        /// Метод линейной интерполяции между двумя значениями a и b с параметром t
        /// </summary>
        /// <param name="a">Минимум</param>
        /// <param name="b">Максимум</param>
        /// <param name="t">Коэфициент</param>
        /// <returns></returns>
        private static double Lerp(double a, double b, double t) => a + (b - a) * t;

        /// <summary>
        /// Метод для получения экземпляра Random, уникального для каждого потока
        /// </summary>
        /// <returns>Экземпляр Random</returns>
        private static Random GetRnd()
        {
            return threadRnd ??= new Random(GlobalRnd.Next());
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
    }
}
