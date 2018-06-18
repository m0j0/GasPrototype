using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;
using MugenMvvmToolkit;
using NUnit.Framework;
using Prototype.Core.Controls.PipeFlowScheme;

namespace Prototype.Test.PipeFlowScheme
{
    [TestFixture]
    internal class Manifold2Test : UnitTestBase
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
                Assert.IsTrue(Extensions.PipeSegmentsFlowHasValue(pipe, false));
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
                Assert.IsTrue(Extensions.PipeSegmentsFlowHasValue(pipe, true));
            }
        }

        [Test]
        public void TestV1Open()
        {
            var manifold = new Manifold();

            manifold.Valve1.CanPassFlow = true;
            manifold.Valve2.CanPassFlow = false;
            manifold.UpdateGraph();

            Assert.IsTrue(Extensions.PipeHasSegmentFlow(manifold.Pipe1, true, true, true));
            Assert.IsTrue(Extensions.PipeHasSegmentFlow(manifold.Pipe2, true, true, true));
            Assert.IsTrue(Extensions.PipeHasSegmentFlow(manifold.Pipe3, true, true, true, true, true));
            Assert.IsTrue(Extensions.PipeHasSegmentFlow(manifold.Pipe4, true, false, false));
            Assert.IsTrue(Extensions.PipeHasSegmentFlow(manifold.Pipe5, true, true, true, true, true));
            Assert.IsTrue(Extensions.PipeHasSegmentFlow(manifold.Pipe6, false, false, false));
            Assert.IsTrue(Extensions.PipeHasSegmentFlow(manifold.Pipe7, false, false, false));
            Assert.IsTrue(Extensions.PipeHasSegmentFlow(manifold.Pipe8, true, true, true));
            Assert.IsTrue(Extensions.PipeHasSegmentFlow(manifold.Pipe9, true, false, false));
            Assert.IsTrue(Extensions.PipeHasSegmentFlow(manifold.Pipe10, true, true, true));
        }

        [Test]
        public void TestV2Open()
        {
            var manifold = new Manifold();

            manifold.Valve1.CanPassFlow = false;
            manifold.Valve2.CanPassFlow = true;
            manifold.UpdateGraph();

            Assert.IsTrue(Extensions.PipeHasSegmentFlow(manifold.Pipe1, true, true, true));
            Assert.IsTrue(Extensions.PipeHasSegmentFlow(manifold.Pipe2, true, true, true));
            Assert.IsTrue(Extensions.PipeHasSegmentFlow(manifold.Pipe3, true, true, true, true, true));
            Assert.IsTrue(Extensions.PipeHasSegmentFlow(manifold.Pipe4, true, true, true));
            Assert.IsTrue(Extensions.PipeHasSegmentFlow(manifold.Pipe5, true, false, false, false, false));
            Assert.IsTrue(Extensions.PipeHasSegmentFlow(manifold.Pipe6, true, true, true));
            Assert.IsTrue(Extensions.PipeHasSegmentFlow(manifold.Pipe7, true, true, true));
            Assert.IsTrue(Extensions.PipeHasSegmentFlow(manifold.Pipe8, false, false, true));
            Assert.IsTrue(Extensions.PipeHasSegmentFlow(manifold.Pipe9, true, true, true));
            Assert.IsTrue(Extensions.PipeHasSegmentFlow(manifold.Pipe10, true, true, true));
        }

        [Test]
        public void TestInvertedAllValvesClosed()
        {
            var manifold = new Manifold();
            manifold.InvertSourceDestination();

            foreach (var pipe in manifold.GetPipes())
            {
                Assert.IsTrue(Extensions.PipeSegmentsFlowHasValue(pipe, false));
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
                Assert.IsTrue(Extensions.PipeSegmentsFlowHasValue(pipe, true));
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

            Assert.IsTrue(Extensions.PipeHasSegmentFlow(manifold.Pipe1, true, true, true));
            Assert.IsTrue(Extensions.PipeHasSegmentFlow(manifold.Pipe2, true, true, true));
            Assert.IsTrue(Extensions.PipeHasSegmentFlow(manifold.Pipe3, true, true, true, true, true));
            Assert.IsTrue(Extensions.PipeHasSegmentFlow(manifold.Pipe4, true, false, false));
            Assert.IsTrue(Extensions.PipeHasSegmentFlow(manifold.Pipe5, true, true, true, true, true));
            Assert.IsTrue(Extensions.PipeHasSegmentFlow(manifold.Pipe6, false, false, false));
            Assert.IsTrue(Extensions.PipeHasSegmentFlow(manifold.Pipe7, false, false, false));
            Assert.IsTrue(Extensions.PipeHasSegmentFlow(manifold.Pipe8, true, true, true));
            Assert.IsTrue(Extensions.PipeHasSegmentFlow(manifold.Pipe9, true, false, false));
            Assert.IsTrue(Extensions.PipeHasSegmentFlow(manifold.Pipe10, true, true, true));
        }

        [Test]
        public void TestInvertedV2Open()
        {
            var manifold = new Manifold();
            manifold.InvertSourceDestination();

            manifold.Valve1.CanPassFlow = false;
            manifold.Valve2.CanPassFlow = true;
            manifold.UpdateGraph();

            Assert.IsTrue(Extensions.PipeHasSegmentFlow(manifold.Pipe1, true, true, true));
            Assert.IsTrue(Extensions.PipeHasSegmentFlow(manifold.Pipe2, true, true, true));
            Assert.IsTrue(Extensions.PipeHasSegmentFlow(manifold.Pipe3, true, true, true, true, true));
            Assert.IsTrue(Extensions.PipeHasSegmentFlow(manifold.Pipe4, true, true, true));
            Assert.IsTrue(Extensions.PipeHasSegmentFlow(manifold.Pipe5, true, false, false, false, false));
            Assert.IsTrue(Extensions.PipeHasSegmentFlow(manifold.Pipe6, true, true, true));
            Assert.IsTrue(Extensions.PipeHasSegmentFlow(manifold.Pipe7, true, true, true));
            Assert.IsTrue(Extensions.PipeHasSegmentFlow(manifold.Pipe8, false, false, true));
            Assert.IsTrue(Extensions.PipeHasSegmentFlow(manifold.Pipe9, true, true, true));
            Assert.IsTrue(Extensions.PipeHasSegmentFlow(manifold.Pipe10, true, true, true));
        }

        [Test]
        public void TestSegmentsSplitting()
        {
            var manifold = new Manifold();

            var cnn = typeof(ConnectorSegment);
            var line = typeof(LineSegment);
            Assert.IsTrue(Extensions.PipeHasSegmentTypes(manifold.Pipe1, cnn, line, cnn));
            Assert.IsTrue(Extensions.PipeHasSegmentTypes(manifold.Pipe2, cnn, line, cnn));
            Assert.IsTrue(Extensions.PipeHasSegmentTypes(manifold.Pipe3, cnn, line, cnn, line, cnn));
            Assert.IsTrue(Extensions.PipeHasSegmentTypes(manifold.Pipe4, cnn, line, cnn));
            Assert.IsTrue(Extensions.PipeHasSegmentTypes(manifold.Pipe5, cnn, line, cnn, line, cnn));
            Assert.IsTrue(Extensions.PipeHasSegmentTypes(manifold.Pipe6, cnn, line, cnn));
            Assert.IsTrue(Extensions.PipeHasSegmentTypes(manifold.Pipe7, cnn, line, cnn));
            Assert.IsTrue(Extensions.PipeHasSegmentTypes(manifold.Pipe8, cnn, line, cnn));
            Assert.IsTrue(Extensions.PipeHasSegmentTypes(manifold.Pipe9, cnn, line, cnn));
            Assert.IsTrue(Extensions.PipeHasSegmentTypes(manifold.Pipe10, cnn, line, cnn));
        }

        [Test]
        public void TestPipeHiding()
        {
            var manifold = new Manifold();
            manifold.HideRightPath();
            
            Assert.IsTrue(Extensions.PipeHasSegments(manifold.Pipe1, 3));
            Assert.IsTrue(Extensions.PipeHasSegments(manifold.Pipe2, 3));
            Assert.IsTrue(Extensions.PipeHasSegments(manifold.Pipe3, 5));
            Assert.IsTrue(Extensions.PipeDoesNotHaveSegments(manifold.Pipe4));
            Assert.IsTrue(Extensions.PipeHasSegments(manifold.Pipe5, 5));
            Assert.IsTrue(Extensions.PipeDoesNotHaveSegments(manifold.Pipe6));
            Assert.IsTrue(Extensions.PipeDoesNotHaveSegments(manifold.Pipe7));
            Assert.IsTrue(Extensions.PipeHasSegments(manifold.Pipe8, 3));
            Assert.IsTrue(Extensions.PipeDoesNotHaveSegments(manifold.Pipe9));
            Assert.IsTrue(Extensions.PipeHasSegments(manifold.Pipe10, 3));
        }

        [Test]
        public void TestHidedPipesFlow()
        {
            var manifold = new Manifold();
            manifold.Valve1.CanPassFlow = false;
            manifold.Valve2.CanPassFlow = true;
            manifold.HideRightPath();

            Assert.IsTrue(Extensions.PipeHasSegmentFlow(manifold.Pipe1, false, false, false));
            Assert.IsTrue(Extensions.PipeHasSegmentFlow(manifold.Pipe2, false, false, false));
            Assert.IsTrue(Extensions.PipeHasSegmentFlow(manifold.Pipe3, false, false, false, false, false));
            Assert.IsTrue(Extensions.PipeHasSegmentFlow(manifold.Pipe4));
            Assert.IsTrue(Extensions.PipeHasSegmentFlow(manifold.Pipe5, false, false, false, false, false));
            Assert.IsTrue(Extensions.PipeHasSegmentFlow(manifold.Pipe6));
            Assert.IsTrue(Extensions.PipeHasSegmentFlow(manifold.Pipe7));
            Assert.IsTrue(Extensions.PipeHasSegmentFlow(manifold.Pipe8, false, false, false));
            Assert.IsTrue(Extensions.PipeHasSegmentFlow(manifold.Pipe9));
            Assert.IsTrue(Extensions.PipeHasSegmentFlow(manifold.Pipe10, false, false, false));
        }

        [Test]
        public void TestIncomplitedPaths()
        {
            var manifold = new Manifold();
            manifold.Pipe7.IsVisible = false;
            manifold.Pipe9.IsVisible = false;
            manifold.UpdateGraph();

            Assert.IsTrue(Extensions.PipeIsNotFailed(manifold.Pipe1));
            Assert.IsTrue(Extensions.PipeIsNotFailed(manifold.Pipe2));
            Assert.IsTrue(Extensions.PipeIsNotFailed(manifold.Pipe3));
            Assert.IsTrue(Extensions.PipeIsFailed(manifold.Pipe4, FailType.DeadPath));
            Assert.IsTrue(Extensions.PipeIsNotFailed(manifold.Pipe5));
            Assert.IsTrue(Extensions.PipeIsFailed(manifold.Pipe6, FailType.DeadPath));
            Assert.IsTrue(Extensions.PipeIsEmpty(manifold.Pipe7));
            Assert.IsTrue(Extensions.PipeIsNotFailed(manifold.Pipe8));
            Assert.IsTrue(Extensions.PipeIsEmpty(manifold.Pipe9));
            Assert.IsTrue(Extensions.PipeIsNotFailed(manifold.Pipe10));
        }

        #endregion
    }
}