using System.Collections.Generic;
using System.Windows.Controls;
using MugenMvvmToolkit;
using NUnit.Framework;
using Prototype.Core.Controls.PipeFlowScheme;

namespace Prototype.Test.PipeFlowScheme
{
    [TestFixture]
    internal class FlowTests1 : UnitTestBase
    {
        #region Nested types

        //      12v             13v
        //      |               |
        //      |               |
        //      v7              v8
        //      |               |
        //      |               |
        // 2>---6v---7v    10>--11v--3v
        // 1v   |    |     9v   |    |
        // |    |    |     |    |    |
        // v1   v2   v3    v4   v5   v6
        // |    |    |     |    |    |
        // |    |    |     8>---|    |
        // |    |    |     |         |
        // |    5>---4v-----         |
        // |         |               |
        // |         |               |

        private class Manifold
        {
            public Manifold()
            {
                Container = new TestContainer();
                Container.Add(Pipe1 = new TestPipe(Container) {Left = 30, Top = 111, Orientation = Orientation.Vertical, Height = 165, Type = PipeType.Source});
                Container.Add(Pipe2 = new TestPipe(Container) {Left = 30, Top = 111, Width = 156});
                Container.Add(Pipe3 = new TestPipe(Container) {Left = 441, Top = 111, Orientation = Orientation.Vertical, Height = 165, Type = PipeType.Source});
                Container.Add(Pipe4 = new TestPipe(Container) {Left = 181, Top = 226, Orientation = Orientation.Vertical, Height = 50, Type = PipeType.Source});
                Container.Add(Pipe5 = new TestPipe(Container) {Left = 99, Top = 226, Width = 174});
                Container.Add(Pipe6 = new TestPipe(Container) {Left = 99, Top = 111, Orientation = Orientation.Vertical, Height = 120});
                Container.Add(Pipe7 = new TestPipe(Container) {Left = 181, Top = 111, Orientation = Orientation.Vertical, Height = 120});
                Container.Add(Pipe8 = new TestPipe(Container) {Left = 268, Top = 210, Width = 84});
                Container.Add(Pipe9 = new TestPipe(Container) {Left = 268, Top = 111, Orientation = Orientation.Vertical, Height = 120});
                Container.Add(Pipe10 = new TestPipe(Container) {Left = 268, Top = 111, Width = 178});
                Container.Add(Pipe11 = new TestPipe(Container) {Left = 347, Top = 111, Orientation = Orientation.Vertical, Height = 104});
                Container.Add(Pipe12 = new TestPipe(Container) {Left = 99, Top = 16, Orientation = Orientation.Vertical, Height = 100, Type = PipeType.Destination});
                Container.Add(Pipe13 = new TestPipe(Container) {Left = 347, Top = 26, Orientation = Orientation.Vertical, Height = 90, Type = PipeType.Destination});

                Container.Add(Valve1 = new TestValve(Container) {Left = 14, Top = 133, Orientation = Orientation.Vertical});
                Container.Add(Valve2 = new TestValve(Container) {Left = 83, Top = 133, Orientation = Orientation.Vertical});
                Container.Add(Valve3 = new TestValve(Container) {Left = 165, Top = 133, Orientation = Orientation.Vertical});
                Container.Add(Valve4 = new TestValve(Container) {Left = 252, Top = 133, Orientation = Orientation.Vertical});
                Container.Add(Valve5 = new TestValve(Container) {Left = 331, Top = 133, Orientation = Orientation.Vertical});
                Container.Add(Valve6 = new TestValve(Container) {Left = 425, Top = 133, Orientation = Orientation.Vertical});
                Container.Add(Valve7 = new TestValve(Container) {Left = 83, Top = 41, Orientation = Orientation.Vertical});
                Container.Add(Valve8 = new TestValve(Container) {Left = 331, Top = 41, Orientation = Orientation.Vertical});

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
            public TestPipe Pipe11 { get; }
            public TestPipe Pipe12 { get; }
            public TestPipe Pipe13 { get; }
            public TestValve Valve1 { get; }
            public TestValve Valve2 { get; }
            public TestValve Valve3 { get; }
            public TestValve Valve4 { get; }
            public TestValve Valve5 { get; }
            public TestValve Valve6 { get; }
            public TestValve Valve7 { get; }
            public TestValve Valve8 { get; }

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