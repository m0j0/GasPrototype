using MugenMvvmToolkit;
using NUnit.Framework;
using Prototype.Core.Models.GasPanel;

namespace Prototype.Test.GasPanel
{
    [TestFixture]
    internal class Manifold2Test : UnitTestBase
    {
        #region Nested types
        
        //      d1              d2
        //      |               |
        //      |               |
        //      v7              v8
        //      |               |
        //      |               |
        // -----c1----     -----c4----
        // |    |    |     |    |    |
        // |    |    |     |    |    |
        // v1   v2   v3    v4   v5   v6
        // |    |    |     |    |    |
        // |    |    |     |    |    |
        // |    -----c2----c3----    |
        // |         |               |
        // s1        s2              s3

        private class Manifold
        {
            public Manifold()
            {
                ValveVm1 = new ValveVm("Valve1");
                ValveVm2 = new ValveVm("Valve2");
                ValveVm3 = new ValveVm("Valve3");
                ValveVm4 = new ValveVm("Valve4");
                ValveVm5 = new ValveVm("Valve5");
                ValveVm6 = new ValveVm("Valve6");
                ValveVm7 = new ValveVm("Valve7");
                ValveVm8 = new ValveVm("Valve8");

                ValveVertex1 = new ValveVertex(ValveVm1);
                ValveVertex2 = new ValveVertex(ValveVm2);
                ValveVertex3 = new ValveVertex(ValveVm3);
                ValveVertex4 = new ValveVertex(ValveVm4);
                ValveVertex5 = new ValveVertex(ValveVm5);
                ValveVertex6 = new ValveVertex(ValveVm6);
                ValveVertex7 = new ValveVertex(ValveVm7);
                ValveVertex8 = new ValveVertex(ValveVm8);

                SourceVertex1 = new SourceVertex();
                SourceVertex2 = new SourceVertex();
                SourceVertex3 = new SourceVertex();

                DestinationVertex1 = new DestinationVertex();
                DestinationVertex2 = new DestinationVertex();

                ConnectionVertex1 = new Vertex();
                ConnectionVertex2 = new Vertex();
                ConnectionVertex3 = new Vertex();
                ConnectionVertex4 = new Vertex();

                Scheme = new PipeScheme(
                    new VertexPair(SourceVertex1, ValveVertex1),
                    new VertexPair(ValveVertex1, ConnectionVertex1),
                    new VertexPair(ConnectionVertex1, ValveVertex7),
                    new VertexPair(ValveVertex7, DestinationVertex1),
                    new VertexPair(SourceVertex2, ConnectionVertex2),
                    new VertexPair(ConnectionVertex2, ValveVertex2),
                    new VertexPair(ConnectionVertex2, ValveVertex3),
                    new VertexPair(ValveVertex2, ConnectionVertex1),
                    new VertexPair(ValveVertex3, ConnectionVertex1),
                    new VertexPair(ConnectionVertex2, ConnectionVertex3),
                    new VertexPair(ConnectionVertex3, ValveVertex4),
                    new VertexPair(ConnectionVertex3, ValveVertex5),
                    new VertexPair(ValveVertex4, ConnectionVertex4),
                    new VertexPair(ValveVertex5, ConnectionVertex4),
                    new VertexPair(ConnectionVertex4, ValveVertex8),
                    new VertexPair(ValveVertex8, DestinationVertex2),
                    new VertexPair(SourceVertex3, ValveVertex6),
                    new VertexPair(ValveVertex6, ConnectionVertex4)
                );

                Scheme.Initialize();
            }

            public ValveVm ValveVm1 { get; }
            public ValveVm ValveVm2 { get; }
            public ValveVm ValveVm3 { get; }
            public ValveVm ValveVm4 { get; }
            public ValveVm ValveVm5 { get; }
            public ValveVm ValveVm6 { get; }
            public ValveVm ValveVm7 { get; }
            public ValveVm ValveVm8 { get; }

            public PipeScheme Scheme { get; }

            public ValveVertex ValveVertex1 { get; }
            public ValveVertex ValveVertex2 { get; }
            public ValveVertex ValveVertex3 { get; }
            public ValveVertex ValveVertex4 { get; }
            public ValveVertex ValveVertex5 { get; }
            public ValveVertex ValveVertex6 { get; }
            public ValveVertex ValveVertex7 { get; }
            public ValveVertex ValveVertex8 { get; }

            public SourceVertex SourceVertex1 { get; }
            public SourceVertex SourceVertex2 { get; }
            public SourceVertex SourceVertex3 { get; }

            public DestinationVertex DestinationVertex1 { get; }
            public DestinationVertex DestinationVertex2 { get; }

            public Vertex ConnectionVertex1 { get; }
            public Vertex ConnectionVertex2 { get; }
            public Vertex ConnectionVertex3 { get; }
            public Vertex ConnectionVertex4 { get; }
        }

        #endregion

        [SetUp]
        public void SetUp()
        {
            Initialize(new MugenContainer());
        }

        [Test]
        public void TestFlows()
        {
            var manifold = new Manifold();
            
            var pipe_s1_v1 = manifold.Scheme.FindPipeVm(manifold.SourceVertex1, manifold.ValveVertex1);
            var pipe_v1_c1 = manifold.Scheme.FindPipeVm(manifold.ValveVertex1, manifold.ConnectionVertex1);
            var pipe_c1_v7 = manifold.Scheme.FindPipeVm(manifold.ConnectionVertex1, manifold.ValveVertex7);
            var pipe_v7_d1 = manifold.Scheme.FindPipeVm(manifold.ValveVertex7, manifold.DestinationVertex1);
            var pipe_c1_v3 = manifold.Scheme.FindPipeVm(manifold.ConnectionVertex1, manifold.ValveVertex3);
            var pipe_v3_c2 = manifold.Scheme.FindPipeVm(manifold.ValveVertex3, manifold.ConnectionVertex2);
            var pipe_c1_v2 = manifold.Scheme.FindPipeVm(manifold.ConnectionVertex1, manifold.ValveVertex2);
            var pipe_v2_c2 = manifold.Scheme.FindPipeVm(manifold.ValveVertex2, manifold.ConnectionVertex2);
            var pipe_s2_c2 = manifold.Scheme.FindPipeVm(manifold.SourceVertex2, manifold.ConnectionVertex2);
            var pipe_c2_c3 = manifold.Scheme.FindPipeVm(manifold.ConnectionVertex2, manifold.ConnectionVertex3);
            var pipe_c3_v4 = manifold.Scheme.FindPipeVm(manifold.ConnectionVertex3, manifold.ValveVertex4);
            var pipe_v4_c4 = manifold.Scheme.FindPipeVm(manifold.ValveVertex4, manifold.ConnectionVertex4);
            var pipe_c3_v5 = manifold.Scheme.FindPipeVm(manifold.ConnectionVertex3, manifold.ValveVertex5);
            var pipe_v5_c4 = manifold.Scheme.FindPipeVm(manifold.ValveVertex5, manifold.ConnectionVertex4);
            var pipe_c4_v8 = manifold.Scheme.FindPipeVm(manifold.ConnectionVertex4, manifold.ValveVertex8);
            var pipe_v8_d2 = manifold.Scheme.FindPipeVm(manifold.ValveVertex8, manifold.DestinationVertex2);
            var pipe_c4_v6 = manifold.Scheme.FindPipeVm(manifold.ConnectionVertex4, manifold.ValveVertex6);
            var pipe_s3_v6 = manifold.Scheme.FindPipeVm(manifold.SourceVertex3, manifold.ValveVertex6);

            void AllClosed()
            {
                manifold.ValveVm1.State = ValveState.Closed;
                manifold.ValveVm2.State = ValveState.Closed;
                manifold.ValveVm3.State = ValveState.Closed;
                manifold.ValveVm4.State = ValveState.Closed;
                manifold.ValveVm5.State = ValveState.Closed;
                manifold.ValveVm6.State = ValveState.Closed;
                manifold.ValveVm7.State = ValveState.Closed;
                manifold.ValveVm8.State = ValveState.Closed;
                Assert.IsFalse(pipe_s1_v1.HasFlow);
                Assert.IsFalse(pipe_v1_c1.HasFlow);
                Assert.IsFalse(pipe_c1_v7.HasFlow);
                Assert.IsFalse(pipe_v7_d1.HasFlow);
                Assert.IsFalse(pipe_c1_v3.HasFlow);
                Assert.IsFalse(pipe_v3_c2.HasFlow);
                Assert.IsFalse(pipe_c1_v2.HasFlow);
                Assert.IsFalse(pipe_v2_c2.HasFlow);
                Assert.IsFalse(pipe_s2_c2.HasFlow);
                Assert.IsFalse(pipe_c2_c3.HasFlow);
                Assert.IsFalse(pipe_c3_v4.HasFlow);
                Assert.IsFalse(pipe_v4_c4.HasFlow);
                Assert.IsFalse(pipe_c3_v5.HasFlow);
                Assert.IsFalse(pipe_v5_c4.HasFlow);
                Assert.IsFalse(pipe_c4_v8.HasFlow);
                Assert.IsFalse(pipe_v8_d2.HasFlow);
                Assert.IsFalse(pipe_c4_v6.HasFlow);
                Assert.IsFalse(pipe_s3_v6.HasFlow);
            }

            void AllOpened()
            {
                manifold.ValveVm1.State = ValveState.Opened;
                manifold.ValveVm2.State = ValveState.Opened;
                manifold.ValveVm3.State = ValveState.Opened;
                manifold.ValveVm4.State = ValveState.Opened;
                manifold.ValveVm5.State = ValveState.Opened;
                manifold.ValveVm6.State = ValveState.Opened;
                manifold.ValveVm7.State = ValveState.Opened;
                manifold.ValveVm8.State = ValveState.Opened;
                Assert.IsTrue(pipe_s1_v1.HasFlow);
                Assert.IsTrue(pipe_v1_c1.HasFlow);
                Assert.IsTrue(pipe_c1_v7.HasFlow);
                Assert.IsTrue(pipe_v7_d1.HasFlow);
                Assert.IsTrue(pipe_c1_v3.HasFlow);
                Assert.IsTrue(pipe_v3_c2.HasFlow);
                Assert.IsTrue(pipe_c1_v2.HasFlow);
                Assert.IsTrue(pipe_v2_c2.HasFlow);
                Assert.IsTrue(pipe_s2_c2.HasFlow);
                Assert.IsTrue(pipe_c2_c3.HasFlow);
                Assert.IsTrue(pipe_c3_v4.HasFlow);
                Assert.IsTrue(pipe_v4_c4.HasFlow);
                Assert.IsTrue(pipe_c3_v5.HasFlow);
                Assert.IsTrue(pipe_v5_c4.HasFlow);
                Assert.IsTrue(pipe_c4_v8.HasFlow);
                Assert.IsTrue(pipe_v8_d2.HasFlow);
                Assert.IsTrue(pipe_c4_v6.HasFlow);
                Assert.IsTrue(pipe_s3_v6.HasFlow);
            }

            void OpenedV1V2V3V4V5V6()
            {
                manifold.ValveVm1.State = ValveState.Opened;
                manifold.ValveVm2.State = ValveState.Opened;
                manifold.ValveVm3.State = ValveState.Opened;
                manifold.ValveVm4.State = ValveState.Opened;
                manifold.ValveVm5.State = ValveState.Opened;
                manifold.ValveVm6.State = ValveState.Opened;
                manifold.ValveVm7.State = ValveState.Closed;
                manifold.ValveVm8.State = ValveState.Closed;
                Assert.IsFalse(pipe_s1_v1.HasFlow);
                Assert.IsFalse(pipe_v1_c1.HasFlow);
                Assert.IsFalse(pipe_c1_v7.HasFlow);
                Assert.IsFalse(pipe_v7_d1.HasFlow);
                Assert.IsFalse(pipe_c1_v3.HasFlow);
                Assert.IsFalse(pipe_v3_c2.HasFlow);
                Assert.IsFalse(pipe_c1_v2.HasFlow);
                Assert.IsFalse(pipe_v2_c2.HasFlow);
                Assert.IsFalse(pipe_s2_c2.HasFlow);
                Assert.IsFalse(pipe_c2_c3.HasFlow);
                Assert.IsFalse(pipe_c3_v4.HasFlow);
                Assert.IsFalse(pipe_v4_c4.HasFlow);
                Assert.IsFalse(pipe_c3_v5.HasFlow);
                Assert.IsFalse(pipe_v5_c4.HasFlow);
                Assert.IsFalse(pipe_c4_v8.HasFlow);
                Assert.IsFalse(pipe_v8_d2.HasFlow);
                Assert.IsFalse(pipe_c4_v6.HasFlow);
                Assert.IsFalse(pipe_s3_v6.HasFlow);
            }

            void OpenedV1V2V3V4V5V6V7()
            {
                manifold.ValveVm1.State = ValveState.Opened;
                manifold.ValveVm2.State = ValveState.Opened;
                manifold.ValveVm3.State = ValveState.Opened;
                manifold.ValveVm4.State = ValveState.Opened;
                manifold.ValveVm5.State = ValveState.Opened;
                manifold.ValveVm6.State = ValveState.Opened;
                manifold.ValveVm7.State = ValveState.Opened;
                manifold.ValveVm8.State = ValveState.Closed;
                Assert.IsTrue(pipe_s1_v1.HasFlow);
                Assert.IsTrue(pipe_v1_c1.HasFlow);
                Assert.IsTrue(pipe_c1_v7.HasFlow);
                Assert.IsTrue(pipe_v7_d1.HasFlow);
                Assert.IsTrue(pipe_c1_v3.HasFlow);
                Assert.IsTrue(pipe_v3_c2.HasFlow);
                Assert.IsTrue(pipe_c1_v2.HasFlow);
                Assert.IsTrue(pipe_v2_c2.HasFlow);
                Assert.IsTrue(pipe_s2_c2.HasFlow);
                Assert.IsTrue(pipe_c2_c3.HasFlow);
                Assert.IsTrue(pipe_c3_v4.HasFlow);
                Assert.IsTrue(pipe_v4_c4.HasFlow);
                Assert.IsTrue(pipe_c3_v5.HasFlow);
                Assert.IsTrue(pipe_v5_c4.HasFlow);
                Assert.IsFalse(pipe_c4_v8.HasFlow);
                Assert.IsFalse(pipe_v8_d2.HasFlow);
                Assert.IsTrue(pipe_c4_v6.HasFlow);
                Assert.IsTrue(pipe_s3_v6.HasFlow);
            }

            void OpenedV7V8()
            {
                manifold.ValveVm1.State = ValveState.Closed;
                manifold.ValveVm2.State = ValveState.Closed;
                manifold.ValveVm3.State = ValveState.Closed;
                manifold.ValveVm4.State = ValveState.Closed;
                manifold.ValveVm5.State = ValveState.Closed;
                manifold.ValveVm6.State = ValveState.Closed;
                manifold.ValveVm7.State = ValveState.Opened;
                manifold.ValveVm8.State = ValveState.Opened;
                Assert.IsFalse(pipe_s1_v1.HasFlow);
                Assert.IsFalse(pipe_v1_c1.HasFlow);
                Assert.IsFalse(pipe_c1_v7.HasFlow);
                Assert.IsFalse(pipe_v7_d1.HasFlow);
                Assert.IsFalse(pipe_c1_v3.HasFlow);
                Assert.IsFalse(pipe_v3_c2.HasFlow);
                Assert.IsFalse(pipe_c1_v2.HasFlow);
                Assert.IsFalse(pipe_v2_c2.HasFlow);
                Assert.IsFalse(pipe_s2_c2.HasFlow);
                Assert.IsFalse(pipe_c2_c3.HasFlow);
                Assert.IsFalse(pipe_c3_v4.HasFlow);
                Assert.IsFalse(pipe_v4_c4.HasFlow);
                Assert.IsFalse(pipe_c3_v5.HasFlow);
                Assert.IsFalse(pipe_v5_c4.HasFlow);
                Assert.IsFalse(pipe_c4_v8.HasFlow);
                Assert.IsFalse(pipe_v8_d2.HasFlow);
                Assert.IsFalse(pipe_c4_v6.HasFlow);
                Assert.IsFalse(pipe_s3_v6.HasFlow);
            }

            void OpenedV4V5V6V7V8()
            {
                manifold.ValveVm1.State = ValveState.Closed;
                manifold.ValveVm2.State = ValveState.Closed;
                manifold.ValveVm3.State = ValveState.Closed;
                manifold.ValveVm4.State = ValveState.Opened;
                manifold.ValveVm5.State = ValveState.Opened;
                manifold.ValveVm6.State = ValveState.Opened;
                manifold.ValveVm7.State = ValveState.Opened;
                manifold.ValveVm8.State = ValveState.Opened;
                Assert.IsFalse(pipe_s1_v1.HasFlow);
                Assert.IsFalse(pipe_v1_c1.HasFlow);
                Assert.IsFalse(pipe_c1_v7.HasFlow);
                Assert.IsFalse(pipe_v7_d1.HasFlow);
                Assert.IsFalse(pipe_c1_v3.HasFlow);
                Assert.IsFalse(pipe_v3_c2.HasFlow);
                Assert.IsFalse(pipe_c1_v2.HasFlow);
                Assert.IsFalse(pipe_v2_c2.HasFlow);
                Assert.IsTrue(pipe_s2_c2.HasFlow);
                Assert.IsTrue(pipe_c2_c3.HasFlow);
                Assert.IsTrue(pipe_c3_v4.HasFlow);
                Assert.IsTrue(pipe_v4_c4.HasFlow);
                Assert.IsTrue(pipe_c3_v5.HasFlow);
                Assert.IsTrue(pipe_v5_c4.HasFlow);
                Assert.IsTrue(pipe_c4_v8.HasFlow);
                Assert.IsTrue(pipe_v8_d2.HasFlow);
                Assert.IsTrue(pipe_c4_v6.HasFlow);
                Assert.IsTrue(pipe_s3_v6.HasFlow);
            }

            AllClosed();
            AllOpened();
            OpenedV1V2V3V4V5V6();
            OpenedV1V2V3V4V5V6V7();
            OpenedV7V8();
            OpenedV4V5V6V7V8();
        }
    }
}