namespace ShadowModel.Resources.Code
{
    /// <summary>
    /// Класс, представляющий плоскость с толщиной и углом наклона.
    /// </summary>
    public class LightPlane
    {
        private double thickness; // Толщина плоскости
        private double angle; // Угол наклона плоскости в градусах
        public Point[] Position { get; private set; }
        /// <summary>
        /// Возвращает или задает толщину плоскости.
        /// </summary>
        /// <remarks>Толщина должна быть больше нуля. Есть валидация.</remarks>
        public double Thickness 
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
        public double Angle 
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
        public Point[] CalculatePosition(double radius, int size, int count, double DistanceToDisplay, double step = 0.01)
        {
            List<Point> result = new List<Point>();
            
            double dist = radius * 2 * (int)(Math.Sqrt(count) + 1) * size;

            // Вектор сдвига по толщине с учетом угла наклона экрана
            double angleRad = angle * Math.PI / 180.0;
            double offsetX = thickness * Math.Cos(angleRad);
            double offsetY = thickness * Math.Sin(angleRad);

            // Вычисляем 4 точки параллелограмма
            Point p1 = new Point(DistanceToDisplay, 0);
            Point p2 = new Point(DistanceToDisplay + thickness, 0);
            Point p3 = new Point(p2.X + offsetX, p2.Y + offsetY + dist);
            Point p4 = new Point(p1.X + offsetX, p1.Y + offsetY + dist);

            // Добавляем точки на каждом ребре
            GenerateEdgePoints(p1, p2, step, result);
            GenerateEdgePoints(p2, p3, step, result);
            GenerateEdgePoints(p3, p4, step, result);
            GenerateEdgePoints(p4, p1, step, result);

            return result.Distinct().ToArray();
        }
        /// <summary>
        /// Генерация точек на одном ребре
        /// </summary>
        /// <param name="a">Точка 0,0</param>
        /// <param name="b">Точка 1,0</param>
        /// <param name="step">Шаг точности</param>
        /// <param name="result">Результат</param>
        private void GenerateEdgePoints(Point a, Point b, double step, List<Point> result)
        {
            double length = Math.Sqrt(Math.Pow(b.X - a.X, 2) + Math.Pow(b.Y - a.Y, 2));
            int segments = (int)(length / step);

            for (int i = 0; i <= segments; i++)
            {
                double t = (double)i / segments;
                double x = a.X + (b.X - a.X) * t;
                double y = a.Y + (b.Y - a.Y) * t;
                result.Add(new Point(x, y));
            }
        }
        public LightPlane() { }
        public LightPlane(double thickness, double angle, Point[] position)
        {
            Thickness = thickness;
            Angle = angle;
            this.Position = position;
        }
        /// <summary>
        /// Используется для получения строкового представления координат плоскости.
        /// </summary>
        /// <remarks>Исключительно для отладки и логирования.</remarks>
        /// <returns>Строку с координатами</returns>
        public override string ToString()
        {
            return $"LightPlane(Толщина: {Thickness}, Угол: {Angle}, Позиция: [{string.Join(", ", Position.Cast<object>())}])";
        }
    }
}
