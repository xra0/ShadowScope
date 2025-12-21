using ShadowScope.Resources.Code;

namespace Tests
{
    [TestClass]
    public class DistributionTests
    {
        [TestMethod]
        public void UniformDistribution_ValueIsInRange()
        {
            for (int i = 0; i < 10_000; i++)
            {
                double value = Balls.GetRnd().NextDouble();
                Assert.IsTrue(value >= 0.0 && value <= 1.0);
            }
        }
        [TestMethod]
        public void NormalDistribution_ValueIsInRange()
        {
            for (int i = 0; i < 10_000; i++)
            {
                double value = Balls.Normal();
                Assert.IsTrue(value >= 0.0 && value <= 1.0);
            }
        }
        [TestMethod]
        public void PhysicsInitialization_CreatesCorrectBalls()
        {
            Balls.Count = 100;
            Balls.Radius = 1;
            Balls.Speed = 10;

            LightPlane.Thickness = 1;
            LightPlane.Angle = 0;
            LightPlane.DistanceToScreen = 100;

            Physics.InitializePhysics();
            Balls.SpawnBalls(DistributionType.Uniform);

            Assert.AreEqual(Balls.Count, Balls.Array.Count());
            Assert.IsGreaterThan(0, Physics.Time);
        }
        [TestMethod]
        public void StressTest_LargeBallCount_DoesNotThrow()
        {
            Balls.Count = 1_000_000_000;
            Balls.Radius = 1;
            Balls.Speed = 1;

            LightPlane.Thickness = 2000;
            LightPlane.Angle = 15;
            LightPlane.DistanceToScreen = 500;

            try
            {
                Physics.InitializePhysics();
                Balls.SpawnBalls(DistributionType.Uniform);
            }
            catch (Exception ex)
            {
                Assert.Fail($"Exception thrown: {ex.Message}");
            }
        }
        [TestMethod]
        public void InitializePhysics_MinValidValues_WorksCorrectly()
        {
            Balls.Count = 1;
            Balls.Radius = 0.001;
            Balls.Speed = 0.001;

            LightPlane.Thickness = 0.001;
            LightPlane.Angle = 0;
            LightPlane.DistanceToScreen = 1;

            Physics.InitializePhysics();

            Assert.IsNotNull(Physics.SumArea);
            Assert.IsNotEmpty(Physics.SumArea);
            Assert.IsGreaterThan(0, Physics.Time);
        }
        [TestMethod]
        public void LargeBallsCount_DoesNotCrash()
        {
            Balls.Count = 100_000;
            Balls.Radius = 0.01;
            Balls.Speed = 10;

            LightPlane.Thickness = 1;
            LightPlane.Angle = 10;
            LightPlane.DistanceToScreen = 10;

            Physics.InitializePhysics();

            Assert.IsNotNull(Physics.SumArea);
        }
        [TestMethod]
        public void ExtremeSpeed_DoesNotOverflow()
        {
            Balls.Count = 10;
            Balls.Radius = 0.01;
            Balls.Speed = 3_000_000; // верхняя граница из UI

            LightPlane.Thickness = 1;
            LightPlane.Angle = 45;
            LightPlane.DistanceToScreen = 100;

            Physics.InitializePhysics();

            Assert.IsFalse(double.IsInfinity(Physics.Time));
            Assert.IsFalse(double.IsNaN(Physics.Time));
        }
        [TestMethod]
        public void EmptySumArea_DoesNotCrash()
        {
            // Имитируем состояние, когда расчёт не дал данных
            Physics.SumArea = Array.Empty<double>();

            double sum = 0;
            foreach (var v in Physics.SumArea)
                sum += v;

            Assert.AreEqual(0, sum);
        }

    }
}
