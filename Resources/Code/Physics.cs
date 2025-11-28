using BepuPhysics;
using BepuPhysics.Collidables;
using BepuUtilities.Memory;
using System.Numerics;

namespace ShadowScope.Resources.Code
{
    class Physics
    {
        public static double Thickness { get; set; }
        public static double Angle { get; set; }
        public static double Radius { get; set; }
        public static double DistanceToScreen { get; set; }
        public static int DistributionType { get; set; }
        public static double Speed { get; set; }
        public static double Area { get; set; }
        public static double Count { get; set; }
        public static Simulation? Simulation { get; set; }

        public static void InitializePhysics()
        {
            var bufferPool = new BufferPool();

            var narrowPhaseCallbacks = new NarrowPhaseCallbacks();
            var poseIntegratorCallbacks = new PoseIntegratorCallbacks();

            // Create simulation with default settings
            Simulation = Simulation.Create(
                bufferPool,
                narrowPhaseCallbacks,
                poseIntegratorCallbacks,
                new SolveDescription(8, 1) // 8 velocity iterations, 1 substep
            );
        }

        private static BodyHandle CreateSphere()
        {
            // Create a sphere shape
            var sphereShape = new Sphere(1); // 1-meter radius sphere
            var sphereShapeIndex = Simulation.Shapes.Add(sphereShape);

            // Create a body description
            var bodyDescription = BodyDescription.CreateDynamic(
                new Vector3(0, 10, 0),  // Initial position
                new BodyInertia { InverseMass = 1 }, // Mass = 1kg
                new CollidableDescription(sphereShapeIndex, 0.1f), // Shape with 0.1 speculative margin
                new BodyActivityDescription(0.01f) // Sleep threshold
            );

            // Add the body to the simulation
            return Simulation.Bodies.Add(bodyDescription);
        }
    }
}
