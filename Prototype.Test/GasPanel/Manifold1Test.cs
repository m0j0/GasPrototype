using System;
using MugenMvvmToolkit;
using NUnit.Framework;
using Prototype.Core.Models.GasPanel;

namespace Prototype.Test.GasPanel
{
    [TestFixture]
    internal class Manifold1Test : UnitTestBase
    {
        #region Nested types

        // s1        s2
        // |         |
        // |         |
        // --c1------c2--
        //   |          |
        //   |          |
        //   v1         v2
        //   |          |
        //   |          |
        //   -----c3-----
        //        |
        //        |
        //        d1

        private class Manifold
        {
            #region Constructors

            public Manifold()
            {
                ValveVm1 = new ValveVm("Valve1");
                ValveVm2 = new ValveVm("Valve2");

                ValveVertex1 = new ValveVertex(ValveVm1);
                ValveVertex2 = new ValveVertex(ValveVm2);

                SourceVertex1 = new SourceVertex();
                SourceVertex2 = new SourceVertex();

                DestinationVertex1 = new DestinationVertex();

                ConnectionVertex1 = new Vertex();
                ConnectionVertex2 = new Vertex();
                ConnectionVertex3 = new Vertex();

                Scheme = new PipeScheme(
                    new VertexPair(SourceVertex1, ConnectionVertex1),
                    new VertexPair(ConnectionVertex1, ValveVertex1),
                    new VertexPair(ValveVertex1, ConnectionVertex3),
                    new VertexPair(SourceVertex2, ConnectionVertex2),
                    new VertexPair(ConnectionVertex2, ConnectionVertex1),
                    new VertexPair(ConnectionVertex2, ValveVertex2),
                    new VertexPair(ValveVertex2, ConnectionVertex3),
                    new VertexPair(ConnectionVertex3, DestinationVertex1)
                );

                Scheme.Initialize();
            }

            #endregion

            #region Properties

            public ValveVm ValveVm1 { get; }
            public ValveVm ValveVm2 { get; }

            public PipeScheme Scheme { get; }

            public ValveVertex ValveVertex1 { get; }
            public ValveVertex ValveVertex2 { get; }

            public SourceVertex SourceVertex1 { get; }
            public SourceVertex SourceVertex2 { get; }

            public DestinationVertex DestinationVertex1 { get; }

            public Vertex ConnectionVertex1 { get; }
            public Vertex ConnectionVertex2 { get; }
            public Vertex ConnectionVertex3 { get; }

            #endregion
        }

        #endregion

        #region SetUp

        [SetUp]
        public void SetUp()
        {
            Initialize(new MugenContainer());
        }

        #endregion

        #region Pipes

        [Test]
        public void TestPipes()
        {
            var manifold = new Manifold();

            var pipe_s1_c1 = manifold.Scheme.FindPipeVm(manifold.SourceVertex1, manifold.ConnectionVertex1);
            Assert.IsNotNull(pipe_s1_c1);
            var pipe_s2_c2 = manifold.Scheme.FindPipeVm(manifold.SourceVertex2, manifold.ConnectionVertex2);
            Assert.IsNotNull(pipe_s2_c2);
            var pipe_c1_c2 = manifold.Scheme.FindPipeVm(manifold.ConnectionVertex1, manifold.ConnectionVertex2);
            Assert.IsNotNull(pipe_c1_c2);
            var pipe_c1_v1 = manifold.Scheme.FindPipeVm(manifold.ConnectionVertex1, manifold.ValveVertex1);
            Assert.IsNotNull(pipe_c1_v1);
            var pipe_c2_v2 = manifold.Scheme.FindPipeVm(manifold.ConnectionVertex2, manifold.ValveVertex2);
            Assert.IsNotNull(pipe_c2_v2);
            var pipe_v1_c3 = manifold.Scheme.FindPipeVm(manifold.ValveVertex1, manifold.ConnectionVertex3);
            Assert.IsNotNull(pipe_v1_c3);
            var pipe_v2_c3 = manifold.Scheme.FindPipeVm(manifold.ValveVertex2, manifold.ConnectionVertex3);
            Assert.IsNotNull(pipe_v2_c3);
            var pipe_c3_d1 = manifold.Scheme.FindPipeVm(manifold.ConnectionVertex3, manifold.DestinationVertex1);
            Assert.IsNotNull(pipe_c3_d1);

            Assert.IsNull(manifold.Scheme.FindPipeVm(manifold.SourceVertex1, manifold.ConnectionVertex2));
            Assert.IsNull(manifold.Scheme.FindPipeVm(manifold.ConnectionVertex2, manifold.SourceVertex1));
            Assert.IsNull(manifold.Scheme.FindPipeVm(manifold.SourceVertex1, manifold.ConnectionVertex2));
        }

        #endregion

        #region Flows

        [Test]
        public void TestFlows()
        {
            var manifold = new Manifold();

            var pipe_s1_c1 = manifold.Scheme.FindPipeVm(manifold.SourceVertex1, manifold.ConnectionVertex1);
            var pipe_s2_c2 = manifold.Scheme.FindPipeVm(manifold.SourceVertex2, manifold.ConnectionVertex2);
            var pipe_c1_c2 = manifold.Scheme.FindPipeVm(manifold.ConnectionVertex1, manifold.ConnectionVertex2);
            var pipe_c1_v1 = manifold.Scheme.FindPipeVm(manifold.ConnectionVertex1, manifold.ValveVertex1);
            var pipe_c2_v2 = manifold.Scheme.FindPipeVm(manifold.ConnectionVertex2, manifold.ValveVertex2);
            var pipe_v1_c3 = manifold.Scheme.FindPipeVm(manifold.ValveVertex1, manifold.ConnectionVertex3);
            var pipe_v2_c3 = manifold.Scheme.FindPipeVm(manifold.ValveVertex2, manifold.ConnectionVertex3);
            var pipe_c3_d1 = manifold.Scheme.FindPipeVm(manifold.ConnectionVertex3, manifold.DestinationVertex1);

            void AllClosed()
            {
                manifold.ValveVm1.State = ValveState.Closed;
                manifold.ValveVm2.State = ValveState.Closed;
                Assert.IsFalse(pipe_s1_c1.HasFlow);
                Assert.IsFalse(pipe_s2_c2.HasFlow);
                Assert.IsFalse(pipe_c1_c2.HasFlow);
                Assert.IsFalse(pipe_c1_v1.HasFlow);
                Assert.IsFalse(pipe_c2_v2.HasFlow);
                Assert.IsFalse(pipe_v1_c3.HasFlow);
                Assert.IsFalse(pipe_v2_c3.HasFlow);
                Assert.IsFalse(pipe_c3_d1.HasFlow);
            }

            void V1Opened()
            {
                manifold.ValveVm1.State = ValveState.Opened;
                manifold.ValveVm2.State = ValveState.Closed;
                Assert.IsTrue(pipe_s1_c1.HasFlow);
                Assert.IsTrue(pipe_s2_c2.HasFlow);
                Assert.IsTrue(pipe_c1_c2.HasFlow);
                Assert.IsTrue(pipe_c1_v1.HasFlow);
                Assert.IsFalse(pipe_c2_v2.HasFlow);
                Assert.IsTrue(pipe_v1_c3.HasFlow);
                Assert.IsFalse(pipe_v2_c3.HasFlow);
                Assert.IsTrue(pipe_c3_d1.HasFlow);
            }

            void V2Opened()
            {
                manifold.ValveVm1.State = ValveState.Closed;
                manifold.ValveVm2.State = ValveState.Opened;
                Assert.IsTrue(pipe_s1_c1.HasFlow);
                Assert.IsTrue(pipe_s2_c2.HasFlow);
                Assert.IsTrue(pipe_c1_c2.HasFlow);
                Assert.IsFalse(pipe_c1_v1.HasFlow);
                Assert.IsTrue(pipe_c2_v2.HasFlow);
                Assert.IsFalse(pipe_v1_c3.HasFlow);
                Assert.IsTrue(pipe_v2_c3.HasFlow);
                Assert.IsTrue(pipe_c3_d1.HasFlow);
            }

            void AllOpened()
            {
                manifold.ValveVm1.State = ValveState.Opened;
                manifold.ValveVm2.State = ValveState.Opened;
                Assert.IsTrue(pipe_s1_c1.HasFlow);
                Assert.IsTrue(pipe_s2_c2.HasFlow);
                Assert.IsTrue(pipe_c1_c2.HasFlow);
                Assert.IsTrue(pipe_c1_v1.HasFlow);
                Assert.IsTrue(pipe_c2_v2.HasFlow);
                Assert.IsTrue(pipe_v1_c3.HasFlow);
                Assert.IsTrue(pipe_v2_c3.HasFlow);
                Assert.IsTrue(pipe_c3_d1.HasFlow);
            }

            AllClosed();
            V1Opened();
            V2Opened();
            AllOpened();
            V2Opened();
            V1Opened();
            AllClosed();
            AllOpened();
        }

        #endregion
    }
}