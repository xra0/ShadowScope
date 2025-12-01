namespace ShadowModel.Resources.Code
{
    /// <summary>
    /// Класс, представляющий точку в 2D пространстве.
    /// </summary>
    public class Point
    {
        public double X { get; set; }
        public double Y { get; set; }
        public Point(double x, double y)
        {
            X = x;
            Y = y;
        }
        /// <summary>
        /// Возвращает расстояние до другой точки.
        /// </summary>
        /// <param name="other">Другая точка</param>
        /// <returns>Расстояние от данной точки до other.</returns>
        public double DistanceTo(Point other)
        {
            return System.Math.Sqrt(System.Math.Pow(X - other.X, 2) + System.Math.Pow(Y - other.Y, 2));
        }
        /// <summary>
        /// Используется для получения строкового представления координат точки.
        /// </summary>
        /// <remarks>Исключительно для отладки и логирования.</remarks>
        /// <returns>Строку с координатами</returns>
        public override string ToString()
        {
            return $"({X}, {Y})";
        }
    }
}
