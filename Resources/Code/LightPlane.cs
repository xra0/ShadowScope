namespace ShadowScope.Resources.Code
{
    /// <summary>
    /// Класс, представляющий плоскость с толщиной и углом наклона.
    /// </summary>
    public static class LightPlane
    {
        private static double thickness;
        private static double angle;

        public static Vec2[] Position { get; private set; }

        public static double Thickness
        {
            get => thickness;
            set
            {
                if (value <= 0)
                    throw new ArgumentOutOfRangeException("Толщина должна быть больше нуля.");
                thickness = value;
            }
        }

        public static double Angle
        {
            get => angle;
            set
            {
                if (value < -90 || value > 90)
                    throw new ArgumentOutOfRangeException("Угол должен быть в диапазоне от -90 до 90 градусов.");
                angle = value;
            }
        }

        /// <summary>
        /// Расстояние от плоскости света до экрана.
        /// </summary>
        public static double DistanceToScreen { get; internal set; }

        /// <summary>
        /// Вычисляет четыре вершины наклонённой плоскости.
        /// </summary>
        public static Vec2[] CalculatePosition(double step = 0.01)
        {
            double distanceToPlane = Balls.Count * 100 + DistanceToScreen;

            double angleRad = Angle * Math.PI / 180.0;

            // смещение по толщине с учётом угла
            double dx = Thickness * Math.Sin(angleRad);
            double dy = Thickness * Math.Cos(angleRad);

            // 4 точки параллелограмма
            Vec2 L1 = new Vec2(distanceToPlane, 0);          // нижняя левая
            Vec2 L2 = new Vec2(distanceToPlane + dx, dy);     // верхняя левая
            Vec2 R2 = new Vec2(distanceToPlane + Thickness, 0); // нижняя правая
            Vec2 R1 = new Vec2(distanceToPlane + dx + Thickness, dy); // верхняя правая

            Position = new Vec2[] { L1, L2, R1, R2 };
            return Position;
        }

        public static string ToString_()
        {
            return $"LightPlane(Толщина: {Thickness}, Угол: {Angle}, " +
                   $"Позиции: [{string.Join(", ", Position.Select(p => p.ToString()))}])";
        }
    }
}
