namespace ShadowScope.Resources.Code
{
    /// <summary>
    /// Класс, представляющий плоскость с толщиной и углом наклона.
    /// </summary>
    public static class LightPlane
    {
        private static double thickness;    // Толщина плоскости
        private static double angle;        // Угол наклона плоскости в градусах

        /// <summary>
        /// Массив из четырёх вершин плоскости.
        /// </summary>
        /// <remarks>Позиции хранятся в порядке: нижняя левая, верхняя левая, верхняя правая, нижняя правая.</remarks>
        public static Vec2[] Position { get; private set; } // Позиции четырёх вершин плоскости

        /// <summary>
        /// Толщина плоскости.
        /// </summary>
        /// <remarks>Толщина должна быть больше нуля.</remarks>
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

        /// <summary>
        /// Угол наклона плоскости в градусах.
        /// </summary>
        /// <remarks>Допустимый диапазон угла от -90 до 90 градусов.</remarks>
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
        /// <remarks>Расстояние не может быть отрицательным.</remarks>
        public static double DistanceToScreen { get; 
            set
            {
                if (value < 0)
                    throw new ArgumentOutOfRangeException("Расстояние до экрана не может быть отрицательным.");
            } 
        }

        /// <summary>
        /// Метод для расчёта позиций четырёх вершин плоскости.
        /// </summary>
        /// <param name="step">Шаг точности</param>
        /// <returns>Vec2 Массив координат</returns>
        public static Vec2[] CalculatePosition(double step = 0.01)
        {
            double distanceToPlane = Balls.Count * 100 + DistanceToScreen;  // Расстояние до плоскости света

            double angleRad = Angle * Math.PI / 180.0;  // угол в радианах

            // смещение по толщине с учётом угла
            double dx = Thickness * Math.Sin(angleRad);
            double dy = Thickness * Math.Cos(angleRad);

            // 4 точки параллелограмма
            Vec2 L1 = new Vec2(distanceToPlane, 0);          // нижняя левая
            Vec2 L2 = new Vec2(distanceToPlane + dx, dy);     // верхняя левая
            Vec2 R2 = new Vec2(distanceToPlane + Thickness, 0); // нижняя правая
            Vec2 R1 = new Vec2(distanceToPlane + dx + Thickness, dy); // верхняя правая

            return Position = [L1, L2, R1, R2];
        }

        /// <summary>
        /// Метод для получения строкового представления плоскости.
        /// </summary>
        /// <remarks>Нужно для отладки.</remarks>
        /// <returns>Строка с информацией о плоскости</returns>
        private static string ToString_()
        {
            return $"LightPlane(Толщина: {Thickness}, Угол: {Angle}, " +
                   $"Позиции: [{string.Join(", ", Position.Select(p => p.ToString()))}])";
        }
    }
}
