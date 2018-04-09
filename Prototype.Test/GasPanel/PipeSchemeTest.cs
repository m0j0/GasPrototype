using System;
using System.ComponentModel;
using MugenMvvmToolkit;
using MugenMvvmToolkit.Models;
using NUnit.Framework;
using Prototype.Core.Models.GasPanel;

namespace Prototype.Test.GasPanel
{
    [TestFixture]
    internal class PipeSchemeTest : UnitTestBase
    {
        #region SetUp

        [SetUp]
        public void SetUp()
        {
            Initialize(new MugenContainer());
        }

        #endregion

        #region Initialization

        [Test]
        public void TestPipeSchemeInitializationWithoutVertices()
        {
            Assert.Throws<InvalidOperationException>(() =>
            {
                var scheme = new PipeScheme();
            });
        }

        [Test]
        public void TestPipeSchemeInitializationWithoutDestinations()
        {
            var sourceVertex = new SourceVertex();
            var vertex = new Vertex();

            Assert.Throws<InvalidOperationException>(() =>
            {
                var scheme = new PipeScheme(
                    new VertexPair(sourceVertex, vertex)
                );
            });
        }

        [Test]
        public void TestPipeSchemeInitializationWithoutSources()
        {
            var destinationVertex = new DestinationVertex();
            var vertex = new Vertex();
            
            Assert.Throws<InvalidOperationException>(() =>
            {
                var scheme = new PipeScheme(
                    new VertexPair(vertex, destinationVertex)
                );
            });
        }

        [Test]
        public void TestPipeSchemeProperInitialization()
        {
            var sourceVertex = new SourceVertex();
            var destinationVertex = new DestinationVertex();

            Assert.DoesNotThrow(() =>
            {
                var scheme = new PipeScheme(
                    new VertexPair(sourceVertex, destinationVertex)
                );
            });
        }

        [Test]
        public void TestPipeSchemeTwiceInitialization()
        {
            var vertex1 = new SourceVertex();
            var vertex2 = new DestinationVertex();

            var scheme1 = new PipeScheme(
                new VertexPair(vertex1, vertex2)
            );

            Assert.Throws<InvalidOperationException>(() =>
            {
                var scheme2 = new PipeScheme(
                    new VertexPair(vertex1, vertex2)
                );
            });
        }

        #endregion

        #region Dispose

        [Test]
        public void DisposeTest()
        {
            var valveVm1 = new ValveVm();
            var valveVm2 = new ValveVm();

            var valveVertex1 = new ValveVertex(valveVm1);
            var valveVertex2 = new ValveVertex(valveVm2);

            var sourceVertex = new SourceVertex();

            var destinationVertex = new DestinationVertex();

            var count1 = GetPropertyChangedSubscribersCount(valveVm1);
            var count2 = GetPropertyChangedSubscribersCount(valveVm2);

            var scheme = new PipeScheme(
                new VertexPair(sourceVertex, valveVertex1),
                new VertexPair(valveVertex1, valveVertex2),
                new VertexPair(valveVertex2, destinationVertex)
            );

            Assert.Greater(GetPropertyChangedSubscribersCount(valveVm1), count1);
            Assert.Greater(GetPropertyChangedSubscribersCount(valveVm2), count2);

            scheme.Dispose();

            Assert.AreEqual(GetPropertyChangedSubscribersCount(valveVm1), count1);
            Assert.AreEqual(GetPropertyChangedSubscribersCount(valveVm2), count2);
        }

        private int GetPropertyChangedSubscribersCount(INotifyPropertyChanged obj)
        {
            var fieldInfo = obj.GetType().GetFieldEx(nameof(obj.PropertyChanged), MemberFlags.NonPublic | MemberFlags.Instance);
            var field = fieldInfo.GetValue(obj);
            var eventDelegate = (MulticastDelegate) field;
            return eventDelegate.GetInvocationList().Length;
        }

        [Test]
        public void TestUsingDisposedPipeScheme()
        {
            var sourceVertex = new SourceVertex();
            var destinationVertex = new DestinationVertex();

            var scheme = new PipeScheme(
                new VertexPair(sourceVertex, destinationVertex)
            );
            scheme.Dispose();

            Assert.Throws<ObjectDisposedException>(() => scheme.FindPipeVm(sourceVertex, destinationVertex));
        }

        #endregion
    }
}