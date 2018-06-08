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
                Container.Add(Pipe1 = new TestPipe(Container) { Left = 57, Top = 60, Orientation = Orientation.Vertical, Height = 105, IsSource = true });
                Container.Add(Pipe2 = new TestPipe(Container) { Left = 214, Top = 60, Orientation = Orientation.Vertical, Height = 105, IsSource = true });
                Container.Add(Pipe3 = new TestPipe(Container) { Left = 57, Top = 160, Width = 162 });
                Container.Add(Pipe4 = new TestPipe(Container) { Left = 214, Top = 160, Width = 118 });
                Container.Add(Pipe5 = new TestPipe(Container) { Left = 112, Top = 160, Orientation = Orientation.Vertical, Height = 155 });
                Container.Add(Pipe6 = new TestPipe(Container) { Left = 327, Top = 160, Orientation = Orientation.Vertical, Height = 92 });
                Container.Add(Pipe7 = new TestPipe(Container) { Left = 327, Top = 247, Orientation = Orientation.Vertical, Height = 68 });
                Container.Add(Pipe8 = new TestPipe(Container) { Left = 112, Top = 310, Width = 110 });
                Container.Add(Pipe9 = new TestPipe(Container) { Left = 217, Top = 310, Width = 115 });
                Container.Add(Pipe10 = new TestPipe(Container) { Left = 217, Top = 310, Orientation = Orientation.Vertical, Height = 68, IsDestination = true });

                Container.Add(Valve1 = new TestValve(Container) {Left = 96, Top = 226, Orientation = Orientation.Vertical});
                Container.Add(Valve2 = new TestValve(Container) {Left = 311, Top = 226, Orientation = Orientation.Vertical});

                Graph = Container.CreateGraph();
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
        }

        #endregion


        [SetUp]
        public void SetUp()
        {
            Initialize(new MugenContainer());
        }

        [Test]
        public void TestFindPipes()
        {
            var manifold = new Manifold();

            Assert.IsTrue(SegmentsHasValue(manifold.Pipe1, false));
            Assert.IsTrue(SegmentsHasValue(manifold.Pipe2, false));
            Assert.IsTrue(SegmentsHasValue(manifold.Pipe3, false));
            Assert.IsTrue(SegmentsHasValue(manifold.Pipe4, false));
            Assert.IsTrue(SegmentsHasValue(manifold.Pipe5, false));
            Assert.IsTrue(SegmentsHasValue(manifold.Pipe6, false));
            Assert.IsTrue(SegmentsHasValue(manifold.Pipe7, false));
            Assert.IsTrue(SegmentsHasValue(manifold.Pipe8, false));
            Assert.IsTrue(SegmentsHasValue(manifold.Pipe9, false));
            Assert.IsTrue(SegmentsHasValue(manifold.Pipe10, false));
            
            manifold.Valve1.CanPassFlow = true;
            manifold.Valve2.CanPassFlow = true;
            manifold.UpdateGraph();
            
            Assert.IsTrue(SegmentsHasValue(manifold.Pipe1, true));
            Assert.IsTrue(SegmentsHasValue(manifold.Pipe2, true));
            Assert.IsTrue(SegmentsHasValue(manifold.Pipe3, true));
            Assert.IsTrue(SegmentsHasValue(manifold.Pipe4, true));
            Assert.IsTrue(SegmentsHasValue(manifold.Pipe5, true));
            Assert.IsTrue(SegmentsHasValue(manifold.Pipe6, true));
            Assert.IsTrue(SegmentsHasValue(manifold.Pipe7, true));
            Assert.IsTrue(SegmentsHasValue(manifold.Pipe8, true));
            Assert.IsTrue(SegmentsHasValue(manifold.Pipe9, true));
            Assert.IsTrue(SegmentsHasValue(manifold.Pipe10, true));
        }

        private static bool SegmentsHasValue(IPipe pipe, bool v)
        {
            foreach (var segment in pipe.Segments)
            {
                if (segment.HasFlow != v)
                {
                    return false;
                }
            }

            return true;
        }
    }
}