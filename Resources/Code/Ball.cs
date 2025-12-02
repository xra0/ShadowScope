namespace ShadowScope.Resources.Code
{
    /// <summary>
    /// Класс, представляющий шар с радиусом, скоростью и позицией.
    /// </summary>
    public class Ball
    {
        private double radius;  // Радиус шара
        private double speed;   // Скорость шара
        private static double area;    // Площадь шара
        /// <summary>
        /// Положение шара в 2D пространстве.
        /// </summary>
        public Point Position { get; set; }
        /// <summary>
        /// Возвращает или задает, радиус шара.
        /// </summary>
        /// <remarks>Радиус должен быть больше нуля. Есть валидация.</remarks>
        public double Radius
        {
            get { return radius; }
            set
            {
                if (value > 0)
                    radius = value;
            }
        }
        /// <summary>
        /// Возвращает или задает скорость шара.
        /// </summary>
        /// <remarks>Скорость должна быть больше нуля. Есть валидация.</remarks>
        public double Speed
        {
            get { return speed; }
            set
            {
                if (value > 0)
                    speed = value;
            }
        }
        /// <summary>
        /// Задает площадь от малого времени(dt)
        /// </summary>
        private static double DS
        {
            get
            {
                return area / Physics.DTime;
            }
        }
        /// <summary>
        /// Возвращает площадь шара
        /// </summary>
        public double Area { get; set; }
        /// <summary>
        /// Возвращает площадь шара за малое время(dt)
        /// </summary>
        /// <remarks>Используется для сохранения вошедшей части шара в плоскость.</remarks>
        public double AreaDt { get; internal set; }
        public Ball() { }
        public Ball(double radius, double speed, Point position)
        {
            Radius = radius;
            Speed = speed;
            Position = position;
            Area = Math.PI * Radius * Radius;
            AreaDt = Area / Physics.Time;
        }
        /// <summary>
        /// Метод для проверки пересечения шара с плоскостью.
        /// </summary>
        /// <remarks>Выполняется только по оси X.</remarks>
        /// <param name="plane">Плоскость света.</param>
        /// <returns>Возвращает true, если шар пересекает плоскость, иначе false.</returns>
        public bool IntersectsPlaneX()
        {
            // Получаем углы плоскости
            Point leftBottom = LightPlane.Position[0];
            Point leftTop = LightPlane.Position[1];
            Point rightTop = LightPlane.Position[2];
            Point rightBottom = LightPlane.Position[3];

            double y = Position.Y;

            // 1) Находим X левой и правой граней в точке Y шара
            double leftX = InterpolateXByY(leftTop, leftBottom, y);
            double rightX = InterpolateXByY(rightTop, rightBottom, y);

            // 2) Находим границы шара по X
            double ballLeft = Position.X - Radius;
            double ballRight = Position.X + Radius;

            // 3) Пересечение только по X
            bool intersects =
                ballRight >= leftX &&   // правая часть шара пересекает левую грань
                ballLeft <= rightX;     // левая часть шара пересекает правую грань

            return intersects;
        }
        /// <summary>
        /// Метод для интерполяции X по Y между двумя точками.
        /// </summary>
        /// <param name="a">Первая точка</param>
        /// <param name="b">Вторая точка</param>
        /// <param name="y"></param>
        /// <returns></returns>
        private double InterpolateXByY(Point a, Point b, double y)
        {
            if (Math.Abs(b.Y - a.Y) < 0.0001)
                return a.X; // грань горизонтальна

            double t = (y - a.Y) / (b.Y - a.Y);
            return a.X + (b.X - a.X) * t;
        }
        /// <summary>
        /// Метод для перемещения шара на основе его скорости и времени.
        /// </summary>
        /// <param name="dt">Шаг времени.</param>
        public void MoveBall(double dt)
        {
            Position.X += (int)(Speed * dt);
        }
        /// <summary>
        /// Возвращает строковое представление шара.
        /// </summary>
        /// <remarks>Исключительно для отладки и логирования.</remarks>
        /// <returns>Строка</returns>
        public override string ToString()
        {
            return $"Ball(Радиус: {Radius}, Скорость: {Speed}, Позиция: {Position.ToString})";
        }
    }
}
