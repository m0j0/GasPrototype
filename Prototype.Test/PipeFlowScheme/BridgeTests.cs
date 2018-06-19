using System.Windows.Controls;
using MugenMvvmToolkit;
using NUnit.Framework;
using Prototype.Core.Controls.PipeFlowScheme;

namespace Prototype.Test.PipeFlowScheme
{
    [TestFixture]
    internal class BridgeTests : UnitTestBase
    {
        #region Nested types

        // 1v        3v  
        // |         |   
        // |    4v   |   
        // |    |    |   
        // V1   V3   V2  
        // |    |    |   
        // |    5v   |   
        // |    |    |   
        // 2>---b-----
        //      |         
        //      |                                        

        private class Manifold
        {
            public Manifold()
            {
                Container = new TestContainer();

                Container.Add(Pipe1 = new TestPipe(Container) {Left = 82, Top = 10, Height = 261, Orientation = Orientation.Vertical, Type = PipeType.Source});
                Container.Add(Pipe2 = new TestPipe(Container) {Left = 82, Top = 266, Width = 139});
                Container.Add(Pipe3 = new TestPipe(Container) {Left = 216, Top = 10, Height = 261, Orientation = Orientation.Vertical, Type = PipeType.Destination});
                Container.Add(Pipe4 = new TestPipe(Container) {Left = 151, Top = 56, Height = 163, Orientation = Orientation.Vertical, Type = PipeType.Source});
                Container.Add(Pipe5 = new TestPipe(Container) {Left = 151, Top = 214, Height = 92, Orientation = Orientation.Vertical, Type = PipeType.Destination});

                Container.Add(Valve1 = new TestValve(Container) {Left = 66, Top = 86, Orientation = Orientation.Vertical});
                Container.Add(Valve2 = new TestValve(Container) {Left = 200, Top = 86, Orientation = Orientation.Vertical});
                Container.Add(Valve3 = new TestValve(Container) {Left = 135, Top = 136, Orientation = Orientation.Vertical});

                UpdateGraph();
            }

            public TestContainer Container { get; }
            public FlowGraph Graph { get; private set; }

            public TestPipe Pipe1 { get; }
            public TestPipe Pipe2 { get; }
            public TestPipe Pipe3 { get; }
            public TestPipe Pipe4 { get; }
            public TestPipe Pipe5 { get; }

            public TestValve Valve1 { get; }
            public TestValve Valve2 { get; }
            public TestValve Valve3 { get; }

            public void UpdateGraph()
            {
                Graph = Container.CreateGraph();
            }
        }

        #endregion

        #region Set up

        [SetUp]
        public void SetUp()
        {
            Initialize(new MugenContainer());
        }

        #endregion

        #region Tests

        [Test]
        public void TestAllValvesClosed()
        {
            var manifold = new Manifold();

            foreach (var pipe in manifold.Container.GetPipes())
            {
                Assert.IsTrue(pipe.PipeSegmentsFlowHasValue(false));
            }
        }

        [Test]
        public void TestAllValvesOpen()
        {
            var manifold = new Manifold();

            foreach (var valve in manifold.Container.GetValves())
            {
                valve.CanPassFlow = true;
            }

            manifold.UpdateGraph();

            foreach (var pipe in manifold.Container.GetPipes())
            {
                Assert.IsTrue(pipe.PipeSegmentsFlowHasValue(true));
            }
        }

        [Test]
        public void Test_V3_Open()
        {
            var manifold = new Manifold();

            manifold.Valve3.CanPassFlow = true;

            manifold.UpdateGraph();

            Assert.IsTrue(manifold.Pipe1.PipeHasSegmentFlow(false, false, false, false, false));
            Assert.IsTrue(manifold.Pipe2.PipeHasSegmentFlow(false, false, false));
            Assert.IsTrue(manifold.Pipe3.PipeHasSegmentFlow(false, false, false, false, false));
            Assert.IsTrue(manifold.Pipe4.PipeHasSegmentFlow(true, true, true, true, true));
            Assert.IsTrue(manifold.Pipe5.PipeHasSegmentFlow(true, true, true, true, true));
        }

        [Test]
        public void Test_V1V2_Open()
        {
            var manifold = new Manifold();

            manifold.Valve1.CanPassFlow = true;
            manifold.Valve2.CanPassFlow = true;

            manifold.UpdateGraph();

            Assert.IsTrue(manifold.Pipe1.PipeHasSegmentFlow(true, true, true, true, true));
            Assert.IsTrue(manifold.Pipe2.PipeHasSegmentFlow(true, true, true));
            Assert.IsTrue(manifold.Pipe3.PipeHasSegmentFlow(true, true, true, true, true));
            Assert.IsTrue(manifold.Pipe4.PipeHasSegmentFlow(false, false, false, false, false));
            Assert.IsTrue(manifold.Pipe5.PipeHasSegmentFlow(false, false, false, false, false));
        }

        [Test]
        public void Test_V1_Open()
        {
            var manifold = new Manifold();

            manifold.Valve1.CanPassFlow = true;

            manifold.UpdateGraph();

            foreach (var pipe in manifold.Container.GetPipes())
            {
                Assert.IsTrue(pipe.PipeSegmentsFlowHasValue(false));
            }
        }

        [Test]
        public void TestSegments()
        {
            var manifold = new Manifold();

            manifold.Valve1.CanPassFlow = true;

            manifold.UpdateGraph();

            var cnn = typeof(ConnectorSegment);
            var lin = typeof(LineSegment);
            var brg = typeof(BridgeSegment);
            Assert.IsTrue(manifold.Pipe1.PipeHasSegmentTypes(cnn, lin, cnn, lin, cnn));
            Assert.IsTrue(manifold.Pipe2.PipeHasSegmentTypes(cnn, lin, cnn));
            Assert.IsTrue(manifold.Pipe3.PipeHasSegmentTypes(cnn, lin, cnn, lin, cnn));
            Assert.IsTrue(manifold.Pipe4.PipeHasSegmentTypes(cnn, lin, cnn, lin, cnn));
            Assert.IsTrue(manifold.Pipe5.PipeHasSegmentTypes(cnn, lin, brg, lin, cnn));
        }

        #endregion
    }
}