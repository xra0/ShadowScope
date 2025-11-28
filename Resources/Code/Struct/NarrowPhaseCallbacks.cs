using BepuPhysics;
using BepuPhysics.Collidables;
using BepuPhysics.CollisionDetection;
using BepuPhysics.Constraints;

public struct NarrowPhaseCallbacks : INarrowPhaseCallbacks
{
    public void Initialize(Simulation simulation) { }
    public bool AllowContactGeneration(int worker, CollidableReference a, CollidableReference b, ref float speculativeMargin) => true;
    public bool AllowContactGeneration(int worker, CollidablePair pair, int childA, int childB) => true;
    public bool ConfigureContactManifold<TManifold>(int w, CollidablePair pair, ref TManifold m, out PairMaterialProperties p)
        where TManifold : unmanaged, IContactManifold<TManifold>
    {
        p = new PairMaterialProperties
        {
            FrictionCoefficient = 0.6f,
            MaximumRecoveryVelocity = 2f,
            SpringSettings = new SpringSettings(30f, 1f)
        };
        return true;
    }
    public bool ConfigureContactManifold(int w, CollidablePair pair, int a, int b, ref ConvexContactManifold m) => true;
    public void Dispose() { }
}
