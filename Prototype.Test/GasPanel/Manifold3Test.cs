using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MugenMvvmToolkit;
using MugenMvvmToolkit.ViewModels;
using NUnit.Framework;
using Prototype.Core.Models.GasPanel;

namespace Prototype.Test.GasPanel
{
    [TestFixture]
    internal class Manifold3Test : UnitTestBase
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

        public class Manifold
        {
            #region Constructors

            public Manifold()
            {
                ValveVm1 = new ValveVm("Valve1");
                ValveVm2 = new ValveVm("Valve2");
                ValveVm3 = new ValveVm("Valve3");
                ValveVm4 = new ValveVm("Valve4");
                ValveVm5 = new ValveVm("Valve5");
                ValveVm6 = new ValveVm("Valve6");
                ValveVm7 = new ValveVm("Valve7");

                ValveVertex1 = new ValveVertex(ValveVm1);
                ValveVertex2 = new ValveVertex(ValveVm2);
                ValveVertex3 = new ValveVertex(ValveVm3);
                ValveVertex4 = new ValveVertex(ValveVm4);
                ValveVertex5 = new ValveVertex(ValveVm5);
                ValveVertex6 = new ValveVertex(ValveVm6);
                ValveVertex7 = new ValveVertex(ValveVm7);

                SourceVertex1 = new SourceVertex();
                SourceVertex2 = new SourceVertex();
                SourceVertex3 = new SourceVertex();
                SourceVertex4 = new SourceVertex();
                SourceVertex5 = new SourceVertex();
                SourceVertex6 = new SourceVertex();
                SourceVertex7 = new SourceVertex();

                DestinationVertex1 = new DestinationVertex();

                ConnectionVertex1 = new Vertex();
                ConnectionVertex2 = new Vertex();
                ConnectionVertex3 = new Vertex();
                ConnectionVertex4 = new Vertex();
                ConnectionVertex5 = new Vertex();

                Scheme = new PipeScheme(
                    new VerticesPair(SourceVertex1, ValveVertex1),
                    new VerticesPair(SourceVertex2, ValveVertex2),
                    new VerticesPair(SourceVertex3, ValveVertex3),
                    new VerticesPair(SourceVertex4, ValveVertex4),
                    new VerticesPair(SourceVertex5, ValveVertex5),
                    new VerticesPair(SourceVertex6, ValveVertex6),
                    new VerticesPair(SourceVertex7, ValveVertex7),
                    new VerticesPair(ValveVertex1, ConnectionVertex1),
                    new VerticesPair(ValveVertex2, ConnectionVertex1),
                    new VerticesPair(ConnectionVertex1, ConnectionVertex2),
                    new VerticesPair(ValveVertex3, ConnectionVertex2),
                    new VerticesPair(ConnectionVertex2, DestinationVertex1),
                    new VerticesPair(ConnectionVertex2, ConnectionVertex3),
                    new VerticesPair(ValveVertex4, ConnectionVertex3),
                    new VerticesPair(ConnectionVertex3, ConnectionVertex4),
                    new VerticesPair(ValveVertex5, ConnectionVertex4),
                    new VerticesPair(ConnectionVertex4, ConnectionVertex5),
                    new VerticesPair(ValveVertex6, ConnectionVertex5),
                    new VerticesPair(ValveVertex7, ConnectionVertex5)
                );
            }

            #endregion

            #region Properties

            public ValveVm ValveVm1 { get; }
            public ValveVm ValveVm2 { get; }
            public ValveVm ValveVm3 { get; }
            public ValveVm ValveVm4 { get; }
            public ValveVm ValveVm5 { get; }
            public ValveVm ValveVm6 { get; }
            public ValveVm ValveVm7 { get; }

            public PipeScheme Scheme { get; }

            public ValveVertex ValveVertex1 { get; }
            public ValveVertex ValveVertex2 { get; }
            public ValveVertex ValveVertex3 { get; }
            public ValveVertex ValveVertex4 { get; }
            public ValveVertex ValveVertex5 { get; }
            public ValveVertex ValveVertex6 { get; }
            public ValveVertex ValveVertex7 { get; }

            public SourceVertex SourceVertex1 { get; }
            public SourceVertex SourceVertex2 { get; }
            public SourceVertex SourceVertex3 { get; }
            public SourceVertex SourceVertex4 { get; }
            public SourceVertex SourceVertex5 { get; }
            public SourceVertex SourceVertex6 { get; }
            public SourceVertex SourceVertex7 { get; }

            public DestinationVertex DestinationVertex1 { get; }

            public Vertex ConnectionVertex1 { get; }
            public Vertex ConnectionVertex2 { get; }
            public Vertex ConnectionVertex3 { get; }
            public Vertex ConnectionVertex4 { get; }
            public Vertex ConnectionVertex5 { get; }

            #endregion
        }

        #endregion

        #region Tests

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
            var pipe_s2_v2 = manifold.Scheme.FindPipeVm(manifold.SourceVertex2, manifold.ValveVertex2);
            var pipe_s3_v3 = manifold.Scheme.FindPipeVm(manifold.SourceVertex3, manifold.ValveVertex3);
            var pipe_s4_v4 = manifold.Scheme.FindPipeVm(manifold.SourceVertex4, manifold.ValveVertex4);
            var pipe_s5_v5 = manifold.Scheme.FindPipeVm(manifold.SourceVertex5, manifold.ValveVertex5);
            var pipe_s6_v6 = manifold.Scheme.FindPipeVm(manifold.SourceVertex6, manifold.ValveVertex6);
            var pipe_s7_v7 = manifold.Scheme.FindPipeVm(manifold.SourceVertex7, manifold.ValveVertex7);
            var pipe_v1_c1 = manifold.Scheme.FindPipeVm(manifold.ValveVertex1, manifold.ConnectionVertex1);
            var pipe_v2_c1 = manifold.Scheme.FindPipeVm(manifold.ValveVertex2, manifold.ConnectionVertex1);
            var pipe_v3_c2 = manifold.Scheme.FindPipeVm(manifold.ValveVertex3, manifold.ConnectionVertex2);
            var pipe_v4_c3 = manifold.Scheme.FindPipeVm(manifold.ValveVertex4, manifold.ConnectionVertex3);
            var pipe_v5_c4 = manifold.Scheme.FindPipeVm(manifold.ValveVertex5, manifold.ConnectionVertex4);
            var pipe_v6_c5 = manifold.Scheme.FindPipeVm(manifold.ValveVertex6, manifold.ConnectionVertex5);
            var pipe_v7_c5 = manifold.Scheme.FindPipeVm(manifold.ValveVertex7, manifold.ConnectionVertex5);
            var pipe_c2_d1 = manifold.Scheme.FindPipeVm(manifold.ConnectionVertex2, manifold.DestinationVertex1);
            var pipe_c1_c2 = manifold.Scheme.FindPipeVm(manifold.ConnectionVertex1, manifold.ConnectionVertex2);
            var pipe_c2_c3 = manifold.Scheme.FindPipeVm(manifold.ConnectionVertex2, manifold.ConnectionVertex3);
            var pipe_c3_c4 = manifold.Scheme.FindPipeVm(manifold.ConnectionVertex3, manifold.ConnectionVertex4);
            var pipe_c4_c5 = manifold.Scheme.FindPipeVm(manifold.ConnectionVertex4, manifold.ConnectionVertex5);

            void TestV2V6opened()
            {
                manifold.ValveVm2.State = ValveState.Opened;
                manifold.ValveVm6.State = ValveState.Opened;

                Assert.IsFalse(pipe_s1_v1.HasFlow);
                Assert.IsTrue(pipe_s2_v2.HasFlow);
                Assert.IsFalse(pipe_s3_v3.HasFlow);
                Assert.IsFalse(pipe_s4_v4.HasFlow);
                Assert.IsFalse(pipe_s5_v5.HasFlow);
                Assert.IsTrue(pipe_s6_v6.HasFlow);
                Assert.IsFalse(pipe_s7_v7.HasFlow);
                Assert.IsFalse(pipe_v1_c1.HasFlow);
                Assert.IsTrue(pipe_v2_c1.HasFlow);
                Assert.IsFalse(pipe_v3_c2.HasFlow);
                Assert.IsFalse(pipe_v4_c3.HasFlow);
                Assert.IsFalse(pipe_v5_c4.HasFlow);
                Assert.IsTrue(pipe_v6_c5.HasFlow);
                Assert.IsFalse(pipe_v7_c5.HasFlow);
                Assert.IsTrue(pipe_c2_d1.HasFlow);
                Assert.IsTrue(pipe_c1_c2.HasFlow);
                Assert.IsTrue(pipe_c2_c3.HasFlow);
                Assert.IsTrue(pipe_c3_c4.HasFlow);
                Assert.IsTrue(pipe_c4_c5.HasFlow);
            }

            TestV2V6opened();
        }

        #endregion
    }
}
