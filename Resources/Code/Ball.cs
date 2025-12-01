namespace ShadowModel.Resources.Code
{
    /// <summary>
    /// Класс, представляющий шар с радиусом, скоростью и позицией.
    /// </summary>
    public class Ball
    {
        private double radius;  // Радиус шара
        private double speed;   // Скорость шара
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
        private static double DS { get; set; }
        /// <summary>
        /// Возвращает площадь шара
        /// </summary>
        public double Area => Math.PI * Radius * Radius;
        /// <summary>
        /// Возвращает площадь шара за малое время(dt)
        /// </summary>
        /// <remarks>Используется для сохранения вошедшей части шара в плоскость.</remarks>
        public static double AreaDt { get; internal set; } = 0;
        public Ball() { }
        public Ball(double radius, double speed, Point position)
        {
            Radius = radius;
            Speed = speed;
            Position = position;
        }
        /// <summary>
        /// Метод для проверки пересечения шара с плоскостью.
        /// </summary>
        /// <param name="plane">Плоскость света.</param>
        /// <returns>Возвращает true, если шар пересекает плоскость, иначе false.</returns>
        public bool Intersects(LightPlane plane)
        {
            // TODO: Реализовать проверку пересечения шара с плоскостью
            return false; // Пересечений нет
        }
        /// <summary>
        /// Метод для перемещения шара на основе его скорости и времени.
        /// </summary>
        /// <param name="dt">Шаг времени.</param>
        public void MoveBall(double dt)
        {
            Position.X += Speed * dt;
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
