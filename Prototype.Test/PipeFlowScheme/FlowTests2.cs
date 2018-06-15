using System.Collections.Generic;
using System.Windows.Controls;
using MugenMvvmToolkit;
using NUnit.Framework;
using Prototype.Core.Controls.PipeFlowScheme;

namespace Prototype.Test.PipeFlowScheme
{
    [TestFixture]
    internal class FlowTests2 : UnitTestBase
    {
        #region Nested types

        // s1    s2    s3    s4    s5    s6    s7
        // |     |     |     |     |     |     |
        // |     |     |     |     |     |     |
        // v1    v2    v3    v4    v5    v6    v7
        // |     |     |     |     |     |     |
        // |     |     |     |     |     |     |
        // ------c1----c2----c3----c4----c5-----
        //             |
        //             |
        //             d1

        private class Manifold
        {
            public Manifold()
            {
                Container = new TestContainer();
                Container.Add(Pipe1 = new TestPipe(Container) {Left = 57, Top = 60, Orientation = Orientation.Vertical, Height = 105, Type = PipeType.Source});

                Container.Add(Valve1 = new TestValve(Container) {Left = 96, Top = 226, Orientation = Orientation.Vertical});

                UpdateGraph();
            }

            public TestContainer Container { get; }
            public FlowGraph Graph { get; private set; }
            public TestPipe Pipe1 { get; }
            public TestValve Valve1 { get; }

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
                Assert.IsTrue(Extensions.SegmentsFlowHasValue(pipe, false));
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
                Assert.IsTrue(Extensions.SegmentsFlowHasValue(pipe, true));
            }
        }

        #endregion
    }
}