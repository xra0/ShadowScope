using ShadowScope.Resources.Code;

public struct Ball
{
    public double Radius;
    public double Speed;

    public Vec2 Position;

    public double Area;
    public double AreaDt;

    public Ball(double radius, double speed, double x, double y)
    {
        Radius = radius;
        Speed = speed;
        Position = new Vec2(x, y);

        Area = Math.PI * radius * radius;
        AreaDt = Area * Physics.DTime; // площадь за шаг dt
    }

    public Ball(double radius, double speed, Vec2 vector)
    {
        Radius = radius;
        Speed = speed;
        Position = vector;

        Area = Math.PI * radius * radius;
        AreaDt = Area * Physics.DTime; // площадь за шаг dt
    }

    public bool IntersectsPlaneX()
    {
        var leftBottom = LightPlane.Position[0];
        var leftTop = LightPlane.Position[1];
        var rightTop = LightPlane.Position[2];
        var rightBottom = LightPlane.Position[3];

        double leftX = InterpolateXByY(leftTop, leftBottom, Position.Y);
        double rightX = InterpolateXByY(rightTop, rightBottom, Position.Y);

        double ballLeft = Position.X - Radius;
        double ballRight = Position.X + Radius;

        return (ballRight >= leftX) && (ballLeft <= rightX);
    }

    private static double InterpolateXByY(Vec2 a, Vec2 b, double y)
    {
        double t = (y - a.Y) / (b.Y - a.Y);
        return a.X + (b.X - a.X) * t;
    }

    public void MoveBall(double dt)
    {
        Position.X += Speed * dt;
    }
}
