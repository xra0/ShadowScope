public struct Vec2
{
    public double X;
    public double Y;

    public Vec2(double x, double y)
    {
        X = x;
        Y = y;
    }

    public override string ToString() => $"({X:F3}, {Y:F3})";
}
