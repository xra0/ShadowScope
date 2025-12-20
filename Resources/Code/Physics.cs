namespace ShadowScope.Resources.Code
{
    /// <summary>
    /// Класс, отвечающий за физику движения шаров и их взаимодействие с плоскостью света.
    /// </summary>
    public static class Physics
    {
        public static DistributionType Distribution_Type { get; internal set; } // Тип распределения для генерации позиций шаров
        
        public static double DTime = 1;  // Шаг времени для симуляции

        /// <summary>
        /// Общее время симуляции
        /// </summary>
        public static double Time { get; internal set; }   
        
        /// <summary>
        /// Массив для хранения суммарной площади шаров, пересекающих плоскость света на каждом шаге времени
        /// </summary>
        public static double[] SumArea { get; set; }

        /// <summary>
        /// Метод инициализации физических параметров симуляции
        /// </summary>
        public static void InitializePhysics()
        {
            Balls.InitializeGenerator(); // Инициализация генератора шаров
            Time = (Math.Sqrt(Balls.Count * 10 + 1) + LightPlane.DistanceToScreen + LightPlane.Thickness + Math.Sqrt(Balls.Count * 10 + 1) * Math.Tan(LightPlane.Angle) + 10) /Balls.Speed;     // Время численно равно расстоянию, которое должен пройти самый дальний шар плюс запас
            SumArea = new double[(int)Time + 1];
            Balls.SpawnBalls(Distribution_Type);  // Генерация начальных позиций шаров
        }

        /// <summary>
        /// Метод запуска симуляции физики движения шаров
        /// </summary>
        /// <param name="reportProgress">Делегат для отчета о прогрессе симуляции </param>
        public static void Start(Action<double> reportProgress)
        {
            int totalSteps = (int)(Time / DTime);   // Общее количество шагов времени
            double progress = 0;    // Переменная для отслеживания прогресса

            for (int step = 0; step < totalSteps; step++)
            {
                double t = step * DTime;    // Текущее время симуляции
                double areaAccumulator = 0; // Аккумулятор для суммарной площади на текущем шаге

                // Параллельный цикл для обновления состояния каждого шара и вычисления площади пересечения с плоскостью света
                Parallel.For(0, Balls.Array.Length, () => 0.0,  
                    (i, state, local) =>
                    {
                        ref Ball b = ref Balls.Array[i];

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
        }    }
}
