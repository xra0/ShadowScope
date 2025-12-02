using System;
using System.Threading;
using System.Threading.Tasks;

namespace ShadowScope.Resources.Code
{
    internal static class Physics
    {
        public static DistributionType Distribution_Type { get; internal set; }
        public const double DTime = 1;

        public static double Time { get; internal set; }
        public static double[] SumArea { get; private set; }

        public static Ball[] Balls_Array { get; private set; }
        private static Vec2[] Position_Generator;

        [ThreadStatic]
        private static Random threadRnd;

        private static Random GlobalRnd = new();

        public static void InitializePhysics()
        {
            Position_Generator = new[]
            {
                new Vec2(0,0),
                new Vec2(0,1000),
                new Vec2(Balls.Count * 100,1000),
                new Vec2(Balls.Count * 100,0)
            };

            Time = Balls.Count * 100 + LightPlane.DistanceToScreen + LightPlane.Thickness + 10;
            SumArea = new double[(int)Time + 1];
            Balls_Array = new Ball[Balls.Count];
        }

        public static void Start(Action<double> reportProgress)
        {
            SpawnBalls(Distribution_Type);

            int totalSteps = (int)(Time / DTime);
            double progress = 0;

            for (int step = 0; step < totalSteps; step++)
            {
                double t = step * DTime;
                double areaAccumulator = 0;

                Parallel.For(0, Balls_Array.Length, () => 0.0,
                    (i, state, local) =>
                    {
                        ref Ball b = ref Balls_Array[i];

                        if (b.IntersectsPlaneX())
                            local += b.AreaDt;

                        b.MoveBall(DTime);
                        return local;
                    },
                    local => InterlockedAdd(ref areaAccumulator, local)
                );

                SumArea[step] = areaAccumulator;

                progress++;
                reportProgress?.Invoke(progress);
            }
        }

        private static void InterlockedAdd(ref double location, double value)
        {
            double initial, computed;
            do
            {
                initial = location;
                computed = initial + value;
            }
            while (Interlocked.CompareExchange(ref location, computed, initial) != initial);
        }

        public static void ResetPhysics()
        {
            Array.Clear(SumArea, 0, SumArea.Length);
        }

        public static void SpawnBalls(DistributionType type)
        {
            for (int i = 0; i < Balls.Count; i++)
            {
                double rx = Sample(type);
                double ry = Sample(type);

                double x = Lerp(Position_Generator[0].X, Position_Generator[2].X, rx);
                double y = Lerp(Position_Generator[3].Y, Position_Generator[2].Y, ry);

                Balls_Array[i] = new Ball(Balls.Radius, Balls.Speed, new Vec2(x, y));
            }
        }

        private static double Lerp(double a, double b, double t) => a + (b - a) * t;

        private static Random GetRnd()
        {
            return threadRnd ??= new Random(GlobalRnd.Next());
        }

        private static double Sample(DistributionType type)
        {
            return type switch
            {
                DistributionType.Uniform => GetRnd().NextDouble(),
                DistributionType.Normal => Normal(),
                DistributionType.Rayleigh => Rayleigh(),
                _ => GetRnd().NextDouble()
            };
        }

        private static double Normal()
        {
            var rnd = GetRnd();
            double u1 = 1.0 - rnd.NextDouble();
            double u2 = 1.0 - rnd.NextDouble();
            double normal = Math.Sqrt(-2.0 * Math.Log(u1)) * Math.Cos(2 * Math.PI * u2);
            return Math.Clamp(0.5 + normal * 0.15, 0.0, 1.0);
        }

        private static double Rayleigh()
        {
            var rnd = GetRnd();
            double u = rnd.NextDouble();
            double sigma = 0.3;
            return Math.Clamp(sigma * Math.Sqrt(-2.0 * Math.Log(1 - u)), 0.0, 1.0);
        }
    }
}
