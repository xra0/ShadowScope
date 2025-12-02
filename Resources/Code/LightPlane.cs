namespace ShadowScope.Resources.Code
{
    /// <summary>
    /// Класс, представляющий плоскость с толщиной и углом наклона.
    /// </summary>
    public static class LightPlane
    {
        private static double thickness; // Толщина плоскости
        private static double angle; // Угол наклона плоскости в градусах
        public static Point[] Position { get; private set; }
        /// <summary>
        /// Возвращает или задает толщину плоскости.
        /// </summary>
        /// <remarks>Толщина должна быть больше нуля. Есть валидация.</remarks>
        public static double Thickness 
        {
            get { return thickness; }
            set
            {
                if (value > 0)
                {
                    thickness = value;
                    return;
                }
                throw new ArgumentOutOfRangeException("Толщина должна быть больше нуля.");
            }
        }
        /// <summary>
        /// Возвращает или задает угол наклона плоскости в градусах.
        /// </summary>
        /// <remarks>Допустимый диапазон угла от -90 до 90 градусов. Есть валидация.</remarks>
        public static double Angle 
        {
            get { return angle; }
            set
            {
                if (value >= -90 && value <= 90)
                {
                    angle = value;
                    return;
                }
                throw new ArgumentOutOfRangeException("Угол должен быть в диапазоне от -90 до 90 градусов.");
            }
        }
        /// <summary>
        /// Задает расстояние от плоскости света до экрана
        /// </summary>
        public static double DistanceToScreen { get; internal set; }
        /// <summary>
        /// Метод для вычисления позиций точек параллелограмма, представляющего плоскость.
        /// </summary>
        /// <param name="radius">Радиус шара.</param>
        /// <param name="size">Уровень увеличения.</param>
        /// <param name="count">Количество шаров.</param>
        /// <param name="DistanceToDisplay">Расстояние до экрана.</param>
        /// <param name="step">Шаг точности.</param>
        /// <returns></returns>
        public static Point[] CalculatePosition(double step = 0.01)
        {
            // Расстояние до плоскости
            double distanceToPlane = Balls.Count * 100 + DistanceToScreen;

            // Угол в радианах
            double angleRad = Angle * Math.PI / 180.0;

            // Смещение второй грани (толщина)
            double dx = Thickness * Math.Sin(angleRad);
            double dy = 1000 * Math.Cos(angleRad);

            // Левая передняя точка (L1)
            Point L1 = new Point((int)distanceToPlane, 0);

            // Левая задняя точка (L2)
            Point L2 = new Point((int)(distanceToPlane + dx), (int)(dy));

            // Правая передняя точка (R1)
            Point R1 = new Point((int)(distanceToPlane + dx + Thickness), (int)(dy));

            // Правая задняя точка (R2)
            Point R2 = new Point((int)(distanceToPlane + Thickness), 0);

            List<Point> result = new List<Point>();

            // Генерация точек на 2 боковых гранях
            //GenerateEdgePoints(L1, L2, step, result); // левая
            //GenerateEdgePoints(R1, R2, step, result); // правая

            //Position = result.ToArray();
            return Position = new Point[4] { L1, L2, R1, R2 }; ;
        }
        /// <summary>
        /// Генерация точек на одном ребре
        /// </summary>
        /// <param name="a">Точка 0,0</param>
        /// <param name="b">Точка 1,0</param>
        /// <param name="step">Шаг точности</param>
        /// <param name="result">Результат</param>
        private static void GenerateEdgePoints(Point a, Point b, double step, List<Point> result)
        {
            double length = Math.Sqrt(Math.Pow(b.X - a.X, 2) + Math.Pow(b.Y - a.Y, 2));
            int segments = Math.Max(1, (int)(length / step));

            for (int i = 0; i <= segments; i++)
            {
                double t = (double)i / segments;
                int x = (int)(a.X + (b.X - a.X) * t);
                int y = (int)(a.Y + (b.Y - a.Y) * t);
                result.Add(new Point(x, y));
            }
        }
        /// <summary>
        /// Используется для получения строкового представления координат плоскости.
        /// </summary>
        /// <remarks>Исключительно для отладки и логирования.</remarks>
        /// <returns>Строку с координатами</returns>
        public static string ToString_()
        {
            return $"LightPlane(Толщина: {Thickness}, Угол: {Angle}, Позиция: [{string.Join(", ", Position.Cast<object>())}])";
        }
    }
}
