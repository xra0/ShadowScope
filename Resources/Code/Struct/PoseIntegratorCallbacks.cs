using BepuPhysics;
using BepuUtilities;
using System.Numerics;

struct PoseIntegratorCallbacks : IPoseIntegratorCallbacks // TODO: Реализовать интерфейс
{
    public AngularIntegrationMode AngularIntegrationMode => AngularIntegrationMode.Nonconserving;

    public bool AllowSubstepsForUnconstrainedBodies => false;

    public bool IntegrateVelocityForKinematics => false;

    // Не используется, но в структуре обычно есть
    public Vector3 Gravity;

    public PoseIntegratorCallbacks(Vector3 gravity)
    {
        Gravity = gravity;
    }

    public void Initialize(Simulation simulation)
    {
    }

    public void PrepareForIntegration(float dt)
    {
        // Без гравитации — ничего не делаем
    }

    // ВАЖНО: это правильная wide-сигнатура для текущей версии bepu v2
    public void IntegrateVelocity(
        Vector<int> bodyIndices,
        Vector3Wide position,
        QuaternionWide orientation,
        BodyInertiaWide localInertia,
        Vector<int> integrationMask,
        int workerIndex,
        Vector<float> dt,
        ref BodyVelocityWide velocity)
    {
        // Без гравитации: скорость остаётся неизменной → ничего не делаем.
        // (в отличие от версии v1, здесь запрещено возвращать out-значения)
    }
}
