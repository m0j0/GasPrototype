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

        // 1v        3v
        // |         |
        // |         |
        // 2>----4v---------5v
        //       |          |
        //       |          |
        //       v1         v2
        //       |          |
        //       |          |
        //       6>---7v-----
        //            |
        //            |
        //            |

        private class Manifold
        {
            public Manifold()
            {
                Container = new TestContainer();
                Container.Add(Pipe1 = new TestPipe(Container) {Left = 1, Top = 1, Orientation = Orientation.Vertical, Height = 105, IsSource = true});
                Container.Add(Pipe2 = new TestPipe(Container) {Left = 1, Top = 101, Width = 275});
                Container.Add(Pipe3 = new TestPipe(Container) {Left = 158, Top = 1, Orientation = Orientation.Vertical, Height = 105, IsSource = true});
                Container.Add(Pipe4 = new TestPipe(Container) {Left = 56, Top = 101, Orientation = Orientation.Vertical, Height = 155});
                Container.Add(Pipe5 = new TestPipe(Container) {Left = 271, Top = 101, Orientation = Orientation.Vertical, Height = 155});
                Container.Add(Pipe6 = new TestPipe(Container) {Left = 56, Top = 251, Width = 220});
                Container.Add(Pipe7 = new TestPipe(Container) {Left = 161, Top = 251, Orientation = Orientation.Vertical, Height = 68, IsDestination = true});

                Container.Add(Valve1 = new TestValve(Container) {Left = 40, Top = 167, Orientation = Orientation.Vertical});
                Container.Add(Valve2 = new TestValve(Container) {Left = 255, Top = 167, Orientation = Orientation.Vertical});

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
        }

        private static bool SegmentsHasValue(IPipe pipe, bool v)
        {
            foreach (var segment in pipe.Segments)
            {
                if (v ? segment.FlowDirection != FlowDirection.Both : segment.FlowDirection != FlowDirection.None)
                {
                    return false;
                }
            }

            return true;
        }
    }
}