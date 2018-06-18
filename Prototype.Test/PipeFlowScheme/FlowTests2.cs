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

        // 1v    3v    5v    7v    9v    11v   13v
        // |     |     |     |     |     |     |
        // |     |     |     |     |     |     |
        // v1    v2    v3    v4    v5    v6    v7
        // |     |     |     |     |     |     |
        // |     |     |     |     |     |     |
        // 2>----4>----6>----8>----10>---12>----
        //             14v
        //             |
        //             |

        private class Manifold
        {
            public Manifold()
            {
                Container = new TestContainer();

                Container.Add(Pipe1 = new TestPipe(Container) { Left = 206, Top = 14, Orientation = Orientation.Vertical, Height = 122, Type = PipeType.Source });
                Container.Add(Pipe2 = new TestPipe(Container) { Left = 206, Top = 131, Width = 75 });
                Container.Add(Pipe3 = new TestPipe(Container) { Left = 276, Top = 14, Orientation = Orientation.Vertical, Height = 122, Type = PipeType.Source });
                Container.Add(Pipe4 = new TestPipe(Container) { Left = 276, Top = 131, Width = 77 });
                Container.Add(Pipe5 = new TestPipe(Container) { Left = 348, Top = 14, Orientation = Orientation.Vertical, Height = 122, Type = PipeType.Source });
                Container.Add(Pipe6 = new TestPipe(Container) { Left = 348, Top = 131, Width = 77 });
                Container.Add(Pipe7 = new TestPipe(Container) { Left = 420, Top = 14, Orientation = Orientation.Vertical, Height = 122, Type = PipeType.Source });
                Container.Add(Pipe8 = new TestPipe(Container) { Left = 420, Top = 131, Width = 76 });
                Container.Add(Pipe9 = new TestPipe(Container) { Left = 491, Top = 14, Orientation = Orientation.Vertical, Height = 122, Type = PipeType.Source });
                Container.Add(Pipe10 = new TestPipe(Container) { Left = 491, Top = 131, Width = 77 });
                Container.Add(Pipe11 = new TestPipe(Container) { Left = 563, Top = 14, Orientation = Orientation.Vertical, Height = 122, Type = PipeType.Source });
                Container.Add(Pipe12 = new TestPipe(Container) { Left = 563, Top = 131, Width = 77 });
                Container.Add(Pipe13 = new TestPipe(Container) { Left = 635, Top = 14, Orientation = Orientation.Vertical, Height = 122, Type = PipeType.Source });
                Container.Add(Pipe14 = new TestPipe(Container) { Left = 348, Top = 131, Orientation = Orientation.Vertical, Type = PipeType.Destination });

                Container.Add(Valve1 = new TestValve(Container) { Left = 190, Top = 71, Orientation = Orientation.Vertical });
                Container.Add(Valve2 = new TestValve(Container) { Left = 260, Top = 71, Orientation = Orientation.Vertical });
                Container.Add(Valve3 = new TestValve(Container) { Left = 332, Top = 71, Orientation = Orientation.Vertical });
                Container.Add(Valve4 = new TestValve(Container) { Left = 404, Top = 71, Orientation = Orientation.Vertical });
                Container.Add(Valve5 = new TestValve(Container) { Left = 475, Top = 71, Orientation = Orientation.Vertical });
                Container.Add(Valve6 = new TestValve(Container) { Left = 547, Top = 71, Orientation = Orientation.Vertical });
                Container.Add(Valve7 = new TestValve(Container) { Left = 619, Top = 71, Orientation = Orientation.Vertical });

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
            public TestPipe Pipe14 { get; }

            public TestValve Valve1 { get; }
            public TestValve Valve2 { get; }
            public TestValve Valve3 { get; }
            public TestValve Valve4 { get; }
            public TestValve Valve5 { get; }
            public TestValve Valve6 { get; }
            public TestValve Valve7 { get; }

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
                Assert.IsTrue(Extensions.PipeSegmentsFlowHasValue(pipe, false));
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
                Assert.IsTrue(Extensions.PipeSegmentsFlowHasValue(pipe, true));
            }
        }

        #endregion
    }
}