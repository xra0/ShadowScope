namespace ShadowScope.Resources.Code
{
    internal class Balls
    {
        // Свойства шара
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
        /// Возвращает или задает скорость шара.
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
        /// Задает количество шаров
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
    }
}
