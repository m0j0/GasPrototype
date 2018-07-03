using System.Collections.Generic;
using System.Linq;
using System.Windows.Controls;
using MugenMvvmToolkit;
using NUnit.Framework;
using Prototype.Core.Controls.PipeFlowScheme;

namespace Prototype.Test.PipeFlowScheme
{
    [TestFixture]
    internal class ManifoldTest : UnitTestBase
    {
        #region Nested types

        // 1v        2v
        // |         |
        // |         |
        // 3>----5v--4>-----6v
        //       |          |
        //       |          |
        //       v1         7v v2
        //       |          |
        //       |          |
        //       8>---9>-----
        //            10v
        //            |
        //            |

        private class Manifold
        {
            public Manifold()
            {
                Container = new TestContainer();
                Container.Add(Pipe1 = new TestPipe(Container) {Left = 57, Top = 60, Orientation = Orientation.Vertical, Height = 105, Type = PipeType.Source});
                Container.Add(Pipe2 = new TestPipe(Container) {Left = 214, Top = 60, Orientation = Orientation.Vertical, Height = 105, Type = PipeType.Source});
                Container.Add(Pipe3 = new TestPipe(Container) {Left = 57, Top = 160, Width = 162});
                Container.Add(Pipe4 = new TestPipe(Container) {Left = 214, Top = 160, Width = 118});
                Container.Add(Pipe5 = new TestPipe(Container) {Left = 112, Top = 160, Orientation = Orientation.Vertical, Height = 155});
                Container.Add(Pipe6 = new TestPipe(Container) {Left = 327, Top = 160, Orientation = Orientation.Vertical, Height = 92});
                Container.Add(Pipe7 = new TestPipe(Container) {Left = 327, Top = 247, Orientation = Orientation.Vertical, Height = 68});
                Container.Add(Pipe8 = new TestPipe(Container) {Left = 112, Top = 310, Width = 110});
                Container.Add(Pipe9 = new TestPipe(Container) {Left = 217, Top = 310, Width = 115});
                Container.Add(Pipe10 = new TestPipe(Container) {Left = 217, Top = 310, Orientation = Orientation.Vertical, Height = 68, Type = PipeType.Destination});

                Container.Add(Valve1 = new TestValve(Container) {Left = 96, Top = 226, Orientation = Orientation.Vertical});
                Container.Add(Valve2 = new TestValve(Container) {Left = 311, Top = 226, Orientation = Orientation.Vertical});

                UpdateGraph();
            }

            public TestContainer Container { get; }
            public FlowGraph Graph { get; private set; }
            public TestPipe Pipe1 { get; }
            public TestPipe Pipe2 { get; }
            public TestPipe Pipe3 { get; }
            public TestPipe Pipe4 { get; }
            public TestPipe Pipe5 { get; }
            public TestPipe Pipe6 { get; }
            public TestPipe Pipe7 { get; }
            public TestPipe Pipe8 { get; }
            public TestPipe Pipe9 { get; }
            public TestPipe Pipe10 { get; }
            public TestValve Valve1 { get; }
            public TestValve Valve2 { get; }

            public void UpdateGraph()
            {
                Graph = Container.CreateGraph();
            }

            public void InvertSourceDestination()
            {
                Pipe1.Type = PipeType.Destination;
                Pipe2.Type = PipeType.Destination;
                Pipe10.Type = PipeType.Source;
                UpdateGraph();
            }

            public void HideRightPath()
            {
                Pipe4.IsVisible = false;
                Pipe6.IsVisible = false;
                Pipe7.IsVisible = false;
                Pipe9.IsVisible = false;
                Valve2.IsVisible = false;
                UpdateGraph();
            }

            public IEnumerable<TestPipe> GetPipes()
            {
                return Container.GetPipes();
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

            foreach (var pipe in manifold.GetPipes())
            {
                Assert.IsTrue(pipe.PipeSegmentsFlowHasValue(false));
            }
        }

        [Test]
        public void TestAllValvesOpen()
        {
            var manifold = new Manifold();

            manifold.Valve1.CanPassFlow = true;
            manifold.Valve2.CanPassFlow = true;
            manifold.UpdateGraph();

            foreach (var pipe in manifold.GetPipes())
            {
                Assert.IsTrue(pipe.PipeSegmentsFlowHasValue(true));
            }
        }

        [Test]
        public void TestV1Open()
        {
            var manifold = new Manifold();

            manifold.Valve1.CanPassFlow = true;
            manifold.Valve2.CanPassFlow = false;
            manifold.UpdateGraph();

            Assert.IsTrue(manifold.Pipe1.PipeHasSegmentFlow(true, true, true));
            Assert.IsTrue(manifold.Pipe2.PipeHasSegmentFlow(true, true, true));
            Assert.IsTrue(manifold.Pipe3.PipeHasSegmentFlow(true, true, true, true, true));
            Assert.IsTrue(manifold.Pipe4.PipeHasSegmentFlow(true, false, false));
            Assert.IsTrue(manifold.Pipe5.PipeHasSegmentFlow(true, true, true, true, true));
            Assert.IsTrue(manifold.Pipe6.PipeHasSegmentFlow(false, false, false));
            Assert.IsTrue(manifold.Pipe7.PipeHasSegmentFlow(false, false, false));
            Assert.IsTrue(manifold.Pipe8.PipeHasSegmentFlow(true, true, true));
            Assert.IsTrue(manifold.Pipe9.PipeHasSegmentFlow(true, false, false));
            Assert.IsTrue(manifold.Pipe10.PipeHasSegmentFlow(true, true, true));
        }

        [Test]
        public void TestV2Open()
        {
            var manifold = new Manifold();

            manifold.Valve1.CanPassFlow = false;
            manifold.Valve2.CanPassFlow = true;
            manifold.UpdateGraph();

            Assert.IsTrue(manifold.Pipe1.PipeHasSegmentFlow(true, true, true));
            Assert.IsTrue(manifold.Pipe2.PipeHasSegmentFlow(true, true, true));
            Assert.IsTrue(manifold.Pipe3.PipeHasSegmentFlow(true, true, true, true, true));
            Assert.IsTrue(manifold.Pipe4.PipeHasSegmentFlow(true, true, true));
            Assert.IsTrue(manifold.Pipe5.PipeHasSegmentFlow(true, false, false, false, false));
            Assert.IsTrue(manifold.Pipe6.PipeHasSegmentFlow(true, true, true));
            Assert.IsTrue(manifold.Pipe7.PipeHasSegmentFlow(true, true, true));
            Assert.IsTrue(manifold.Pipe8.PipeHasSegmentFlow(false, false, true));
            Assert.IsTrue(manifold.Pipe9.PipeHasSegmentFlow(true, true, true));
            Assert.IsTrue(manifold.Pipe10.PipeHasSegmentFlow(true, true, true));
        }

        [Test]
        public void TestInvertedAllValvesClosed()
        {
            var manifold = new Manifold();
            manifold.InvertSourceDestination();

            foreach (var pipe in manifold.GetPipes())
            {
                Assert.IsTrue(pipe.PipeSegmentsFlowHasValue(false));
            }
        }

        [Test]
        public void TestInvertedAllValvesOpen()
        {
            var manifold = new Manifold();
            manifold.InvertSourceDestination();

            manifold.Valve1.CanPassFlow = true;
            manifold.Valve2.CanPassFlow = true;
            manifold.UpdateGraph();

            foreach (var pipe in manifold.GetPipes())
            {
                Assert.IsTrue(pipe.PipeSegmentsFlowHasValue(true));
            }
        }

        [Test]
        public void TestInvertedV1Open()
        {
            var manifold = new Manifold();
            manifold.InvertSourceDestination();

            manifold.Valve1.CanPassFlow = true;
            manifold.Valve2.CanPassFlow = false;
            manifold.UpdateGraph();

            Assert.IsTrue(manifold.Pipe1.PipeHasSegmentFlow(true, true, true));
            Assert.IsTrue(manifold.Pipe2.PipeHasSegmentFlow(true, true, true));
            Assert.IsTrue(manifold.Pipe3.PipeHasSegmentFlow(true, true, true, true, true));
            Assert.IsTrue(manifold.Pipe4.PipeHasSegmentFlow(true, false, false));
            Assert.IsTrue(manifold.Pipe5.PipeHasSegmentFlow(true, true, true, true, true));
            Assert.IsTrue(manifold.Pipe6.PipeHasSegmentFlow(false, false, false));
            Assert.IsTrue(manifold.Pipe7.PipeHasSegmentFlow(false, false, false));
            Assert.IsTrue(manifold.Pipe8.PipeHasSegmentFlow(true, true, true));
            Assert.IsTrue(manifold.Pipe9.PipeHasSegmentFlow(true, false, false));
            Assert.IsTrue(manifold.Pipe10.PipeHasSegmentFlow(true, true, true));
        }

        [Test]
        public void TestInvertedV2Open()
        {
            var manifold = new Manifold();
            manifold.InvertSourceDestination();

            manifold.Valve1.CanPassFlow = false;
            manifold.Valve2.CanPassFlow = true;
            manifold.UpdateGraph();

            Assert.IsTrue(manifold.Pipe1.PipeHasSegmentFlow(true, true, true));
            Assert.IsTrue(manifold.Pipe2.PipeHasSegmentFlow(true, true, true));
            Assert.IsTrue(manifold.Pipe3.PipeHasSegmentFlow(true, true, true, true, true));
            Assert.IsTrue(manifold.Pipe4.PipeHasSegmentFlow(true, true, true));
            Assert.IsTrue(manifold.Pipe5.PipeHasSegmentFlow(true, false, false, false, false));
            Assert.IsTrue(manifold.Pipe6.PipeHasSegmentFlow(true, true, true));
            Assert.IsTrue(manifold.Pipe7.PipeHasSegmentFlow(true, true, true));
            Assert.IsTrue(manifold.Pipe8.PipeHasSegmentFlow(false, false, true));
            Assert.IsTrue(manifold.Pipe9.PipeHasSegmentFlow(true, true, true));
            Assert.IsTrue(manifold.Pipe10.PipeHasSegmentFlow(true, true, true));
        }

        [Test]
        public void TestSegmentsSplitting()
        {
            var manifold = new Manifold();

            var cnn = typeof(ConnectorSegment);
            var line = typeof(LineSegment);
            Assert.IsTrue(manifold.Pipe1.PipeHasSegmentTypes(cnn, line, cnn));
            Assert.IsTrue(manifold.Pipe2.PipeHasSegmentTypes(cnn, line, cnn));
            Assert.IsTrue(manifold.Pipe3.PipeHasSegmentTypes(cnn, line, cnn, line, cnn));
            Assert.IsTrue(manifold.Pipe4.PipeHasSegmentTypes(cnn, line, cnn));
            Assert.IsTrue(manifold.Pipe5.PipeHasSegmentTypes(cnn, line, cnn, line, cnn));
            Assert.IsTrue(manifold.Pipe6.PipeHasSegmentTypes(cnn, line, cnn));
            Assert.IsTrue(manifold.Pipe7.PipeHasSegmentTypes(cnn, line, cnn));
            Assert.IsTrue(manifold.Pipe8.PipeHasSegmentTypes(cnn, line, cnn));
            Assert.IsTrue(manifold.Pipe9.PipeHasSegmentTypes(cnn, line, cnn));
            Assert.IsTrue(manifold.Pipe10.PipeHasSegmentTypes(cnn, line, cnn));
        }

        [Test]
        public void TestPipeHiding()
        {
            var manifold = new Manifold();
            manifold.HideRightPath();

            Assert.IsTrue(manifold.Pipe1.PipeHasSegments(3));
            Assert.IsTrue(manifold.Pipe2.PipeHasSegments(3));
            Assert.IsTrue(manifold.Pipe3.PipeHasSegments(5));
            Assert.IsTrue(manifold.Pipe4.PipeDoesNotHaveSegments());
            Assert.IsTrue(manifold.Pipe5.PipeHasSegments(5));
            Assert.IsTrue(manifold.Pipe6.PipeDoesNotHaveSegments());
            Assert.IsTrue(manifold.Pipe7.PipeDoesNotHaveSegments());
            Assert.IsTrue(manifold.Pipe8.PipeHasSegments(3));
            Assert.IsTrue(manifold.Pipe9.PipeDoesNotHaveSegments());
            Assert.IsTrue(manifold.Pipe10.PipeHasSegments(3));
        }

        [Test]
        public void TestHidedPipesFlow()
        {
            var manifold = new Manifold();
            manifold.Valve1.CanPassFlow = false;
            manifold.Valve2.CanPassFlow = true;
            manifold.HideRightPath();

            Assert.IsTrue(manifold.Pipe1.PipeHasSegmentFlow(false, false, false));
            Assert.IsTrue(manifold.Pipe2.PipeHasSegmentFlow(false, false, false));
            Assert.IsTrue(manifold.Pipe3.PipeHasSegmentFlow(false, false, false, false, false));
            Assert.IsTrue(manifold.Pipe4.PipeHasSegmentFlow());
            Assert.IsTrue(manifold.Pipe5.PipeHasSegmentFlow(false, false, false, false, false));
            Assert.IsTrue(manifold.Pipe6.PipeHasSegmentFlow());
            Assert.IsTrue(manifold.Pipe7.PipeHasSegmentFlow());
            Assert.IsTrue(manifold.Pipe8.PipeHasSegmentFlow(false, false, false));
            Assert.IsTrue(manifold.Pipe9.PipeHasSegmentFlow());
            Assert.IsTrue(manifold.Pipe10.PipeHasSegmentFlow(false, false, false));
        }

        [Test]
        public void TestIncomplitedPaths()
        {
            var manifold = new Manifold();
            manifold.Pipe7.IsVisible = false;
            manifold.Pipe9.IsVisible = false;
            manifold.UpdateGraph();

            Assert.IsTrue(manifold.Pipe1.PipeIsNotFailed());
            Assert.IsTrue(manifold.Pipe2.PipeIsNotFailed());
            Assert.IsTrue(manifold.Pipe3.PipeIsNotFailed());
            Assert.IsTrue(manifold.Pipe4.PipeIsFailed(FailType.DeadPath));
            Assert.IsTrue(manifold.Pipe5.PipeIsNotFailed());
            Assert.IsTrue(manifold.Pipe6.PipeIsFailed(FailType.DeadPath));
            Assert.IsTrue(manifold.Pipe7.PipeIsEmpty());
            Assert.IsTrue(manifold.Pipe8.PipeIsNotFailed());
            Assert.IsTrue(manifold.Pipe9.PipeIsEmpty());
            Assert.IsTrue(manifold.Pipe10.PipeIsNotFailed());
        }

        [Test]
        public void TestSegmentsLength()
        {
            var manifold = new Manifold();

            Assert.IsTrue(manifold.Pipe1.PipeSegmentsHasLength(5, 95, 5));
            Assert.IsTrue(manifold.Pipe2.PipeSegmentsHasLength(5, 95, 5));
            Assert.IsTrue(manifold.Pipe3.PipeSegmentsHasLength(5, 50, 5, 97, 5));
            Assert.IsTrue(manifold.Pipe4.PipeSegmentsHasLength(5, 108, 5));
            Assert.IsTrue(manifold.Pipe5.PipeSegmentsHasLength(5, 61, 5, 79, 5));
            Assert.IsTrue(manifold.Pipe6.PipeSegmentsHasLength(5, 82, 5));
            Assert.IsTrue(manifold.Pipe7.PipeSegmentsHasLength(5, 58, 5));
            Assert.IsTrue(manifold.Pipe8.PipeSegmentsHasLength(5, 100, 5));
            Assert.IsTrue(manifold.Pipe9.PipeSegmentsHasLength(5, 105, 5));
            Assert.IsTrue(manifold.Pipe10.PipeSegmentsHasLength(5, 58, 5));
        }

        [Test]
        public void TestInvertedSegmentsLength()
        {
            var manifold = new Manifold();
            manifold.InvertSourceDestination();

            Assert.IsTrue(manifold.Pipe1.PipeSegmentsHasLength(5, 95, 5));
            Assert.IsTrue(manifold.Pipe2.PipeSegmentsHasLength(5, 95, 5));
            Assert.IsTrue(manifold.Pipe3.PipeSegmentsHasLength(5, 50, 5, 97, 5));
            Assert.IsTrue(manifold.Pipe4.PipeSegmentsHasLength(5, 108, 5));
            Assert.IsTrue(manifold.Pipe5.PipeSegmentsHasLength(5, 61, 5, 79, 5));
            Assert.IsTrue(manifold.Pipe6.PipeSegmentsHasLength(5, 82, 5));
            Assert.IsTrue(manifold.Pipe7.PipeSegmentsHasLength(5, 58, 5));
            Assert.IsTrue(manifold.Pipe8.PipeSegmentsHasLength(5, 100, 5));
            Assert.IsTrue(manifold.Pipe9.PipeSegmentsHasLength(5, 105, 5));
            Assert.IsTrue(manifold.Pipe10.PipeSegmentsHasLength(5, 58, 5));
        }

        [Test]
        public void TestGraphsEquality()
        {
            var manifold = new Manifold();

            var pipes = manifold.Container.GetPipes().ToArray();
            var valves = manifold.Container.GetValves().ToArray();

            Assert.IsTrue(manifold.Graph.Equals(pipes, valves));

            manifold.Pipe1.Height += 1;
            Assert.IsFalse(manifold.Graph.Equals(pipes, valves));
            
            manifold.Pipe1.Height -= 1;
            Assert.IsTrue(manifold.Graph.Equals(pipes, valves));

            manifold.Pipe1.Top += 1;
            Assert.IsFalse(manifold.Graph.Equals(pipes, valves));
            
            manifold.Pipe1.Top -= 1;
            Assert.IsTrue(manifold.Graph.Equals(pipes, valves));

            manifold.Valve1.Top += 1;
            Assert.IsFalse(manifold.Graph.Equals(pipes, valves));
            
            manifold.Valve1.Top -= 1;
            Assert.IsTrue(manifold.Graph.Equals(pipes, valves));

            manifold.Pipe1.IsVisible = false;
            Assert.IsFalse(manifold.Graph.Equals(pipes, valves));

            manifold.Pipe1.IsVisible = true;
            Assert.IsTrue(manifold.Graph.Equals(pipes, valves));

            manifold.Valve1.IsVisible = false;
            Assert.IsFalse(manifold.Graph.Equals(pipes, valves));
            
            manifold.Valve1.IsVisible = true;
            Assert.IsTrue(manifold.Graph.Equals(pipes, valves));
        }

        #endregion
    }
}