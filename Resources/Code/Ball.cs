using ShadowScope.Resources.Code;

/// <summary>
/// Представляет структуру шара с его свойствами и методами.
/// </summary>
/// <remarks>Нужна только для оптимизации. Структура Ball содержит информацию о радиусе, скорости, позиции и площади шара, а также методы для проверки пересечения с плоскостью и перемещения шара.</remarks>
public struct Ball
{
    public double Radius;   // Радиус шара
    public double Speed;    // Скорость шара

    public Vec2 Position;   // Позиция шара

    public double Area;     // Площадь шара

    /// <summary>
    /// Площадь шара за шаг dt.
    /// </summary>
    public double AreaDt;

    /// <summary>
    /// Конструктор для инициализации шара с заданными радиусом, скоростью и позицией.
    /// </summary>
    /// <param name="radius">Радиус шара</param>
    /// <param name="speed">Скорость шара</param>
    /// <param name="vector">Координаты шара. Могут находиться как в центре, так и на границе шара.</param>
    public Ball(double radius, double speed, Vec2 vector)
    {
        // Инициализация полей структуры
        Radius = radius;
        Speed = speed;
        Position = vector;

        Area = Math.PI * radius * radius;   // пи * r^2
        AreaDt = Area * Physics.DTime; // площадь за шаг dt
    }

    /// <summary>
    /// Метод для проверки пересечения шара с плоскостью света по оси X.
    /// </summary>
    /// <returns>True, если шар пересекает плоскость света, иначе False.</returns>
    public bool IntersectsPlaneX()
    {
        // Считаем по часовой стрелке от левого нижнего угла
        var leftBottom = LightPlane.Position[0];
        var leftTop = LightPlane.Position[1];
        var rightTop = LightPlane.Position[2];
        var rightBottom = LightPlane.Position[3];

        double leftX = InterpolateXByY(leftTop, leftBottom, Position.Y);    // Интерполяция X левой грани плоскости по Y шара
        double rightX = InterpolateXByY(rightTop, rightBottom, Position.Y); // Интерполяция X правой грани плоскости по Y шара

        double ballLeft = Position.X - Radius;  // Левая граница шара
        double ballRight = Position.X + Radius; // Правая граница шара

        return (ballRight >= leftX) && (ballLeft <= rightX);    // Если правая граница шара правее левой грани плоскости и левая граница шара левее правой грани плоскости, то есть пересечение есть
    }

    /// <summary>
    /// Метод для интерполяции X по Y между двумя точками.
    /// </summary>
    /// <param name="a">Первая точка</param>
    /// <param name="b">Вторая точка</param>
    /// <param name="y">Y значение для интерполяции</param>
    /// <returns></returns>
    private static double InterpolateXByY(Vec2 a, Vec2 b, double y)
    {
        double t = (y - a.Y) / (b.Y - a.Y); // Нормализованное значение t между a и b по Y
        return a.X + (b.X - a.X) * t;
    }

    /// <summary>
    /// Метод для перемещения шара вдоль оси X на основе его скорости и времени dt.
    /// </summary>
    /// <param name="dt">Малый промежуток времени</param>
    public void MoveBall(double dt)
    {
        Position.X += Speed * dt;
    }
}
