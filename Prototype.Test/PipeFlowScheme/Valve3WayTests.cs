using System;
using System.Windows.Controls;
using System.Windows.Media.Imaging;
using MugenMvvmToolkit;
using NUnit.Framework;
using Prototype.Core.Controls.PipeFlowScheme;

namespace Prototype.Test.PipeFlowScheme
{
    [TestFixture]
    internal class Valve3WayTests : UnitTestBase
    {
        #region Nested types

        private class Manifold
        {
            private Manifold()
            {
            }

            public TestPipe PipeUpper { get; private set; }
            public TestPipe PipeLower { get; private set; }
            public TestPipe PipeAuxiliary { get; private set; }

            public TestValve3Way Valve { get; private set; }

            public static Manifold CreateManifold(Rotation rotation, PipeType upperPipeType, PipeType lowerPipeType, PipeType auxiliaryPipeType, Valve3WayFlowPath pathWhenOpen,
                Valve3WayFlowPath pathWhenClosed, bool isValveOpen)
            {
                var manifold = new Manifold();
                var container = new TestContainer();

                switch (rotation)
                {
                    case Rotation.Rotate0:
                        container.Add(manifold.PipeUpper =
                            new TestPipe(container) {Left = 36, Top = 21, Height = 41, Orientation = Orientation.Vertical, Type = upperPipeType});
                        container.Add(manifold.PipeLower =
                            new TestPipe(container) {Left = 36, Top = 57, Height = 41, Orientation = Orientation.Vertical, Type = lowerPipeType});
                        container.Add(manifold.PipeAuxiliary =
                            new TestPipe(container) {Left = 36, Top = 57, Width = 47, Type = auxiliaryPipeType});
                        container.Add(manifold.Valve = new TestValve3Way(container)
                            {Left = 20, Top = 38, PathWhenOpen = pathWhenOpen, PathWhenClosed = pathWhenClosed, IsOpen = isValveOpen});
                        break;
                    case Rotation.Rotate90:
                        container.Add(manifold.PipeUpper =
                            new TestPipe(container) {Left = 139, Top = 54, Width = 39, Type = upperPipeType});
                        container.Add(manifold.PipeLower =
                            new TestPipe(container) {Left = 101, Top = 54, Width = 43, Type = lowerPipeType});
                        container.Add(manifold.PipeAuxiliary =
                            new TestPipe(container) {Left = 139, Top = 54, Height = 41, Orientation = Orientation.Vertical, Type = auxiliaryPipeType});
                        container.Add(manifold.Valve = new TestValve3Way(container)
                            {Left = 120, Top = 38, Rotation = Rotation.Rotate90, PathWhenOpen = pathWhenOpen, PathWhenClosed = pathWhenClosed, IsOpen = isValveOpen});
                        break;
                    case Rotation.Rotate180:
                        container.Add(manifold.PipeUpper =
                            new TestPipe(container) {Left = 239, Top = 57, Height = 39, Orientation = Orientation.Vertical, Type = auxiliaryPipeType});
                        container.Add(manifold.PipeLower =
                            new TestPipe(container) {Left = 239, Top = 21, Height = 41, Orientation = Orientation.Vertical, Type = lowerPipeType});
                        container.Add(manifold.PipeAuxiliary =
                            new TestPipe(container) {Left = 200, Top = 57, Width = 44, Type = upperPipeType});
                        container.Add(manifold.Valve = new TestValve3Way(container)
                            {Left = 220, Top = 38, Rotation = Rotation.Rotate180, PathWhenOpen = pathWhenOpen, PathWhenClosed = pathWhenClosed, IsOpen = isValveOpen});
                        break;
                    case Rotation.Rotate270:
                        container.Add(manifold.PipeUpper =
                            new TestPipe(container) {Left = 290, Top = 60, Width = 47, Type = upperPipeType});
                        container.Add(manifold.PipeLower =
                            new TestPipe(container) {Left = 332, Top = 60, Width = 46, Type = lowerPipeType});
                        container.Add(manifold.PipeAuxiliary =
                            new TestPipe(container) {Left = 332, Top = 24, Height = 41, Orientation = Orientation.Vertical, Type = auxiliaryPipeType});
                        container.Add(manifold.Valve = new TestValve3Way(container)
                            {Left = 313, Top = 41, Rotation = Rotation.Rotate270, PathWhenOpen = pathWhenOpen, PathWhenClosed = pathWhenClosed, IsOpen = isValveOpen});
                        break;
                    default:
                        throw new ArgumentOutOfRangeException(nameof(rotation), rotation, null);
                }

                container.CreateGraph();
                return manifold;
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
        public void Test_Rotate0_Direct_UpperAuxiliary_Open()
        {
            var manifold = Manifold.CreateManifold(Rotation.Rotate0, PipeType.Source, PipeType.Destination, PipeType.Destination, 
                Valve3WayFlowPath.Direct, Valve3WayFlowPath.UpperAuxiliary, true);

            Assert.IsTrue(manifold.PipeUpper.PipeHasSegmentFlow(true, true, true));
            Assert.IsTrue(manifold.PipeLower.PipeHasSegmentFlow(true, true, true));
            Assert.IsTrue(manifold.PipeAuxiliary.PipeHasSegmentFlow(true, false, false));
        }

        [Test]
        public void Test_Rotate0_Direct_UpperAuxiliary_Closed()
        {
            var manifold = Manifold.CreateManifold(Rotation.Rotate0, PipeType.Source, PipeType.Destination, PipeType.Destination, 
                Valve3WayFlowPath.Direct, Valve3WayFlowPath.UpperAuxiliary, false);

            Assert.IsTrue(manifold.PipeUpper.PipeHasSegmentFlow(true, true, true));
            Assert.IsTrue(manifold.PipeLower.PipeHasSegmentFlow(true, false, false));
            Assert.IsTrue(manifold.PipeAuxiliary.PipeHasSegmentFlow(true, true, true));
        }


        [Test]
        public void Test_Rotate0_LowerAuxiliary_UpperAuxiliary_Open()
        {
            var manifold = Manifold.CreateManifold(Rotation.Rotate0, PipeType.Source, PipeType.Destination, PipeType.Destination, 
                Valve3WayFlowPath.LowerAuxiliary, Valve3WayFlowPath.UpperAuxiliary, true);

            Assert.IsTrue(manifold.PipeUpper.PipeHasSegmentFlow(false, false, false));
            Assert.IsTrue(manifold.PipeLower.PipeHasSegmentFlow(false, false, false));
            Assert.IsTrue(manifold.PipeAuxiliary.PipeHasSegmentFlow(false, false, false));
        }

        #endregion
    }
}